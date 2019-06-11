using System;
using System.Security;
using EnChanger.Database;
using EnChanger.Database.Entities;
using EnChanger.Services.Abstractions;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.AspNetCore.DataProtection;

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

        public Result<Entry> Add(Some<PasswordDto> passwordInput)
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

        public OptionalResult<PasswordDto> Get(Some<Guid> id)
        {
            var entry = _dbContext.Entries.Find(id.Value);
            if (entry == null)
            {
                return None;
            }

            try
            {
                return new PasswordDto(_dataProtector.Unprotect(entry.Password));
            }
            catch (SecurityException e)
            {
                return new OptionalResult<PasswordDto>(e);
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
