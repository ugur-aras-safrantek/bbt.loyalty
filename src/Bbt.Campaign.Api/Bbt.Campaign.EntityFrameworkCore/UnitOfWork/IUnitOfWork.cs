using Bbt.Campaign.EntityFrameworkCore.Repositories;

namespace Bbt.Campaign.EntityFrameworkCore.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryAsync<T> GetRepository<T>() where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
