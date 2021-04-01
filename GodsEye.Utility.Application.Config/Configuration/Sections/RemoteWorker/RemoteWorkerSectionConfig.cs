using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class RemoteWorkerSectionConfig : AbstractConfig
    {
        public WebSocketSectionConfig WebSocket { get; set; }
    }
}
