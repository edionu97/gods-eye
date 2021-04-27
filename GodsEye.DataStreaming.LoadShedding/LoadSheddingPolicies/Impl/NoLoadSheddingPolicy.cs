using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public class NoLoadSheddingPolicy : INoLoadSheddingPolicy
    {
        public Task<Queue<(DateTime, NetworkImageFrameMessage)>> 
            ApplyPolicyAsync(IEnumerable<(DateTime, NetworkImageFrameMessage)> data, int itemsToKeep)
        {
            return Task.FromResult(new Queue<(DateTime, NetworkImageFrameMessage)>(data));
        }
    }
}
