using System;
using System.Threading.Tasks;
using GodsEye.Application.Persistence.Models;
using GodsEye.Application.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.Application.Api.Controllers.Users
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("Username or password is null");
            }

            try
            {
                //get the checksum
                var userChecksum = await _userService
                    .LoginAsync(user.Username, user.PasswordHash);

                //return the response
                return Ok(new
                {
                    UserToken = userChecksum
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateAccountAsync([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("Username or password is null");
            }

            try
            {
                //get the checksum
                var userChecksum = await _userService
                    .CreateAccountAsync(user.Username, user.PasswordHash);

                //return the response
                return Ok(new
                {
                    UserToken = userChecksum
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
