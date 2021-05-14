using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Application.Services.ImageManipulator.Helpers;

namespace GodsEye.Application.Api.Messages
{
    public class FacialIdentificationInfo
    {
        public FaceLocationBoundingBox BoundingBox { get; set; }

        public DrawingOptions DrawingOptions { get; set; }

        public FaceKeypointsLocation FaceKeypointsLocation { get; set; }

        public string ImageBase64 { get; set; }
    }
}
