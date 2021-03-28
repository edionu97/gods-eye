namespace GodsEye.Utility.Configuration.Sections.Camera
{
    public class CameraSection
    {
        public string CameraId { get; set; }

        public  ImageOptionsSection ImageOptions { get; set; }

        public void Deconstruct(
            out string cameraId, out ImageOptionsSection imageOptions)
        {
            cameraId = CameraId;
            imageOptions = ImageOptions;
        }
    }
}
