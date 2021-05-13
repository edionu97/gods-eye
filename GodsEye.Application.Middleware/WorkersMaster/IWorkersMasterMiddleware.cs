using System;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Workers.Messages;

namespace GodsEye.Application.Middleware.WorkersMaster
{
    public interface IWorkersMasterMiddleware
    {
        public Func<IRequestResponseMessage, Task> OnMessageCallback { get; }

        /// <summary>
        /// This method it is used for sending a request to all available workers
        /// They will respond and will send a message
        /// <param name="userId">the id of the user for which the request is send</param>
        /// </summary>
        public Task PingWorkersAsync(string userId);

        /// <summary>
        /// This method will send a search request through all the active workers
        /// </summary>
        /// <param name="userId">the id of the user for which the request is send</param>
        /// <param name="searchedPerson">the searched person image in base64 format</param>
        public Task StartSearchingAsync(string userId, string searchedPerson);

        /// <summary>
        /// This method will send a stop searching request for a specific user
        /// </summary>
        /// <param name="userId">the id of the user for which the request is send</param>
        /// <param name="searchedPerson">the searched person image in base64 format</param>
        public Task StopSearchingAsync(string userId, string searchedPerson);

        /// <summary>
        /// This function 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchedPerson"></param>
        /// <returns></returns>
        public string GetChecksumValue(string userId, string searchedPerson);

        /// <summary>
        /// Sets the message callback, that will be called every time we have a message
        /// </summary>
        /// <param name="callback">the callback that will be set</param>
        public Task SetTheMessageCallbackAsync(Func<IRequestResponseMessage, Task> callback);
    }
}
