using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public class HeuristicLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public Task<Queue<(DateTime, NetworkImageFrameMessage)>> ApplyPolicyAsync(IList<(DateTime, NetworkImageFrameMessage)> data, int itemsToKeep)
        {
            throw new System.NotImplementedException();
        }
    }
}
