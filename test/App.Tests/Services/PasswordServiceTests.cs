using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EnChanger.Database;
using EnChanger.Database.Entities;
using EnChanger.Services;
using EnChanger.Services.Models;
using FluentAssertions;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace EnChanger.Tests.Services
{
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    public class PasswordServiceTests : IDisposable
    {
        private const string Password = "12345";

        private readonly ApplicationDbContext _dbContext;
        private readonly EphemeralDataProtectionProvider _dataProtectionProvider;
        private readonly IDataProtector _dataProtector;
        private readonly FakeClock _clock;

        private readonly PasswordService _sut;

        public PasswordServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("Db").Options;
            _dbContext = new ApplicationDbContext(dbContextOptions);
            _dataProtectionProvider = new EphemeralDataProtectionProvider();
            _dataProtector = _dataProtectionProvider.CreateProtector(PasswordService.ProtectorPurpose);
            _clock = new FakeClock(new Instant());
            _sut = new PasswordService(_dbContext, _dataProtectionProvider, _clock);
        }

        [Fact]
        public void EntryCanBeAdded()
        {
            var entry = _sut.Add(new PasswordInput(Password, 1));

            entry.Id.Should().NotBeEmpty();

            _dataProtector.Unprotect(entry.Password).Should().Be(Password);
        }

        [Fact]
        public void EntryCanBeGot()
        {
            var entry = new Entry(_dataProtector.Protect(Password), null, null, _clock);
            _dbContext.Entries.Add(entry);

            var result = _sut.Get(entry.Id).ValueUnsafe().ValueUnsafe();

            result.Should().NotBeNull();
            result.Password.Should().Be(Password);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(20)]
        [InlineData(100)]
        public void GettingEntriesMultipleTimes_ShouldWork(uint numberOfAccesses)
        {
            var entry = new Entry(_dataProtector.Protect(Password), numberOfAccesses, null, _clock);
            _dbContext.Entries.Add(entry);
            for (uint i = 0; i < numberOfAccesses; i++)
            {
                var result = _sut.Get(entry.Id).ValueUnsafe().ValueUnsafe();
                result.Should().NotBeNull();
            }
            var entries = _dbContext.Entries.AsEnumerable();
            entries.Should().NotContain(entry);
        }

        [Fact]
        public void GettingEntry_BeforeExpirationTime_ShouldReturnEntry()
        {
            var entry = new Entry(_dataProtector.Protect(Password), null, Duration.FromMinutes(1), _clock);
            _dbContext.Entries.Add(entry);
            _clock.AdvanceSeconds(59);
            var result = _sut.Get(entry.Id).ValueUnsafe().ValueUnsafe();
            result.Should().NotBeNull();
        }

        [Fact]
        public void GettingEntry_AfterExpirationTime_ShouldReturnLeft()
        {
            var entry = new Entry(_dataProtector.Protect(Password), null, Duration.FromMinutes(1), _clock);
            _dbContext.Entries.Add(entry);
            _clock.AdvanceMinutes(3);
            var result = _sut.Get(entry.Id);
            result.IsLeft.Should().BeTrue();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
