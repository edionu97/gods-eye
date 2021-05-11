using System.Threading;
using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;

namespace GodsEye.RemoteWorker.Worker.FacialAnalysis
{
    public interface IFacialAnalysisAndRecognitionWorker
    {
        public FarwStartingInformation AnalysisSummary { get; }

        /// <summary>
        /// This method it is used for searching for a person
        /// It will run in infinite loop either until the person is found wither until the operation is canceled
        /// </summary>
        /// <param name="startingInformation">the starting information</param>
        /// <param name="cancellationToken">the cancellation token</param>
        /// <exception cref="System.Exception">if it is canceled or any problem occurs</exception>
        /// <returns>the response</returns>
        public Task<SearchForPersonResponse>
            StartSearchingForPersonAsync(FarwStartingInformation startingInformation, CancellationToken cancellationToken);

        /// <summary>
        /// This method it is used for facial attribute analysis
        /// </summary>
        /// <param name="base64Image">the image containing the face</param>
        /// <param name="faceLocation">the location of the face in image</param>
        /// <param name="token">the cancellation token</param>
        /// <returns>the facial attribute analysis response</returns>
        public Task<FacialAttributeAnalysisResponse>
            AnalyzeFaceAndExtractFacialAttributesAsync(
                string base64Image,
                FaceLocationBoundingBox faceLocation, CancellationToken token);
    }
}
