using DLL.Entities;

namespace UsersAPI.Services.EntityServices.DI
{
    public interface IUserStateService
    {
        Task<UserState?> GetOrCreateAsync(State state);
        Task DeleteAsync(UserState group);
    }
}
