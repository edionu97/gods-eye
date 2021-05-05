using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Args;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl.Abstract;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.LoadShedding;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl.EdgeRemovalPolicy
{
    public partial class HeuristicImageSimilarityLoadSheddingPolicy : AbstractTwoPhaseLoadSheddingPolicy
    {
        protected readonly ILogger<HeuristicImageSimilarityLoadSheddingPolicy> Logger;
        protected readonly HeuristicLoadSheddingConfig HeuristicConfig;

        public HeuristicImageSimilarityLoadSheddingPolicy(IConfig config, ILogger<HeuristicImageSimilarityLoadSheddingPolicy> logger)
        {
            HeuristicConfig = config.Get<HeuristicLoadSheddingConfig>();

            //if the heuristic config is null, use the default config
            HeuristicConfig ??= new HeuristicLoadSheddingConfig
            {
                ResizeImageToHeight = 50,
                ResizeImageToWidth = 50
            };

            //set the logger
            Logger = logger;
        }

        public override Task<Queue<(DateTime, NetworkImageFrameMessage)>>
            ApplyPolicyAsync(
                IEnumerable<(DateTime, NetworkImageFrameMessage)> dataToLoadShed, 
                int itemsToKeep,
                LoadSheddingPolicyArgs args = null)
        {
            //convert the enumerable into an list
            var data = dataToLoadShed.ToList();

            //get the number of items that need to be removed
            var numberOfItemsToRemove = data.Count - itemsToKeep;

            //read the heuristic config 
            var (w, h) = HeuristicConfig;

            //resize all the videoFrameImages to a specific size
            var resizedImages =
                ResizeAllTheB64ImagesToFixedSize(data.ToList(), (w, h)).ToList();

            //begin scope for messaging logging
            using (Logger.BeginScope(string
                .Format(Constants.HeuristicLoadSheddingScopeMessage, data.Count, itemsToKeep)))
            {
                //get the distinct frames (image and it's index in original frame buffer)
                var framesAfterFirstRound =
                    ApplyTheBulkRemovalOfDuplicateFramesPhase(resizedImages, numberOfItemsToRemove).ToList();

                //log the message after the first round
                Logger.LogDebug(string
                    .Format(
                        Constants.HeuristicLoadSheddingFirstRoundFinishedMessage,
                        framesAfterFirstRound.Count,
                        framesAfterFirstRound.Count != itemsToKeep));

                //apply the second round and remove from the frames that are no such similar
                var framesAfterSecondRound =
                    ApplyTheRemovalOfRemainingFramesPhase(
                        framesAfterFirstRound,
                        framesAfterFirstRound.Count - itemsToKeep, args);

                //add the items that remain back in the queue
                //those items will be processed
                var queue = new Queue<(DateTime, NetworkImageFrameMessage)>();
                foreach (var (_, dataIndex) in framesAfterSecondRound)
                {
                    queue.Enqueue(data[dataIndex]);
                }

                //log the message
                Logger.LogDebug(string.Format(Constants
                    .HeuristicLoadSheddingDataSizeMessage, queue.Count));

                //return the queue
                return Task.FromResult(new Queue<(DateTime, NetworkImageFrameMessage)>(queue));
            }
        }
    }
}
