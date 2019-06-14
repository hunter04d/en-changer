using System;
using EnChanger.Database;
using EnChanger.Database.Entities;
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

        public Try<Entry> Add(Some<PasswordDto> passwordInput)
        {
            var password = passwordInput.Value.Password;
            var entry = new Entry
            {
                Password = _dataProtector.Protect(password, TimeSpan.FromMinutes(5))
            };
            _dbContext.Add(entry);
            _dbContext.SaveChanges();
            return Try(entry);
        }

        public TryOption<PasswordDto> Get(Some<Guid> id) => TryOption(() =>
            Optional(_dbContext.Entries.Find(id.Value))
                .Map(entry => new PasswordDto(_dataProtector.Unprotect(entry.Password))));
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
