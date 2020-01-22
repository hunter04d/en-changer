using System;
using NodaTime;

namespace EnChanger.Services.Models
{
    public class PasswordInput
    {
        public string Password { get; }
        public uint? NumberOfAccesses { get; }

        public Duration? Duration { get; }

        public Guid? SessionId { get; }

        public PasswordInput(string password, uint? numberOfAccesses, Duration? duration = null, Guid? sessionId = null)
        {
            Password = password;
            NumberOfAccesses = numberOfAccesses;
            Duration = duration;
            SessionId = sessionId;
        }
    }
}
