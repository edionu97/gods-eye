using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Config.Configuration.Base;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.Camera
{
    public class NetworkSectionConfig : IConfig
    {
        public ImageType ImageStreamingFormat { get; set; }
        public int StreamsOnPort { get; set; }
        public string CamerasLocation { get; set; }

        public void Deconstruct(
            out ImageType type, 
            out int port,
            out string camerasLocation)
        {
            port = StreamsOnPort;
            type = ImageStreamingFormat;
            camerasLocation = CamerasLocation;
        }
    }
}
