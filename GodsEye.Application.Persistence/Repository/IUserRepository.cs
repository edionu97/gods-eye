using System.Threading.Tasks;
using GodsEye.Application.Persistence.Models;

namespace GodsEye.Application.Persistence.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Finds an user by its username and password
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
        /// <returns>the user if in database exist such instance or null otherwise</returns>
        /// 
        public Task<User> FindUserByUsernameAndPasswordAsync(string username, string password);
    }
}
