using Grpc.Core;
using System.IO;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Config.BaseConfig;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using static Gods.Eye.Server.Artificial.Intelligence.Messaging.FacialRecognitionAndAnalysis;

namespace GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy.Impl
{
    public class FacialRecognitionAndAnalysisProxy : IFacialRecognitionAndAnalysisProxy
    {
        private readonly FacialRecognitionAndAnalysisClient _facialRecognitionAndAnalysisService;

        public FacialRecognitionAndAnalysisProxy(IConfig configuration)
        {
            //destruct the configuration
            var (certificateLocation,
                 host,
                 port) = configuration.Get<GrpcFacialAnalysisServerConfig>();

            //create the secure channel
            var channel = new Channel(
                host,
                port,
                new SslCredentials(File.ReadAllText(certificateLocation)));
            
            //connect to the server
            _facialRecognitionAndAnalysisService = new FacialRecognitionAndAnalysisClient(channel);
        }

        public async Task<SearchForPersonResponse> IdentifyPersonAsync(string imageFrameBase64, string searchedPersonBase64)
        {
            //call the server method for handling the facial recognition task
            return await _facialRecognitionAndAnalysisService.DoFacialRecognitionAsync(new SearchForPersonRequest
            {
                IncludeCroppedFacesInResponse = false,
                PersonImageB64 = searchedPersonBase64,
                LocationImageB64 = imageFrameBase64
            });
        }

        public async Task<SearchForPersonResponse> IdentifyPersonAndIncludeCroppedFacesAsync(string imageFrameBase64, string searchedPersonBase64)
        {
            //call the server method for handling the facial recognition task
            return await _facialRecognitionAndAnalysisService.DoFacialRecognitionAsync(new SearchForPersonRequest
            {
                IncludeCroppedFacesInResponse = true,
                PersonImageB64 = searchedPersonBase64,
                LocationImageB64 = imageFrameBase64
            });
        }

        public async Task<FacialAttributeAnalysisResponse> ExtractFacialAttributesWhenFaceLocationIsUnknownAsync(string imageWithPersonBase64)
        {
            //call the server method for handling the facial recognition task
            return await _facialRecognitionAndAnalysisService.DoFacialAttributeAnalysisAsync(new FacialAttributeAnalysisRequest
            {
                AnalyzedImageContainingTheFaceB64 = imageWithPersonBase64,
                IsFaceLocationKnown = false
            });
        }

        public async Task<FacialAttributeAnalysisResponse> ExtractFacialAttributesWhenFaceLocationIsKnownAsync(string imageWithPersonBase64, FaceLocationBoundingBox faceLocation)
        {
            //call the server method for handling the facial recognition task
            return await _facialRecognitionAndAnalysisService.DoFacialAttributeAnalysisAsync(new FacialAttributeAnalysisRequest
            {
                AnalyzedImageContainingTheFaceB64 = imageWithPersonBase64,
                FaceBoundingBox = faceLocation,
                IsFaceLocationKnown = true
            });
        }
    }
}
