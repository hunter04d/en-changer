using System;
using System.Linq;
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
        public void AddingEntry_Test()
        {
            var sut = new PasswordService(_dbContext, _dataProtectionProvider);
            var entry = sut.Add(new PasswordInput(Password, 1));

            entry.Id.Should().NotBeEmpty();

            _dataProtector.Unprotect(entry.Password).Should().Be(Password);
        }

        [Fact]
        public void GettingSingleUseEntryTest()
        {
            var sut = new PasswordService(_dbContext, _dataProtectionProvider);
            var entry = new Entry
            {
                Password = _dataProtector.Protect(Password),
                NumberOfAccesses = 1
            };
            _dbContext.Entries.Add(entry);

            var result = sut.Get(entry.Id).ValueUnsafe().ValueUnsafe();

            result.Should().NotBeNull();
            result.Password.Should().Be(Password);
            _dbContext.Entries.AsEnumerable().Should().NotContain(entry);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(20)]
        public void GettingEntriesMultipleTimesTest(uint numberOfAccesses)
        {
            var sut = new PasswordService(_dbContext, _dataProtectionProvider);
            var entry = new Entry
            {
                Password = _dataProtector.Protect(Password),
                NumberOfAccesses = numberOfAccesses
            };
            _dbContext.Entries.Add(entry);
            for (uint i = 0; i < numberOfAccesses; i++)
            {
                var result = sut.Get(entry.Id).ValueUnsafe().ValueUnsafe();
                result.Should().NotBeNull();
                result.Password.Should().Be(Password);
            }

            //_dbContext.SaveChanges();
            var entries = _dbContext.Entries.AsEnumerable();
            entries.Should().NotContain(entry);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
