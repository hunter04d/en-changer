using System;
using System.Collections.Generic;
using System.Linq;
using EnChanger.Database;
using EnChanger.Database.Abstractions;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Services.Abstractions;
using EnChanger.Services.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using static LanguageExt.Prelude;

namespace EnChanger.Services
{
    public class SessionService : ISessionService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IClock _clock;

        public SessionService(IApplicationDbContext dbContext, IClock clock)
        {
            _dbContext = dbContext;
            _clock = clock;
        }


        public SessionDto NewSession()
        {
            var session = new Session
            {
                Id = new Guid(),
                ExpiryTime = _clock.GetCurrentInstant() + Session.DefaultExpiryTime
            };
            _dbContext.Sessions.Add(session);
            _dbContext.SaveChanges();
            return new SessionDto(session.Id, session.ExpiryTime);
        }

        public Either<NotFoundException, Unit> AttachToSession(Entry e, Guid sessionId)
        {
            var session = _dbContext.Sessions.Find(sessionId);
            if (session == null)
            {
                return Left(new NotFoundException(nameof(session), sessionId));
            }

            if (SessionIsValid(session))
            {
                _dbContext.Sessions.Remove(session);
                _dbContext.SaveChanges();
                return Left(new NotFoundException(nameof(session), sessionId));
            }

            e.SessionId = sessionId;
            _dbContext.Entries.Update(e);
            _dbContext.SaveChanges();
            return Right(Unit.Default);
        }

        public Option<SessionDto> Get(Guid sessionId)
        {
            var session = _dbContext.Sessions.Find(sessionId);
            if (session == null)
            {
                return None;
            }

            if (SessionIsValid(session))
            {
                _dbContext.Sessions.Remove(session);
                _dbContext.SaveChanges();
                return None;
            }

            var dto = new SessionDto(session.Id, session.ExpiryTime);
            return Some(dto);
        }

        private bool SessionIsValid(Session session) => session.ExpiryTime < _clock.GetCurrentInstant();

        public IEnumerable<Guid> GetAssociatedEntriesIds(Guid sessionId) =>
            _dbContext.Entries.AsNoTracking()
                .Where(entry => entry.SessionId == sessionId)
                .Select(entry => entry.Id);
    }
}
