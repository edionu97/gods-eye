using System.Threading.Tasks;
using GodsEye.RemoteWorker.Workers.Messages;

namespace GodsEye.Application.Middleware.MessageBroadcaster
{
    public interface IMessageBroadcasterMiddleware
    {
        /// <summary>
        /// This method it is responsible for broadcasting the message to all the clients
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task BroadcastMessageAsync(IRequestResponseMessage message);
    }
}
