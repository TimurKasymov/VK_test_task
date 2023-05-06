

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace DLL
{
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<UsersContext>
    {
        public ApplicationContextFactory()
        {
        }
        public UsersContext CreateDbContext(string[] args)
        {
            var connection = "Server=localhost;Port=5432;Database=Users1;User Id=postgres;Password=kasymov2002;Timeout=300;CommandTimeout=300";
            var options = new DbContextOptionsBuilder<UsersContext>()
                .UseNpgsql(connection)
                .Options;

            return new UsersContext(options);
        }
    }
}
