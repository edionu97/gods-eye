using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.DataStreaming.LoadShedding.SheddingPolicies;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;

namespace GodsEye.DataStreaming.LoadShedding.Manager.Impl
{
    public class LoadSheddingFixedPolicyManager : ILoadSheddingFixedPolicyManager
    {
        private readonly ILoadSheddingPolicy _loadSheddingPolicy;
        private readonly FacialAnalysisAndRecognitionWorkerConfig _facialAnalysisConfig;

        public LoadSheddingFixedPolicyManager(IConfig config, ILoadSheddingPolicy loadSheddingPolicy)
        {
            //set the load shedding policy
            _loadSheddingPolicy = loadSheddingPolicy;

            //set the facial analysis config
            _facialAnalysisConfig = config.Get<FacialAnalysisAndRecognitionWorkerConfig>();
        }

        public async Task<IList<T>> SyncUsedFixedPolicyAsync<T>(
            IList<T> dataToBeProcessed, double avgProcessingRate, double avgIInputRate)
        {
            //round the rates
            var inputRate = Math.Max(Math.Ceiling(avgIInputRate), 1);
            var processingRate = Math.Max(Math.Ceiling(avgProcessingRate), 1);

            //check if the input rate and output rate are almost the same
            if (Math.Abs(inputRate - processingRate) <= _facialAnalysisConfig.LoadSheddingThresholdValue)
            {
                return dataToBeProcessed;
            }

            //apply the policy based on instance
            return await _loadSheddingPolicy.ApplyPolicyAsync(dataToBeProcessed, 10);
        }
    }
}
