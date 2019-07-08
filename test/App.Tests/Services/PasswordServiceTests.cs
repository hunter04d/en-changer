using System;
using EnChanger.Database;
using EnChanger.Services;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace App.Tests.Services
{
    public class PasswordServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly EphemeralDataProtectionProvider _dataProtectionProvider;

        public PasswordServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("Db").Options;
            _dbContext = new ApplicationDbContext(dbContextOptions);
            _dataProtectionProvider = new EphemeralDataProtectionProvider();
        }

        [Fact]
        public void AddingEntryTest()
        {
            const string password = "12345";
            var sut = new PasswordService(_dbContext, _dataProtectionProvider);
            var entry = sut.Add(new PasswordDto(password));

            entry.Id.Should().NotBeEmpty();
            entry.Password.Should().NotBe(password);
        }

        [Fact]
        public void GettingEntryTest()
        {
            const string password = "12345";
            var sut = new PasswordService(_dbContext, _dataProtectionProvider);
            var entry = sut.Add(new PasswordDto(password));
            var either = sut.Get(entry.Id);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
