using System;
using System.Threading.Tasks;

namespace GodsEye.Utility.Application.Helpers.Helpers.Threading
{
    public static class WaitHelpers
    {
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The condition that will perpetuate the block.</param>
        /// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException">if the timeout is encountered</exception>
        /// <returns></returns>
        public static async Task WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            //create the waiting task
            var waitTask = Task.Run(async () =>
            {
                while (condition())
                {
                    await Task.Delay(frequency);
                }
            });

            //if the wait task finishes first then do noting
            if (waitTask == await Task.WhenAny(waitTask, Task.Delay(timeout)))
            {
                return;
            }

            //return the timeout extension
            throw new TimeoutException();
        }
    }
}
