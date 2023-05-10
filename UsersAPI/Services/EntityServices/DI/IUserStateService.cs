using DLL.Entities;

namespace UsersAPI.Services.EntityServices.DI
{
    public interface IUserStateService
    {
        Task<UserState?> GetOrCreateAsync(State state);
        Task CreateStateAsync(UserState state);
        Task DeleteAsync(UserState group);
    }
}
