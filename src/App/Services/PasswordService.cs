using System;
using System.Security;
using System.Security.Cryptography;
using EnChanger.Database;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Services.Abstractions;
using LanguageExt;
using Microsoft.AspNetCore.DataProtection;
using static LanguageExt.Prelude;

namespace EnChanger.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ITimeLimitedDataProtector _dataProtector;

        public PasswordService(ApplicationDbContext dbContext, IDataProtectionProvider dataProtectionProvider)
        {
            _dbContext = dbContext;
            _dataProtector = dataProtectionProvider.CreateProtector("Password").ToTimeLimitedDataProtector();
        }

        public Entry Add(Some<PasswordDto> passwordInput)
        {
            var password = passwordInput.Value.Password;
            var entry = new Entry
            {
                Password = _dataProtector.Protect(password, TimeSpan.FromMinutes(5))
            };
            _dbContext.Add(entry);
            _dbContext.SaveChanges();
            return entry;
        }

        public Either<BadRequestException, Option<PasswordDto>> Get(Some<Guid> id)
        {
            var entry = _dbContext.Entries.Find(id.Value);
            if (entry == null)
                return Right(Option<PasswordDto>.None);
            try
            {
                return Right(Some(new PasswordDto(_dataProtector.Unprotect(entry.Password))));
            }
            catch (SecurityException e)
            {
                return Left(new BadRequestException(e));
            }
        }
    }

    public class PasswordDto
    {
        public PasswordDto(string password)
        {
            Password = password;
        }

        public string Password { get; set; }
    }
}
