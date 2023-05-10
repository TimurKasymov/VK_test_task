using DLL.Abstractions;
using DLL.Entities;
using DLL.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UsersAPI.Services.EntityServices.DI;

namespace UsersAPI.Services.EntityServices
{
    public class UserService : IUserService
    {
        private IDbRepository<User> _repository;
        private ILogger<UserService> _logger;

        public UserService(IDbRepository<User> repository, ILogger<UserService> logger = null)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task CreateAsync(User user)
        {
            try
            { 
                await _repository.CreateAsync(user);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
            }
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            try
            {
                var user = await _repository.Get(x => x.Id == userId)
                    .Include(d => d.State)
                    .Include(d => d.Group)
                    .FirstOrDefaultAsync();

                if (user == null)
                    _logger?.LogInformation($"Объект User c id {userId} не найден");

                return user;
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<List<User>> GetManyAsync(Expression<Func<User, bool>> predicate, bool noTracking = false)
        {
            try
            {
                List<User> users;
                if (noTracking)
                    users = await _repository
                        .Get(predicate)
                        .Include(d => d.State)
                        .Include(d => d.Group)
                        .AsNoTracking()
                        .ToListAsync();
                else
                    users = await _repository
                        .Get(predicate)
                        .Include(d => d.State)
                        .Include(d => d.Group)
                        .ToListAsync();

                return users ?? new List<User>();
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
                return new List<User>();
            }
        } 

        public async Task UpdateAsync(User user)
        {
            try
            {
                _repository.Update(user);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
            }
        }

        public async Task DeleteManyAsync(List<User> users)
        {
            try
            {
                _repository.DeleteMany(users);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
            }
        }

        public async Task<User> GetFirst(Expression<Func<User, bool>> predicate)
        {
            try
            {
                var user = await _repository
                        .Get(predicate)
                        .Include(d => d.State)
                        .Include(d => d.Group)
                        .FirstOrDefaultAsync();

                return user;
            }

            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
                return null;
            }
        }
    }
}
