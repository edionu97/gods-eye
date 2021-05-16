using System;
using System.Threading.Tasks;

namespace GodsEye.Application.Services.UserService
{
    public interface IUserService
    {
        /// <summary>
        /// This method it is used for logging in a user
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
        /// <returns>the userSecretFootprint</returns>
        /// <exception cref="Exception">if the user could not authenticate</exception>
        public Task<string> LoginAsync(string username, string password);

        /// <summary>
        /// Creates a new account 
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
        /// <returns>the userSecretFootprint</returns>
        /// <exception cref="Exception">if the user could not be created (it already exists in database)</exception>
        public Task<string> CreateAccountAsync(string username, string password);
    }
}
