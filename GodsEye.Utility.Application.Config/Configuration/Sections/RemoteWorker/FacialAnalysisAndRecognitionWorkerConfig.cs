using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class FacialAnalysisAndRecognitionWorkerConfig : AbstractConfig
    {
        public bool StartWorkerOnlyWhenBufferIsFull { get; set; }

        public LoadSheddingPolicyType LoadSheddingPolicy { get; set; }

        public void Deconstruct(
            out bool startWorkerOnlyWhenBufferIsFull,
            out LoadSheddingPolicyType loadSheddingPolicy)
        {
            startWorkerOnlyWhenBufferIsFull = StartWorkerOnlyWhenBufferIsFull;
            loadSheddingPolicy = LoadSheddingPolicy;
        }
    }
}
