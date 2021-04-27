using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public partial class HeuristicLoadSheddingPolicy : ILoadSheddingPolicy
    {
        private readonly HeuristicLoadSheddingConfig _heuristicConfig;

        public HeuristicLoadSheddingPolicy(IConfig config)
        {
            _heuristicConfig = config.Get<HeuristicLoadSheddingConfig>();

            //if the heuristic config is null, use the default config
            _heuristicConfig ??= new HeuristicLoadSheddingConfig
            {
                ResizeImageToHeight = 50,
                ResizeImageToWidth = 50
            };
        }

        public Task<Queue<(DateTime, NetworkImageFrameMessage)>>
            ApplyPolicyAsync(IEnumerable<(DateTime, NetworkImageFrameMessage)> dataToLoadShed, int itemsToKeep)
        {
            //convert the enumerable into an list
            var data = dataToLoadShed.ToList();

            //read the heuristic config 
            var (w, h) = _heuristicConfig;

            //resize all the videoFrameImages to a specific size
            //speed up the image comparision
            var images = ResizeAllTheB64ImagesToFixedSize(data.ToList(), (w, h));

            //as long as we need to remove data
            var indexesToRemove = new HashSet<int>();
            while (images.Any() && indexesToRemove.Count < data.Count - itemsToKeep)
            {
                //compute the index of data that needs to be removed
                var index = FindMostSimilarTwoImagesRelativeToListBorders(images);

                //mark the position as removable
                indexesToRemove.Add(index);

                //remove the image
                images.RemoveAt(index);
            }

            //add the items that remain back in the queue
            var queue = new Queue<(DateTime, NetworkImageFrameMessage)>();
            for (var index = 0; index < data.Count; ++index)
            {
                //if the index is back listed then continue
                if (indexesToRemove.Contains(index))
                {
                    continue;
                }

                //add the item in queue
                queue.Enqueue(data[index]);
            }

            //return the queue
            return Task.FromResult(queue);
        }
    }
}
