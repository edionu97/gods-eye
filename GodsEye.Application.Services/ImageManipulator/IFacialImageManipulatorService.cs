using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Application.Services.ImageManipulator.Helpers;

namespace GodsEye.Application.Services.ImageManipulator
{
    public interface IFacialImageManipulatorService
    {
        /// <summary>
        /// This method it is used for identifying the face from the image
        /// </summary>
        /// <param name="imageBase64">the base64 image</param>
        /// <param name="locationBox">the location of the face</param>
        /// <param name="drawingOptions">if the drawing options (containing info about the color and the line thickness)</param>
        /// <param name="keyPointsLocation">the face key points (mouth, nose, left eye, right eye)</param>
        /// <returns>the modified image</returns>
        public Task<string>
            IdentifyAndDrawRectangleAroundFaceAsync(
                string imageBase64,
                FaceLocationBoundingBox locationBox,
                DrawingOptions drawingOptions = null,
                FaceKeypointsLocation keyPointsLocation = null);

        /// <summary>
        /// This method it is used for extracting the face from the given image
        /// </summary>
        /// <param name="imageBase64">the given image</param>
        /// <param name="locationBox">the location in which the face should be</param>
        /// <param name="scaleTo">if the value is provider for this parameter, than the face will be resized to the specified size</param>
        /// <returns>the ROI between (topX, topY, bottomX, bottomY)</returns>
        public Task<string> 
            ExtractFaceAsync(
                string imageBase64,
                FaceLocationBoundingBox locationBox,
                (int Width, int Height)? scaleTo = null);

        /// <summary>
        /// Resize the face image to a defined width and height
        /// </summary>
        /// <param name="imageBase64">the image</param>
        /// <param name="size">the new size</param>
        /// <returns>the modified image</returns>
        public Task<string> 
            ResizeFaceImageAsync(string imageBase64, (int Width, int Height) size);
    }
}
