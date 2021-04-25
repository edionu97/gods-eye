using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;

namespace GodsEye.DataStreaming.LoadShedding.Manager.Impl
{
    public class LoadSheddingFixedPolicyManager : ILoadSheddingFixedPolicyManager
    {
        private readonly ILoadSheddingPolicy _loadSheddingPolicy;
        private readonly ILoadSheddingPolicy _policyUsedWhenNoLoadShedData;
        private readonly FacialAnalysisAndRecognitionWorkerConfig _facialAnalysisConfig;

        // ReSharper disable once SuggestBaseTypeForParameter
        public LoadSheddingFixedPolicyManager(
            IConfig config,
            ILoadSheddingPolicy loadSheddingPolicy, 
            NoLoadSheddingPolicy policyUsedWhenNoLoadShedData)
        {

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
            var tuplesToUnload = Math.Floor(inputRate) - Math.Floor(processingRate);

            //if the system is not overloaded => do not load shed data
            if (tuplesToUnload <= _facialAnalysisConfig.LoadSheddingThresholdValue)
            {
                return 
                    await _policyUsedWhenNoLoadShedData
                        .ApplyPolicyAsync(dataToBeProcessed, (int) tuplesToUnload);
            }

            //apply the policy based on instance
            return await _loadSheddingPolicy.ApplyPolicyAsync(dataToBeProcessed, (int) tuplesToUnload);
        }
    }
}
