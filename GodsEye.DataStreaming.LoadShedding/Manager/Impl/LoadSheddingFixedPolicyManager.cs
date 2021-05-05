using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Args;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.LoadShedding;

namespace GodsEye.DataStreaming.LoadShedding.Manager.Impl
{
    public class LoadSheddingFixedPolicyManager : ILoadSheddingFixedPolicyManager
    {
        private readonly ILoadSheddingPolicy _loadSheddingPolicy;
        private readonly ILogger<ILoadSheddingFixedPolicyManager> _logger;
        private readonly INoLoadSheddingPolicy _noLoadSheddingPolicy;

        public LoadSheddingPolicyArgs PolicyArgs { get; set; }

        public LoadSheddingFixedPolicyManager(
            ILogger<ILoadSheddingFixedPolicyManager> logger,
            ILoadSheddingPolicy loadSheddingPolicy,
            INoLoadSheddingPolicy noLoadSheddingPolicy)
        {
            //set the logger
            _logger = logger;

            //set the load shedding policy
            _loadSheddingPolicy = loadSheddingPolicy;

            //set the no load shedding policy
            _noLoadSheddingPolicy = noLoadSheddingPolicy;
        }

        public async Task<Queue<(DateTime, NetworkImageFrameMessage)>>
            ApplyLoadSheddingPolicyAsync(
                Queue<(DateTime, NetworkImageFrameMessage)> remainingTuplesToProcess,
                double availableTimeToProcessData, double lastKnownTupleProcessingRate)
        {
            //round the available time to process the data
            availableTimeToProcessData = Math
                .Max(Math.Ceiling(availableTimeToProcessData), 1);

            //round the last known tuple processing rate
            lastKnownTupleProcessingRate = Math
                .Max(Math.Ceiling(lastKnownTupleProcessingRate), 1);

            //remaining frames if the ls will be applied
            var remainingTuples = (int)Math
                .Floor(availableTimeToProcessData / lastKnownTupleProcessingRate);

            //if the load shedding is not needed then apply the NoLs policy
            if (remainingTuples >= remainingTuplesToProcess.Count)
            {
                //log the no ls message
                LogTheNoLsMessage(
                    remainingTuplesToProcess.Count,
                    lastKnownTupleProcessingRate, availableTimeToProcessData);

                //apply the no ls policy
                return await _noLoadSheddingPolicy
                    .ApplyPolicyAsync(remainingTuplesToProcess, remainingTuples, PolicyArgs);
            }

            //log the message
            LogTheLsMessage(
                remainingTuplesToProcess.Count,
                lastKnownTupleProcessingRate,
                availableTimeToProcessData, remainingTuples);

            //apply the ls policy
            return await _loadSheddingPolicy
                .ApplyPolicyAsync(remainingTuplesToProcess, remainingTuples, PolicyArgs);
        }

        private void LogTheNoLsMessage(int dataSize, double lastKnownTupleProcessingRate, double availableTimeToProcessData)
        {
            //log the message
            using (_logger.BeginScope(Constants.NoLoadSheddingRequiredMessage))
            {
                _logger.LogDebug(JsonSerializerDeserializer<dynamic>.Serialize(new
                {
                    AppliedPolicy = _noLoadSheddingPolicy.GetType().Name,
                    ActualDataSize = dataSize,
                    ProcessingStatistics = new
                    {
                        TupleProcessing = string
                            .Format(Constants.TupleRequiresXSecondsToBeProcessedMessage, lastKnownTupleProcessingRate),
                        RemaingTimeToProcessAllData = string
                            .Format(Constants.TheAvailableTimeForDataProcessingMessage, availableTimeToProcessData),
                    }
                }) + "\n");
            }
        }

        private void LogTheLsMessage(
            int dataSize,
            double lastKnownTupleProcessingRate,
            double availableTimeToProcessData, int remainingTuples)
        {
            //log the message
            using (_logger.BeginScope(Constants.LoadSheddingShouldBePerformedMessage))
            {
                _logger.LogWarning(JsonSerializerDeserializer<dynamic>.Serialize(new
                {
                    AppliedPolicy = _loadSheddingPolicy.GetType().Name,
                    ActualDataSize = dataSize,
                    ProcessingStatistics = new
                    {
                        TupleProcessing = string
                            .Format(Constants.TupleRequiresXSecondsToBeProcessedMessage, lastKnownTupleProcessingRate),
                        RemaingTimeToProcessAllData = string
                            .Format(Constants.TheAvailableTimeForDataProcessingMessage, availableTimeToProcessData),
                        NewDataSize = remainingTuples,
                    }
                }) + "\n");
            }
        }
    }
}
