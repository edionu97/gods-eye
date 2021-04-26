using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

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
        /// <param name="remainingTuplesToProcess">data to be processes</param>
        /// <param name="availableTimeToProcessData">the time in which the data should be processed</param>
        /// <param name="lastKnownTupleProcessingRate">the last known processing rate of a tuple from the data</param>
        /// <returns>
        ///     the new data such as avgProcessingRate is almost the same as avgProcessingRate or
        ///     the same data either if the NoLoadShedding policy is used
        ///     either if the avgProcessingRate == avgInputRate</returns>
        public Task<Queue<(DateTime, NetworkImageFrameMessage)>> 
            ApplyLoadSheddingPolicyAsync(
                Queue<(DateTime, NetworkImageFrameMessage)> remainingTuplesToProcess,
                double availableTimeToProcessData,
                double lastKnownTupleProcessingRate);
    }
}
