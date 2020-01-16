using System;
using EnChanger.Database.Abstractions;
using EnChanger.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace EnChanger.Database
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Entry> Entries { get; set; } = null!;

        public DbSet<Session> Sessions { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entry>(Configure);
            modelBuilder.Entity<Session>(Configure);
        }

        private static void Configure(EntityTypeBuilder<Session> b)
        {
            b.HasKey(s => s.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();
            b.Property(s => s.ExpiryTime).IsRequired();
            b.HasMany(s => s.Entries)
                .WithOne(e => e.Session!)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void Configure(EntityTypeBuilder<Entry> b)
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();
            b.Property(e => e.Password).IsRequired();
            b.Property(e => e.NumberOfAccesses).IsConcurrencyToken();
            b.Property(e => e.ValidUntil);
            b.Property(e => e.CreatedAt);
        }
    }
}
