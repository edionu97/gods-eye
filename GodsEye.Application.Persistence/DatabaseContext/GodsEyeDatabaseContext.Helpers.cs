using Microsoft.EntityFrameworkCore;

namespace GodsEye.Application.Persistence.DatabaseContext
{
    public partial class GodsEyeDatabaseContext
    {
        /// <summary>
        /// This function it is used for specifying  the connection string
        /// </summary>
        /// <param name="connectionString">the string thar represents the connection string</param>
        /// <returns>an instance of DbContextOptions</returns>
        private static DbContextOptions GetConnectionOptions(string connectionString)
        {
            return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
        }
    }
}
