using System;
using System.Collections.Generic;
using EnChanger.Database;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Services.Models;
using LanguageExt;

namespace EnChanger.Services.Abstractions
{
    public interface ISessionService
    {
        SessionDto NewSession();

        Either<NotFoundException, Unit> AttachToSession(Entry e, Guid sessionId);

        Option<SessionDto> Get(Guid sessionId);

        public IEnumerable<Guid> GetAssociatedEntriesIds(Guid sessionId);
    }
}
