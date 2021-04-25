using System.Threading.Tasks;
using System.Collections.Generic;

namespace GodsEye.DataStreaming.LoadShedding.SheddingPolicies.Impl
{
    public class HeuristicLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public Task<IList<T>> ApplyPolicyAsync<T>(IList<T> data, int itemsToRemove)
        {
            throw new System.NotImplementedException();
        }
    }
}
