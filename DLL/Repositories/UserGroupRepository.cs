using DLL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Repositories
{
    public sealed class UserGroupRepository : DbRepository<UserGroup>
    {
        public UserGroupRepository(UsersContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
