using DLL.Entities;


namespace DLL.Repositories
{
    public sealed class UserStateRepository : DbRepository<UserState>
    {
        public UserStateRepository(UsersContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
