using DLL.Entities;

namespace UsersAPI.Services.EntityServices.DI
{
    public interface IUserGroupService
    {
        Task<UserGroup?> GetOrCreateAsync(Role role);
        Task CreateGroupAsync(UserGroup userGroup);
        Task DeleteAsync(UserGroup group);
    }
}
