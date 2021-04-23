using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;

namespace GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy
{
    public interface IFacialRecognitionAndAnalysisProxy
    {
        public Task<SearchForPersonResponse> IdentifyPersonAsync(
            string imageFrameBase64, string searchedPersonBase64);

        public Task<SearchForPersonResponse> IdentifyPersonAndIncludeCroppedFacesAsync(
            string imageFrameBase64, string searchedPersonBase64);

        public Task<FacialAttributeAnalysisResponse> ExtractFacialAttributesWhenFaceLocationIsUnknownAsync(
            string imageWithPersonBase64);

        public Task<FacialAttributeAnalysisResponse> ExtractFacialAttributesWhenFaceLocationIsKnownAsync(
            string imageWithPersonBase64, FaceLocationBoundingBox faceLocation);
    }
}
