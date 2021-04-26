using System;
using System.Threading;
using System.Threading.Tasks;

namespace GodsEye.Utility.Application.Helpers.Helpers.Threading
{
    public static class WaitHelpers
    {
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The condition that will perpetuate the block.</param>
        /// <param name="cancellationToken">the cancellation token</param>
        /// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException">if the timeout is encountered</exception>
        /// <returns></returns>
        public static async Task WaitWhileAsync(
            Func<bool> condition, 
            CancellationToken cancellationToken, int frequency = 25, int timeout = -1)
        {
            //create the waiting task
            var waitTask = Task.Run(async () =>
            {
                while (condition())
                {
                    await Task.Delay(frequency, cancellationToken);
                }

            }, cancellationToken);

            //if the wait task finishes first then do noting
            if (waitTask == await Task.WhenAny(waitTask, Task.Delay(timeout, cancellationToken)))
            {
                return;
            }

            //return the timeout extension
            throw new TimeoutException();
        }
    }
}
