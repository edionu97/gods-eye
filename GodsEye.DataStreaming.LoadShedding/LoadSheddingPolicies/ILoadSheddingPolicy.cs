using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies
{
    public interface ILoadSheddingPolicy
    {
        /// <summary>
        /// This method it is used for applying the selected load shedding policy
        /// </summary>
        /// <param name="data">the data to be load shed</param>
        /// <param name="itemsToKeep">the number of items that need to remain</param>
        /// <returns>a queue of items</returns>
        public Task<Queue<(DateTime, NetworkImageFrameMessage)>> 
            ApplyPolicyAsync(IEnumerable<(DateTime, NetworkImageFrameMessage)> data, int itemsToKeep);
    }
}
