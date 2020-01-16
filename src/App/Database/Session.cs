using System;
using System.Collections.Generic;
using EnChanger.Database.Entities;
using NodaTime;

namespace EnChanger.Database
{
    public class Session
    {
        public static Duration DefaultExpiryTime = Duration.FromDays(1);


        public Guid Id { get; set; }

        public Instant ExpiryTime { get; set; }

        public ICollection<Entry> Entries { get; } = new List<Entry>();
    }
}
