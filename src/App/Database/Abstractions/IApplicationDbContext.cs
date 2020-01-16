using System.Threading;
using System.Threading.Tasks;
using EnChanger.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnChanger.Database.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<Entry> Entries { get; }

        DbSet<Session> Sessions { get; }
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
