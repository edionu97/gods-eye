using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GodsEye.Application.Persistence.Models;
using GodsEye.Application.Persistence.DatabaseContext;
using GodsEye.Application.Persistence.Repository.Abstract;

namespace GodsEye.Application.Persistence.Repository.Impl
{
    public class UserRepository : AbstractRepository<User, GodsEyeDatabaseContext>, IUserRepository
    {
        public UserRepository(GodsEyeDatabaseContext dbContext) : base(dbContext)
        {
        }
        protected override DbSet<User> GetDatabaseSet(GodsEyeDatabaseContext context)
        {
            return context.Users; ;
        }


        public async Task<User> FindUserByUsernameAndPasswordAsync(string username, string password)
        {
            //get all users
            var allUsers = await GetAllAsync();

            //return the first instance that has that values for username and pasword
            return allUsers
                .FirstOrDefault(x => x.Username == username && x.PasswordHash == password);
        }
    }
}
