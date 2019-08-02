using EnChanger.Database.Abstractions;
using EnChanger.Database.Entities;
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.Entity<Entry>().HasKey(entry => entry.Id);
            modelBuilder.Entity<Entry>().Property(entry => entry.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Entry>().Property(entry => entry.Password).IsRequired();
            modelBuilder.Entity<Entry>().Property(entry => entry.NumberOfAccesses).IsConcurrencyToken().HasDefaultValue();
        }
    }
}
