using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GodsEye.Application.Api.Messages;
using GodsEye.Application.Middleware.WorkersMaster;

namespace GodsEye.Application.Api.Controllers.Recognition
{
    [ApiController]
    [Route("api/facial-recognition")]
    public class FacialRecognitionController : ControllerBase
    {
        private readonly IWorkersMasterMiddleware _workersMasterMiddleware;

        public FacialRecognitionController(IWorkersMasterMiddleware workersMasterMiddleware)
        {
            _workersMasterMiddleware = workersMasterMiddleware;
        }

        /// <summary>
        /// This method does nothing, it should be used to initialize all the dependencies
        /// </summary>
        [HttpGet("initialize")]
        public Task<IActionResult> WakeUp()
        {
            return Task.FromResult<IActionResult>(Ok());
        }

        /// <summary>
        /// This method it is used for getting a search token 
        /// </summary>
        /// <param name="userSecrets">the user secrets (userId, searchedPersonImg)</param>
        /// <returns>a object which contains the action result</returns>
        [HttpPost("token/for")]
        public Task<IActionResult> GetSearchToken([FromBody] UserSecrets userSecrets)
        {
            //handle the null user id 
            if (userSecrets?.UserId == null)
            {
                return Task.FromResult<IActionResult>(Problem());
            }

            //destruct the object
            var (userId, searchedPerson) = userSecrets;

            //create the search token
            return Task.FromResult<IActionResult>(Ok(new
            {
                SearchToken = _workersMasterMiddleware
                    .GetChecksumValue(userId, searchedPerson)
            }));
        }

        /// <summary>
        /// This method it is used for starting the searching
        /// </summary>
        /// <param name="userSecrets">the user secrets (userId, searchedPersonImg)</param>
        [HttpPost("searching/start")]
        public async Task<IActionResult> StartSearchingAsync([FromBody] UserSecrets userSecrets)
        {
            //handle the null user id 
            if (userSecrets?.UserId == null || userSecrets.SearchedPerson == null)
            {
                return BadRequest("User secrets instance should not contain null values");
            }

            //try to execute the request
            try
            {
                //destruct the object
                var (userId, searchedPerson) = userSecrets;

                //initialize the start searching
                await _workersMasterMiddleware.StartSearchingAsync(userId, searchedPerson);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            //return the response
            return Ok(new
            {
                Message = "Start searching message was send successfully"
            });
        }

        /// <summary>
        /// This method it is used for starting the searching
        /// </summary>
        /// <param name="userSecrets">the user secrets (userId, searchedPersonImg)</param>
        [HttpPost("searching/stop")]
        public async Task<IActionResult> StopSearchingAsync([FromBody] UserSecrets userSecrets)
        {
            //handle the null user id 
            if (userSecrets?.UserId == null || userSecrets.SearchedPerson == null)
            {
                return BadRequest("User secrets instance should not contain null values");
            }

            //try to execute the request
            try
            {
                //destruct the object
                var (userId, searchedPerson) = userSecrets;

                //initialize the start searching
                await _workersMasterMiddleware.StopSearchingAsync(userId, searchedPerson);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            //return the response
            return Ok(new
            {
                Message = "Stopped searching message was send successfully"
            });
        }

        /// <summary>
        /// This method it is used for starting the searching
        /// </summary>
        /// <param name="userSecrets">the user secrets (userId, searchedPersonImg)</param>
        [HttpPost("searching/active-workers/all")]
        public async Task<IActionResult> GetAllActiveWorkersAsync([FromBody] UserSecrets userSecrets)
        {
            //handle the null user id 
            if (userSecrets?.UserId == null)
            {
                return BadRequest("User secrets instance should not contain null values");
            }

            //try to execute the request
            try
            {
                //destruct the object
                var (userId, _) = userSecrets;

                //initialize the start searching
                await _workersMasterMiddleware.PingWorkersAsync(userId);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            //return the response
            return Ok(new
            {
                Message = "Message for discovering all workers was send successfully"
            });
        }

    }
}
