using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodsEye.DataStreaming.LoadShedding.SheddingPolicies
{
    public interface ILoadSheddingPolicy
    {
        public Task<IList<T>> ApplyPolicyAsync<T>(IList<T> data, int itemsToRemove);
    }
}
