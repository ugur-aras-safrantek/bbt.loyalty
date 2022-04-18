using Bbt.Campaign.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Bbt.Campaign.EntityFrameworkCore.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryAsync<T> GetRepository<T>() where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();

        Task<List<T>> RawSqlQuery<T>(string query, Func<DbDataReader, T> map);
    }
}
