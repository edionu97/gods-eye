using GodsEye.Utility.Application.Config.BaseConfig.Abstract;
using GodsEye.Utility.Application.Items.Enums;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.Camera
{
    public class NetworkSectionConfig : AbstractConfig
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
