using EnChanger.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnChanger.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entry>().HasKey(entry => entry.Id);
            modelBuilder.Entity<Entry>().Property(entry => entry.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Entry>().Property(entry => entry.Password).IsRequired();
        }
    }
}
