using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public class NoLoadSheddingPolicy : INoLoadSheddingPolicy
    {
        public Task<Queue<T>> ApplyPolicyAsync<T>(IList<T> data, int itemsToKeep)
        {
            return Task.FromResult(new Queue<T>(data));
        }
    }
}
