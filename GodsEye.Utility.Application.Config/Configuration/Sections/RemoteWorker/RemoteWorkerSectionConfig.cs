using GodsEye.Utility.Application.Config.Configuration.Base;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class RemoteWorkerSectionConfig: IConfig
    {
        public WebSocketSectionConfig WebSocket { get; set; }
    }
}
