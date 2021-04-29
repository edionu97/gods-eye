using GodsEye.Utility.Application.Config.BaseConfig.Abstract;
using GodsEye.Utility.Application.Items.Enums;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class FrameBufferConfig : AbstractConfig
    {
        public int BufferSize { get; set; }

        public BufferBehaviourType BufferBehaviour { get; set; }

        public void Deconstruct(out int bufferSize, out BufferBehaviourType bufferBehaviour)
        {
            bufferSize = BufferSize;
            bufferBehaviour = BufferBehaviour;
        }
    }
}
