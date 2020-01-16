using System;
using System.Linq;
using EnChanger.Database;
using EnChanger.Database.Abstractions;
using EnChanger.Database.Entities;
using EnChanger.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace EnChanger.Tests.Services
{
    public class SessionServiceTests
    {
        private readonly SessionService _sut;
        private readonly IApplicationDbContext _dbContext;
        private readonly FakeClock _clock;

        public SessionServiceTests()
        {
            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(nameof(SessionServiceTests))
                .Options;
            _dbContext = new ApplicationDbContext(dbOptions);
            _clock = new FakeClock(new Instant());
            _sut = new SessionService(_dbContext, _clock);
        }


        [Fact]
        public void NewSession()
        {
            var newSession = _sut.NewSession();

            newSession.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void AttachToSession_ShouldReturnRight_WhenIdIsValid()
        {
            var entry = new Entry("hello", null, null, _clock);
            _dbContext.Entries.Add(entry);
            _dbContext.SaveChanges();
            var session = _sut.NewSession();

            var result = _sut.AttachToSession(entry, session.Id);
            result.IsRight.Should().BeTrue();
            entry.Session.Should().NotBeNull();
            session.Entries.Should().NotBeEmpty();
        }

        [Fact]
        public void AttachToSession_ShouldReturnLeft_WhenSessionWithIdDoesNotExist()
        {
            var entry = new Entry("hello", null, null, _clock);
            _dbContext.Entries.Add(entry);
            _dbContext.SaveChanges();
            var result = _sut.AttachToSession(entry, new Guid(Enumerable.Repeat((byte) 0x41, 16).ToArray()));
            result.IsLeft.Should().BeTrue();
        }

        [Fact]
        public void GetSession_ReturnOk_WhenSessionIsValid()
        {
            var session = _sut.NewSession();
            var result = _sut.Get(session.Id);

            result.IsSome.Should().BeTrue();
        }

        [Fact]
        public void GetSession_RemovesSession_IfSessionIsExpired()
        {
            var session = _sut.NewSession();
            _clock.Advance(Session.DefaultExpiryTime + Duration.FromSeconds(1));

            var result = _sut.Get(session.Id);

            result.IsNone.Should().BeTrue();
            _dbContext.Sessions.Find(session.Id).Should().BeNull();
        }
    }
}
