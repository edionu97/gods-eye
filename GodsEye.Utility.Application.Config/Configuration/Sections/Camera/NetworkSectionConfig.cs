using GodsEye.Utility.Application.Config.BaseConfig.Abstract;
using GodsEye.Utility.Application.Items.Enums;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.Camera
{
    public class NetworkSectionConfig : AbstractConfig
    {
        public ImageType ImageStreamingFormat { get; set; }
        public string CamerasLocation { get; set; }
        public bool SendTheGeolocation { get; set; }

        public void Deconstruct(
            out ImageType type, 
            out string camerasLocation,
            out bool sendTheGeolocation)
        {
            type = ImageStreamingFormat;
            camerasLocation = CamerasLocation;
            sendTheGeolocation = SendTheGeolocation;
        }
    }
}
