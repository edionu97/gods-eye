using System;
using System.Threading.Tasks;
using GodsEye.Application.Persistence.Models;
using GodsEye.Application.Persistence.Repository;
using GodsEye.Utility.Application.Helpers.Helpers.Hashing;
using Microsoft.Data.SqlClient;
using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Services;

namespace GodsEye.Application.Services.UserService.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            //get the password hash
            var passwordHash = StringContentHasherHelpers
                .GetChecksumOfStringContent(password);

            //get the user
            var user =
                await _userRepository
                    .FindUserByUsernameAndPasswordAsync(username, passwordHash)
                ?? throw new Exception(Constants.UserNotFoundMessage);

            //return the checksum
            return StringContentHasherHelpers
                .GetChecksumOfStringContent($"{user.UserId}:{passwordHash}");
        }

        public async Task<string> CreateAccountAsync(string username, string password)
        {
            try
            {

                //get the password hash
                var passwordHash = StringContentHasherHelpers
                    .GetChecksumOfStringContent(password);

                //create the user
                var user = new User
                {
                    Username = username,
                    PasswordHash = passwordHash
                };

                //add the user in database
                await _userRepository.AddAsync(user);

                //return the checksum
                return StringContentHasherHelpers
                    .GetChecksumOfStringContent($"{user.UserId}:{passwordHash}");
            }
            catch (Exception e)
            {
                //if the inner exception is not sql exception
                if (!(e.InnerException is SqlException sqlException))
                {
                    throw;
                }

                //check the if nature of exception
                if (sqlException.Message.Contains("duplicate"))
                {
                    throw new Exception(Constants.UserFoundMessage);
                }
                throw new Exception(sqlException.Message);
            }
        }
    }
}
