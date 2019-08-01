using System;
using System.Runtime.CompilerServices;
using EnChanger.Database;
using EnChanger.Database.Entities;
using EnChanger.Services;
using FluentAssertions;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace App.Tests.Services
{
    public class PasswordServiceTests : IDisposable
    {
        private const string Password = "12345";

        private readonly ApplicationDbContext _dbContext;
        private readonly EphemeralDataProtectionProvider _dataProtectionProvider;
        private readonly IDataProtector _dataProtector;

        public PasswordServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("Db").Options;
            _dbContext = new ApplicationDbContext(dbContextOptions);
            _dataProtectionProvider = new EphemeralDataProtectionProvider();
            _dataProtector = _dataProtectionProvider.CreateProtector(PasswordService.ProtectorPurpose);
        }

        [Fact]
        public void AddingEntryTest()
        {
            var sut = new PasswordService(_dbContext, _dataProtectionProvider);
            var entry = sut.Add(new PasswordDto(Password));

            entry.Id.Should().NotBeEmpty();

            _dataProtector.Unprotect(entry.Password).Should().Be(Password);
        }

        [Fact]
        public void GettingEntryTest()
        {
            var sut = new PasswordService(_dbContext, _dataProtectionProvider);
            var entry = new Entry
            {
                Password = _dataProtector.Protect(Password)
            };
            _dbContext.Entries.Add(entry);

            var result = sut.Get(entry.Id);
            var value = result.ValueUnsafe().ValueUnsafe();
            value.Should().NotBeNull();
            value.Password.Should().Be(Password);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
