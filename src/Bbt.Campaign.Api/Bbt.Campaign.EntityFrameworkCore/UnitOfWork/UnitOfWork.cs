using Bbt.Campaign.EntityFrameworkCore.Context;
using Bbt.Campaign.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Bbt.Campaign.EntityFrameworkCore.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CampaignDbContext _dbContext;

        public UnitOfWork(CampaignDbContext dbContext)
        {
            //Database.SetInitializer<safrandev_sigortacini_dbContext>(null);

            if (dbContext == null)
                throw new ArgumentNullException("dbContext can not be null.");

            _dbContext = dbContext;

            // Buradan istediğiniz gibi EntityFramework'ü konfigure edebilirsiniz.
            //_dbContext.Configuration.LazyLoadingEnabled = false;
            //_dbContext.Configuration.ValidateOnSaveEnabled = false;
            //_dbContext.Configuration.ProxyCreationEnabled = false;
        }

        public async Task<List<T>> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        {
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand()) 
            {
                command.CommandText = query;
                command.CommandType = CommandType.StoredProcedure;

                //command.Parameters.AddRange();

                //_dbContext.

                //var userType = _dbContext.Set<T>.FromSql("dbo.SomeSproc @Id = {0}, @Name = {1}", 45, "Ada"

                _dbContext.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var entities = new List<T>();

                    while (result.Read())
                    {
                        entities.Add(map(result));
                    }
                    return entities;
                }
            }  
        }

        #region IUnitOfWork Members

        public IRepositoryAsync<T> GetRepository<T>() where T : class
        {
            return new RepositoryAsync<T>(_dbContext);
        }

        public int SaveChanges()
        {
            try
            {
                // Transaction işlemleri burada ele alınabilir veya Identity Map kurumsal tasarım kalıbı kullanılarak
                // sadece değişen alanları güncellemeyide sağlayabiliriz.
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Burada DbEntityValidationException hatalarını handle edebiliriz.
                throw;
            }
        }


        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region IDisposable Members
        // Burada IUnitOfWork arayüzüne implemente ettiğimiz IDisposable arayüzünün Dispose Patternini implemente ediyoruz.
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                    Console.WriteLine("virtual dispose");
                }
            }

            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            Console.WriteLine("Dispose");
        }
        #endregion
    }
}
