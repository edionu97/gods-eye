using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.SheddingPolicies.Impl
{
    public class NoLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public Task<IList<T>> ApplyPolicyAsync<T>(IList<T> data, int _)
        {
            return Task.FromResult(data);
        }
    }
}
