using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bbt.Campaign.EntityFrameworkCore.Repositories
{
    public interface IRepositoryAsync<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate); // LINQ desteği sunabilmek içinde expression'ları kullanıyoruz.
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetThenIncluding(List<Func<IQueryable<T>, IQueryable<T>>> funcs, params Expression<Func<T, object>>[] includes);
        //  IEnumerable<T> GetThenIncluding(Func<IQueryable<T>, IQueryable<T>> func);
        IQueryable<T> IQueryable();
        //IQueryable<T> Entities { get; }
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetPagedReponseAsync(int pageNumber, int pageSize);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
