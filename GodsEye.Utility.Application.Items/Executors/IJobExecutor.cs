using System;
using System.Threading.Tasks;

namespace GodsEye.Utility.Application.Items.Executors
{
    public interface IJobExecutor
    {
        /// <summary>
        /// This method it is used for adding a new job to be executed
        /// </summary>
        /// <param name="jobToExecute">the job that needs to be executed</param>
        public void QueueJob(Func<Task> jobToExecute);

        /// <summary>
        /// Stop the job execution
        /// </summary>
        public void StopJobExecution();
    }
}
