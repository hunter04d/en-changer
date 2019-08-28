using System;
using EnChanger.Database.Abstractions;
using EnChanger.Database.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace EnChanger.Database
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Entry> Entries { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entry>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e=> e.Id).ValueGeneratedOnAdd();
                b.Property(e=> e.Password).IsRequired();
                b.Property(e=> e.NumberOfAccesses).IsConcurrencyToken();
                b.Property(e => e.ValidUntil);
                b.Property(e=> e.CreatedAt);
            });
        }
    }
}
