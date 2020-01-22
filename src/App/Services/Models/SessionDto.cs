using System;
using NodaTime;

namespace EnChanger.Services.Models
{
    public class SessionDto
    {
        public SessionDto(Guid id, Instant expiryTime)
        {
            Id = id;
            ExpiryTime = expiryTime;
        }

        public Guid Id { get; }

        public Instant ExpiryTime { get; }
    }
}
