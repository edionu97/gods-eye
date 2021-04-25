using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.LoadShedding;

namespace GodsEye.DataStreaming.LoadShedding.Manager.Impl
{
    public class LoadSheddingFixedPolicyManager : ILoadSheddingFixedPolicyManager
    {
        private readonly ILoadSheddingPolicy _loadSheddingPolicy;
        private readonly ILogger<ILoadSheddingFixedPolicyManager> _logger;
        private readonly ILoadSheddingPolicy _policyUsedWhenNoLoadShedData;
        private readonly FacialAnalysisAndRecognitionWorkerConfig _facialAnalysisConfig;

        // ReSharper disable once SuggestBaseTypeForParameter
        public LoadSheddingFixedPolicyManager(
            IConfig config,
            ILogger<ILoadSheddingFixedPolicyManager> logger,
            ILoadSheddingPolicy loadSheddingPolicy,
            NoLoadSheddingPolicy policyUsedWhenNoLoadShedData)
        {
            //set the logger
            _logger = logger;

            //set the load shedding policy
            _loadSheddingPolicy = loadSheddingPolicy;

            //set the no load shedding policy
            _policyUsedWhenNoLoadShedData = policyUsedWhenNoLoadShedData;

            //set the facial analysis config
            _facialAnalysisConfig = config.Get<FacialAnalysisAndRecognitionWorkerConfig>();
        }

        public async Task<Queue<T>> SyncUsedFixedPolicyAsync<T>(
            IList<T> dataToBeProcessed, double avgProcessingRate, double avgInputRate)
        {
            //round the rates
            var inputRate = Math.Max(Math.Ceiling(avgInputRate), 1);
            var processingRate = Math.Max(Math.Ceiling(avgProcessingRate), 1);

            //get the number of tuples that need to be unloaded
            var systemOverloadRate = Math.Floor(inputRate) - Math.Floor(processingRate);

            //if the system is not overloaded => do not load shed data
            if (systemOverloadRate <= _facialAnalysisConfig.LoadSheddingThresholdValue)
            {
                return
                    await _policyUsedWhenNoLoadShedData
                        .ApplyPolicyAsync(dataToBeProcessed, (int)systemOverloadRate);
            }

            //log the messages
            using (_logger.BeginScope(string.Format(Constants.LoadSheddingShouldBePerformedMessage, inputRate, processingRate)))
            {
                _logger.LogWarning(JsonSerializerDeserializer<dynamic>.Serialize(new
                {
                    AppliedPolicy = _loadSheddingPolicy.GetType().Name,
                    CurrentDataSize = dataToBeProcessed.Count,
                    TuplesToBeRemoved = dataToBeProcessed.Count - (int)processingRate
                }));
            }

            //apply the policy based on instance
            return await _loadSheddingPolicy.ApplyPolicyAsync(dataToBeProcessed, (int)processingRate);
        }
    }
}
