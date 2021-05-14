using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using ImageMagick;

namespace GodsEye.Application.Api.Messages
{
    public class FacialExtractionInfo
    {
        public FaceLocationBoundingBox BoundingBox { get; set; }

        public int? ToWidth { get; set; }
        public int? ToHeight { get; set; }
        public string ImageBase64 { get; set; }
    }
}
