using DLL.Abstractions;
using DLL.Entities;
using System.Linq.Expressions;

namespace DLL.Repositories
{
    /// <summary>
    /// Класс, который используется для работы с БД через EF Core
    /// </summary>
    public class DbRepository<TEntity> : IDbRepository<TEntity> where TEntity : class
    {
        private readonly UsersContext _dbContext;

        public DbRepository(UsersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
        public void DeleteMany(IEnumerable<TEntity> entites)
        {
            _dbContext.Set<TEntity>().RemoveRange(entites);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateManyAsync(IEnumerable<TEntity> entity)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entity);
        }
    }
}
