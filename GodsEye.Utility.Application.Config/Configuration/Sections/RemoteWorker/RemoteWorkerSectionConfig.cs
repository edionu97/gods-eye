using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class RemoteWorkerSectionConfig : AbstractConfig
    {
        public string WorkersAddress { get; set; }
        public int FrameBufferSize { get; set; }

        public void Deconstruct(out string workersAddress, out int maxBufferSize)
        {
            workersAddress = WorkersAddress;
            maxBufferSize = FrameBufferSize;
        }
    }
}
