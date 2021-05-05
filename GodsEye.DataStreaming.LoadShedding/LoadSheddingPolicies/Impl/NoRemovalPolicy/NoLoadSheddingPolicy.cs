using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Args;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl.NoRemovalPolicy
{
    public class NoLoadSheddingPolicy : INoLoadSheddingPolicy
    {
        public Task<Queue<(DateTime, NetworkImageFrameMessage)>>
            ApplyPolicyAsync(
                IEnumerable<(DateTime, NetworkImageFrameMessage)> data,
                int _,
                LoadSheddingPolicyArgs __)
        {
            return Task.FromResult(new Queue<(DateTime, NetworkImageFrameMessage)>(data));
        }
    }
}
