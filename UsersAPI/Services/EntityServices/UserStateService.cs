using DLL.Abstractions;
using DLL.Entities;
using DLL.Migrations;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Services.EntityServices.DI;

namespace UsersAPI.Services.EntityServices
{
    public class UserStateService : IUserStateService
    {
        private IDbRepository<UserState> _repository;
        private ILogger<UserStateService> _logger;

        public UserStateService(IDbRepository<UserState> repository, ILogger<UserStateService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<UserState?> GetOrCreateAsync(State state)
        {
            try
            {
                var found = await _repository.Get(us => us.State == state).FirstOrDefaultAsync();
                if (found != null)
                    return found;
                var createdState = new UserState(state, "if state is blocked, the user has been removed");
                await _repository.CreateAsync(createdState);
                return createdState;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task DeleteAsync(UserState state)
        {
            try
            {
                _repository.Delete(state);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
            }
        }

        public async Task CreateStateAsync(UserState state)
        {
            try
            {
                await _repository.CreateAsync(state);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
            }
        }
    }
}
