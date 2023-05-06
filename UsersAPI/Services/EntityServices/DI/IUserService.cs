using DLL.Entities;
using DLL.Migrations;
using System.Linq.Expressions;

namespace UsersAPI.Services.EntityServices.DI
{
    public interface IUserService
    {
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task<User?> GetFirst(Expression<Func<User, bool>> predicate);
        Task<List<User>> GetManyAsync(Expression<Func<User, bool>> predicate, bool noTracking = false);
        Task<User?> GetByIdAsync(int userId);
        Task DeleteManyAsync(List<User> users);

    }
}
