using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies
{
    public interface ILoadSheddingPolicy
    {
        public Task<Queue<T>> ApplyPolicyAsync<T>(IList<T> data, int itemsToKeep);
    }
}
