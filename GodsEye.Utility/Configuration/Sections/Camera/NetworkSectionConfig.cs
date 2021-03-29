using GodsEye.Utility.Configuration.Base;
using GodsEye.Utility.Enums;

namespace GodsEye.Utility.Configuration.Sections.Camera
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
