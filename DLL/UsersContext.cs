

using DLL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DLL
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        public DbSet<UserState> UserStates { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options) { }

    }
}
