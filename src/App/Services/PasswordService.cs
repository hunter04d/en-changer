using System;
using System.Security.Cryptography;
using EnChanger.Database;
using EnChanger.Database.Abstractions;
using EnChanger.Database.Entities;
using EnChanger.Extensions;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Services.Abstractions;
using LanguageExt;
using Microsoft.AspNetCore.DataProtection;
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

        public Entry Add(Some<PasswordDto> passwordInput)
        {
            var password = passwordInput.Value.Password;
            var entry = new Entry
            {
                Password = _dataProtector.Protect(password)
            };
            _dbContext.Entries.Add(entry);
            _dbContext.SaveChanges();
            return entry;
        }

        public Either<BadRequestException, Option<PasswordDto>> Get(Some<Guid> id)
        {
            var entry = _dbContext.Entries.Find(id.Value);
            return entry == null
                ? Right(Option<PasswordDto>.None)
                : TryOption(() => _dataProtector.Unprotect(entry.Password))
                    .Map(s => new PasswordDto(s))
                    .ToEither(e => e.CatchAs<CryptographicException>())
                    .MapLeft(BadRequestException.From);
        }
    }

    public class PasswordDto
    {
        public string Password { get; }

        public PasswordDto(string password) => Password = password;
    }
}
