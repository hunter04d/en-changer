using System;
using System.Linq;
using System.Security.Cryptography;
using EnChanger.Database.Abstractions;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Services.Abstractions;
using EnChanger.Services.Models;
using LanguageExt;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using static LanguageExt.Prelude;

namespace EnChanger.Services
{
    public class PasswordService : IPasswordService
    {
        internal static readonly Duration GraceDuration = Duration.FromSeconds(10);

        private readonly IApplicationDbContext _dbContext;

        private readonly IDataProtector _dataProtector;

        private readonly IClock _clock;

        internal static readonly string ProtectorPurpose = typeof(PasswordService).AssemblyQualifiedName!;

        public PasswordService(
            IApplicationDbContext dbContext,
            IDataProtectionProvider dataProtectionProvider,
            IClock clock
        )
        {
            _dbContext = dbContext;
            _clock = clock;
            _dataProtector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
        }

        public Entry Add(PasswordInput passwordInput)
        {
            var password = _dataProtector.Protect(passwordInput.Password);
            var numberOfAccesses = passwordInput.NumberOfAccesses == null ? null :
                passwordInput.NumberOfAccesses <= 1 ? 1 : passwordInput.NumberOfAccesses;
            var validFor = passwordInput.Duration;
            var entry = new Entry(password, numberOfAccesses, validFor, _clock);
            _dbContext.Entries.Add(entry);
            _dbContext.SaveChanges();
            return entry;
        }

        public Either<BadRequestException, Option<PasswordDto>> Get(Guid id)
        {
            var entry = _dbContext.Entries.Find(id);
            if (entry == null)
            {
                return Right(Option<PasswordDto>.None);
            }

            var expired = entry.ValidUntil != null && entry.ValidUntil + GraceDuration <= _clock.GetCurrentInstant();
            if (expired)
            {
                _dbContext.Entries.Remove(entry);
                _dbContext.SaveChanges();
                return Left(new BadRequestException("Entry expired"));
            }

            switch (entry.NumberOfAccesses)
            {
                case null:
                    break;
                case 1:
                    _dbContext.Entries.Remove(entry);
                    _dbContext.SaveChanges();
                    break;
                default:
                {
                    SubtractOneAccess(entry);
                    break;
                }
            }

            try
            {
                var dto = new PasswordDto(_dataProtector.Unprotect(entry.Password));
                return Right(Some(dto));
            }
            catch (CryptographicException e)
            {
                return Left(BadRequestException.From(e));
            }
        }

        private void SubtractOneAccess(Entry entry)
        {
            var saved = false;
            do
            {
                entry.NumberOfAccesses -= 1;
                try
                {
                    _dbContext.SaveChanges();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException e)
                {
                    e.Entries.Single().Reload();
                }
            } while (!saved);
        }
    }
}
