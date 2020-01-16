using System;
using NodaTime;

namespace EnChanger.Database.Entities
{
    public class Entry
    {
        public Entry(Guid id, string password, uint? numberOfAccesses, Instant? validUntil, Instant createdAt)
        {
            Id = id;
            Password = password;
            NumberOfAccesses = numberOfAccesses;
            ValidUntil = validUntil;
            CreatedAt = createdAt;
        }

        public Entry(string password, uint? numberOfAccesses, Duration? validFor, IClock clock)
        {
            Password = password;
            NumberOfAccesses = numberOfAccesses;
            CreatedAt = clock.GetCurrentInstant();
            ValidUntil = CreatedAt + validFor;
        }

        public Guid Id { get; }

        public string Password { get; }

        public uint? NumberOfAccesses { get; set; }

        public Instant? ValidUntil { get; }

        public Instant CreatedAt { get; }

        public Guid? SessionId { get; set; }
        public Session? Session { get; set; }
    }
}
