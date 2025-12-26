using Account.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbcontext;
        public BaseRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity is NULL");

            await _dbcontext.AddAsync(entity);

            return await Task.FromResult(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbcontext.Set<TEntity>();
        }

        public void Remove(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity is NULL");

            _dbcontext.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbcontext.SaveChangesAsync();
        }

        public TEntity Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity is NULL");

            _dbcontext.Update(entity);

            return entity;
        }
    }
}
