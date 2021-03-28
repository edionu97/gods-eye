namespace GodsEye.Utility.Configuration.Sections.Camera
{
    public class CameraSection
    {
        public string CameraId { get; set; }

        public ImageOptionsSection ImageOptions { get; set; }

        public NetworkSection Network { get; set; }

        public void Deconstruct(
            out string cameraId,
            out ImageOptionsSection imageOptions,
            out NetworkSection network)

        {
            cameraId = CameraId;
            imageOptions = ImageOptions;
            network = Network;
        }
    }
}
