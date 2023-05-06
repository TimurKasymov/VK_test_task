using DLL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Repositories
{
    public sealed class UserRepository : DbRepository<User>
    {
        public UserRepository(UsersContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
