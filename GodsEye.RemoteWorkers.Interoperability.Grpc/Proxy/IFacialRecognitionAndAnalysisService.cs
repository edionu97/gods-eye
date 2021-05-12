using System.Threading;
using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;

namespace GodsEye.RemoteWorkers.Interoperability.Grpc.Proxy
{
    public interface IFacialRecognitionAndAnalysisService
    {
        /// <summary>
        /// Checks if a person it is located in a specific image and returns the basic information about it such as
        ///     *  a value indicating if the person is recognized 
        ///     *  if the person is recognized then in the response the face locations will be included (the bounding box locations)
        ///     *  the key points of the face (eyes location, nose and mouth) 
        /// </summary>
        /// <param name="base64EncodedPersonImage">the image that contains the image of the searched person</param>
        /// <param name="base64EncodedImage">the image in which we are searching for that person</param>
        /// <param name="token">the cancellation token</param>
        /// <returns> an instance of SearchResponse containing information as follows</returns>
        public Task<SearchForPersonResponse>
            SearchPersonInImageAsync(
                string base64EncodedPersonImage, string base64EncodedImage, CancellationToken token);

        /// <summary>
        /// Analyse the face and extracts basic information as age, emotion, nationality and gender
        /// </summary>
        /// <param name="base64FaceImage">the face image as base64</param>
        /// <param name="faceBoundingBox">the coordinates of the bounding box that identifies the face</param>
        /// <param name="token">the cancellation token</param>
        /// <returns>an instance of the FacialAttributeAnalysisResponse containing the required information</returns>
        public Task<FacialAttributeAnalysisResponse> AnalyseFaceAsync(
            string base64FaceImage, FaceLocationBoundingBox faceBoundingBox, CancellationToken token);
    }
}
