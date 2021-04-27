using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class HeuristicLoadSheddingConfig : AbstractConfig
    {
        public int ResizeImageToWidth { get; set; }
        public int ResizeImageToHeight { get; set; }

        public void Deconstruct(out int resizeImageToWidth, out int resizeImageToHeight)
        {
            resizeImageToWidth = ResizeImageToWidth;
            resizeImageToHeight = ResizeImageToHeight;
        }
    }

    public class FacialAnalysisAndRecognitionWorkerConfig : AbstractConfig
    {
        public bool StartWorkerOnlyWhenBufferIsFull { get; set; }

        public LoadSheddingPolicyType LoadSheddingPolicy { get; set; }

        public HeuristicLoadSheddingConfig HeuristicLoadShedding { get; set; }

        public void Deconstruct(
            out bool startWorkerOnlyWhenBufferIsFull,
            out LoadSheddingPolicyType loadSheddingPolicy)
        {
            startWorkerOnlyWhenBufferIsFull = StartWorkerOnlyWhenBufferIsFull;
            loadSheddingPolicy = LoadSheddingPolicy;
        }
    }
}
