using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public class HeuristicLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public Task<Queue<T>> ApplyPolicyAsync<T>(IList<T> data, int itemsToRemove)
        {
            throw new System.NotImplementedException();
        }
    }
}
