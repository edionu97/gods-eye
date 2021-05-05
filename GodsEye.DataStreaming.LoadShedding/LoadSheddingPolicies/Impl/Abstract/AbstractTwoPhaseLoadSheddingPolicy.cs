using System;
using ImageMagick;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Args;
using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Helpers.Helpers.Image;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl.Abstract
{
    public abstract class AbstractTwoPhaseLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public abstract Task<Queue<(DateTime, NetworkImageFrameMessage)>> 
            ApplyPolicyAsync(
                IEnumerable<(DateTime, NetworkImageFrameMessage)> data, 
                int itemsToKeep,
                LoadSheddingPolicyArgs args = null);

        /// <summary>
        /// Resized all the input images to a fixed size to speed up the image comparision
        /// Multi threaded remove 
        /// </summary>
        /// <param name="dataToBeResized">the data that will be resized</param>
        /// <param name="resizeTo">the width and height</param>
        /// <returns>the lists with all the resized images as base64</returns>
        protected static IEnumerable<(MagickImage, int)> ResizeAllTheB64ImagesToFixedSize(
            IEnumerable<(DateTime, NetworkImageFrameMessage)> dataToBeResized, (int Width, int Height) resizeTo)
        {
            //the new data bag
            var newDataBag = new ConcurrentBag<(MagickImage Value, int Position)>();

            //do the the image resizing in parallel
            Parallel.ForEach(
                dataToBeResized
                    .Select((item, index) => (item, index)),
                p =>
                {
                    //unpack the information
                    var ((_, imageFrame), index) = p;

                    //resize the image and convert it to base64
                    var resizedImage = ImageHelpers
                        .ResizeBase64Image(imageFrame.ImageBase64EncodedBytes, resizeTo);

                    //construct the image from byte array
                    var imageInstance = new MagickImage(
                        ImageHelpers.ConvertImageFromBase64(resizedImage));

                    //add data into the bag
                    newDataBag.Add((imageInstance, index));
                });

            //reconstruct the data from bag, based on the index value
            return newDataBag
                .OrderBy(x => x.Position)
                .ToList();
        }

        /// <summary>
        /// Remove the bulk duplicate frames (assuming that we are processing a video frame)
        /// </summary>
        /// <param name="frames">the frames that are about to be processed </param>
        /// <param name="itemsToRemove">the number of items to remove</param>
        /// <returns>an IEnumerable containing frames to keep (size can be different) </returns>
        protected static IEnumerable<(MagickImage, int)> 
            ApplyTheBulkRemovalOfDuplicateFramesPhase(IList<(MagickImage, int)> frames, int itemsToRemove)
        {
            //stop if there are no other frames to remove
            if (!frames.Any())
            {
                yield break;
            }

            //keep the index of the precedent frame
            var precedentFrameIdx = 0;

            //return the first frame
            yield return frames[precedentFrameIdx];

            //check against other frames
            for (var index = 1; index < frames.Count; precedentFrameIdx = index, ++index)
            {
                //repeat either until the frames 
                while (index < frames.Count && itemsToRemove > 0)
                {
                    //compare the images
                    var (imageType, _) = ImageHelpers
                        .CompareImages(frames[precedentFrameIdx].Item1, frames[index].Item1);

                    //if the images are not different advance
                    if (imageType == ImageComparisonResultType.Different)
                    {
                        break;
                    }

                    //keep the last frame
                    precedentFrameIdx = index++;

                    //decrement the number fo frames to remove
                    --itemsToRemove;
                }

                //ensure that we are always in the list bounds
                if (index >= frames.Count)
                {
                    continue;
                }

                //return the frame
                yield return frames[index];
            }
        }
    }
}
