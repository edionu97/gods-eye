using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GodsEye.Utility.Application.Items.Executors.Impl
{
    public class SingleThreadBlockingJobExecutor : IJobExecutor
    {
        private readonly BlockingCollection<Func<Task>> _jobsToBeExecuted = new BlockingCollection<Func<Task>>();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Task JobExecutor { get; }

        public SingleThreadBlockingJobExecutor()
        {
            //get a new cancellation token
            var token = _cancellationTokenSource.Token;

            //create
            JobExecutor = Task.Run(async () =>
            {
                //add the job execution
                while (!token.IsCancellationRequested && !_jobsToBeExecuted.IsAddingCompleted)
                {
                    try
                    {
                        //wait until the 
                        var jobToExecute = _jobsToBeExecuted.Take(token);

                        //try to execute the job
                        try
                        {
                            await jobToExecute();
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

            }, _cancellationTokenSource.Token);
        }

        public void QueueJob(Func<Task> jobToExecute)
        {
            //add the job in collection
            _jobsToBeExecuted.Add(jobToExecute);
        }

        public void StopJobExecution()
        {
            _cancellationTokenSource.Cancel();
            _jobsToBeExecuted.CompleteAdding();
        }
    }
}
