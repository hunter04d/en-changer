using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EnChanger.Database;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Services;
using EnChanger.Services.Abstractions;
using EnChanger.Services.Models;
using FluentAssertions;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using NodaTime.Testing;
using Xunit;
using static LanguageExt.Prelude;

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
        private readonly Mock<ISessionService> _sessionService;

        private readonly PasswordService _sut;


        public PasswordServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("Db").Options;
            _dbContext = new ApplicationDbContext(dbContextOptions);
            _dataProtectionProvider = new EphemeralDataProtectionProvider();
            _dataProtector = _dataProtectionProvider.CreateProtector(PasswordService.ProtectorPurpose);
            _clock = new FakeClock(new Instant());
            _sessionService = new Mock<ISessionService>();
            _sut = new PasswordService(
                _dbContext,
                _dataProtectionProvider,
                _clock,
                _sessionService.Object,
                NullLogger<PasswordService>.Instance
            );
        }

        [Fact]
        public void EntryCanBeAdded()
        {
            var id = _sut.Add(new PasswordInput(Password, 1));


            id.Should().NotBeEmpty();

            var entry = _dbContext.Entries.Find(id);
            entry.Should().NotBeNull();

            _dataProtector.Unprotect(entry.Password).Should().Be(Password);
        }

        [Fact]
        public void AddingEntry_ShouldReturn_WhenAttachingToSessionWasRight()
        {
            _sessionService.Setup(service => service.AttachToSession(It.IsAny<Entry>(), It.IsAny<Guid>()))
                .Returns(Right(Unit.Default));
            var entry = _sut.Add(new PasswordInput(Password, 1, null, Guid.NewGuid()));
            entry.Should().NotBeEmpty();
        }

        [Fact]
        public void AddingEntry_ShouldReturn_WhenAttachingToSessionWasLeft()
        {
            _sessionService.Setup(service => service.AttachToSession(It.IsAny<Entry>(), It.IsAny<Guid>()))
                .Returns(Left(new NotFoundException("test", "key")));
            var entry = _sut.Add(new PasswordInput(Password, 1, null, Guid.NewGuid()));

            entry.Should().NotBeEmpty();
        }

        [Fact]
        public void EntryCanBeGot_WhenNumberOfAccessesIsNull()
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
