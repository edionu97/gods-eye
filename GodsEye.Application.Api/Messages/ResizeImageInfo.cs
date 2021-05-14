namespace GodsEye.Application.Api.Messages
{
    public class ResizeImageInfo
    {
        public string ImageBase64 { get; set; }
        public int ToWidth { get; set; }
        public int ToHeight { get; set; }
    }
}
