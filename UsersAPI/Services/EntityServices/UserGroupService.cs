using DLL.Abstractions;
using DLL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UsersAPI.Services.EntityServices.DI;

namespace UsersAPI.Services.EntityServices
{
    public class UserGroupService : IUserGroupService
    {
        private IDbRepository<UserGroup> _repository;
        private ILogger<UserGroupService> _logger;

        public UserGroupService(IDbRepository<UserGroup> repository, ILogger<UserGroupService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<UserGroup?> GetOrCreateAsync(Role role)
        {
            try
            {
                var found = await _repository.Get(us => us.Role == role).FirstOrDefaultAsync();
                if (found != null)
                    return found;
                var createdGroup = new UserGroup(role, "admin has all the rights, user does not");
                await _repository.CreateAsync(createdGroup);
                return createdGroup;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task DeleteAsync(UserGroup group)
        {
            try
            {
                _repository.Delete(group);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
            }
        }

    }
}
