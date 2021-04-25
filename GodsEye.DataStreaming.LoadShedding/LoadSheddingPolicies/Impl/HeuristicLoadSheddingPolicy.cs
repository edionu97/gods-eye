using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public class HeuristicLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public Task<Queue<T>> ApplyPolicyAsync<T>(IList<T> data, int itemsToKeep)
        {
            throw new System.NotImplementedException();
        }
    }
}
