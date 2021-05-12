using System.Threading;
using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using static Gods.Eye.Server.Artificial.Intelligence.Messaging.FacialRecognitionAndAnalysis;

namespace GodsEye.RemoteWorkers.Interoperability.Grpc.Proxy.Impl
{
    public class FacialRecognitionAndAnalysisService : IFacialRecognitionAndAnalysisService
    {
        private readonly FacialRecognitionAndAnalysisClient _facialRecognitionAndAnalysisService;

        public FacialRecognitionAndAnalysisService(FacialRecognitionAndAnalysisClient facialRecognitionAndAnalysisService)
        {
            _facialRecognitionAndAnalysisService = facialRecognitionAndAnalysisService;
        }

        public async Task<SearchForPersonResponse> SearchPersonInImageAsync(
            string base64EncodedPersonImage, string base64EncodedImage, CancellationToken token)
        {
            //create the search request
            //setting IncludeCroppedFacesInResponse = false decreases the transport time
            var searchRequest = new SearchForPersonRequest
            {
                IncludeCroppedFacesInResponse = false,
                PersonImageB64 = base64EncodedPersonImage,
                LocationImageB64 = base64EncodedImage
            };

            //call the server method for handling the facial recognition task
            return await _facialRecognitionAndAnalysisService
                .DoFacialRecognitionAsync(searchRequest, cancellationToken: token);
        }

        public async Task<FacialAttributeAnalysisResponse> AnalyseFaceAsync(
            string base64FaceImage, FaceLocationBoundingBox faceBoundingBox, CancellationToken token)
        {

            //create the facial attribute request
            //setting IsFaceLocationKnown = true means that we know the location of the face
            var facialAttributeRequest = new FacialAttributeAnalysisRequest
            {
                AnalyzedImageContainingTheFaceB64 = base64FaceImage,
                IsFaceLocationKnown = true,
                FaceBoundingBox = faceBoundingBox
            };

            //call the server method for handling the facial recognition task
            return await _facialRecognitionAndAnalysisService
                .DoFacialAttributeAnalysisAsync(facialAttributeRequest, cancellationToken: token);
        }
    }
}
