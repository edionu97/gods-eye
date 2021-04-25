using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public class NoLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public Task<Queue<T>> ApplyPolicyAsync<T>(IList<T> data, int _)
        {
            return Task.FromResult(new Queue<T>(data));
        }
    }
}
