using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.SheddingPolicies.Impl
{
    public class RandomLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public Task<IList<T>> ApplyPolicyAsync<T>(IList<T> data, int itemsToRemove)
        {
            throw new System.NotImplementedException();
        }
    }
}
