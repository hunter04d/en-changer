using System;

namespace EnChanger.Database.Entities
{
    public class Entry
    {
        public Guid Id { get; set; }

        public string Password { get; set; } = null!;
    }
}
