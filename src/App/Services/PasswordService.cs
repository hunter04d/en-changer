using System;
using System.Linq;
using System.Security.Cryptography;
using EnChanger.Database.Abstractions;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Services.Abstractions;
using LanguageExt;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;

namespace EnChanger.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IApplicationDbContext _dbContext;

        private readonly IDataProtector _dataProtector;

        internal static readonly string ProtectorPurpose = typeof(PasswordService).AssemblyQualifiedName;

        public PasswordService(IApplicationDbContext dbContext, IDataProtectionProvider dataProtectionProvider)
        {
            _dbContext = dbContext;
            _dataProtector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
        }

        public Entry Add(PasswordInput passwordInput)
        {
            var entry = new Entry
            {
                Password = _dataProtector.Protect(passwordInput.Password),
                NumberOfAccesses = passwordInput.NumberOfAccesses <= 1 ? 1 : passwordInput.NumberOfAccesses
            };
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
            var password = entry.Password;
            if (entry.NumberOfAccesses == 1)
            {
                _dbContext.Entries.Remove(entry);
                _dbContext.SaveChanges();
            }
            else
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

            try
            {
                var dto = new PasswordDto(_dataProtector.Unprotect(password));
                return Right(Some(dto));
            }
            catch (CryptographicException e)
            {
                return Left(BadRequestException.From(e));
            }
        }
    }

    public class PasswordInput
    {
        public string Password { get; }
        public uint NumberOfAccesses { get; }

        public PasswordInput(string password, uint numberOfAccesses)
        {
            Password = password;
            NumberOfAccesses = numberOfAccesses;
        }
    }

    public class PasswordDto
    {
        public string Password { get; }

        public PasswordDto(string password)
        {
            Password = password;
        }
    }
}
