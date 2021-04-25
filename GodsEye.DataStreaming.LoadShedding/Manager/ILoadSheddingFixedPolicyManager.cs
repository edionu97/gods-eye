using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.Manager
{
    public interface ILoadSheddingFixedPolicyManager
    {
        /// <summary>
        /// This function it is used for data load shedding. It uses one the following load shedding policies
        ///     NoLoadShedding => no shedding will be applied over data
        ///     RandomLoadShedding => it will use the random policy in order to remove tuples from data
        ///     HeuristicLoadShedding => it will use the heuristic policy in order to remove tuples
        /// </summary>
        /// <param name="dataToBeProcessed">data to be processes</param>
        /// <param name="avgProcessingRate">the last known processing rate</param>
        /// <param name="avgIInputRate">the current input rate</param>
        /// <returns>
        ///     the new data such as avgProcessingRate is almost the same as avgProcessingRate or
        ///     the same data either if the NoLoadShedding policy is used
        ///     either if the avgProcessingRate == avgIInputRate</returns>
        public Task<Queue<T>> 
            SyncUsedFixedPolicyAsync<T>(
                IList<T> dataToBeProcessed, double avgProcessingRate, double avgIInputRate);
    }
}
