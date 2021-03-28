using GodsEye.Utility.Enums;

namespace GodsEye.Utility.Configuration.Sections.Camera
{
    public class NetworkSection
    {
        public ImageType ImageStreamingFormat { get; set; }

        public  string StreamsOnPort { get; set; }

        public void Deconstruct(out ImageType type, out string port)
        {
            port = StreamsOnPort;
            type = ImageStreamingFormat;
        }
    }
}
