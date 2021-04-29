using System;
using System.Linq;
using ImageMagick;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Helpers.Helpers.Image;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public partial class HeuristicLoadSheddingPolicy
    {
        /// <summary>
        /// Resized all the input images to a fixed size to speed up the image comparision
        /// Multi threaded remove 
        /// </summary>
        /// <param name="dataToBeResized">the data that will be resized</param>
        /// <param name="resizeTo">the width and height</param>
        /// <returns>the lists with all the resized images as base64</returns>
        private static IEnumerable<(MagickImage, int)> ResizeAllTheB64ImagesToFixedSize(
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
        private static IEnumerable<(MagickImage, int)> ApplyTheBulkRemovalOfDuplicateFramesPolicy(IList<(MagickImage, int)> frames, int itemsToRemove)
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

        /// <summary>
        /// In this round will be remove images that are not so similar (based on their similarity)
        /// </summary>
        /// <param name="frames">the frames that need to be load shed</param>
        /// <param name="itemsToRemove">the items that need to be removed from that list</param>
        /// <returns></returns>
        private static IEnumerable<(MagickImage, int)> ApplyTheRemovalOfNoSoSimilarFramesPolicy(IList<(MagickImage, int)> frames, int itemsToRemove)
        {
            //as long as we have items to remove
            while (frames.Any() && itemsToRemove-- > 0)
            {
                //get the most similar two images
                var index = FindMostSimilarTwoImagesRelativeToListBorders(frames);

                //remove the frame
                frames.RemoveAt(index);
            }

            return frames;
        }

        /// <summary>
        /// This function it is used to find the most similar image relative to border
        /// Assumption is that the frames are in the same order (or in reversed order) as in video stream
        /// The complexity is O(log2(N))
        /// </summary>
        /// <param name="videoFrameImages">the list of similar videoFrameImages</param>
        /// <returns>the index of the image that need to be removed</returns>
        private static int FindMostSimilarTwoImagesRelativeToListBorders(IList<(MagickImage, int)> videoFrameImages)
        {
            //iterate until one answer is found
            for (int leftImageIndex = 0, rightImageIndex = videoFrameImages.Count - 1; ;)
            {
                //check if we have only one element into list
                if (leftImageIndex == rightImageIndex)
                {
                    return leftImageIndex;
                }

                //check if items are siblings
                if (rightImageIndex - leftImageIndex == 1)
                {
                    //remove always the inferior value
                    return leftImageIndex;
                }

                //compute the middle
                var mid = (leftImageIndex + rightImageIndex) / 2;

                //unpack the left images
                var (leftImage, _) = videoFrameImages[leftImageIndex];

                //unpack the mid image
                var (middleImage, _) = videoFrameImages[mid];

                //unpack the right image
                var (rightImage, _) = videoFrameImages[rightImageIndex];

                //compute the similarity of li image with the mid image
                var (_, similarityLeftToMid) = ImageHelpers.CompareImages(leftImage, middleImage);

                //compute the similarity of the mid image with the ls one
                var (_, similarityMidToRight) = ImageHelpers.CompareImages(middleImage, rightImage);

                //if the image is most similar to the first img 
                if (similarityLeftToMid <= similarityMidToRight)
                {
                    //go in the left direction
                    rightImageIndex = mid;
                    continue;
                }

                // go in the right direction
                leftImageIndex = mid;
            }
        }
    }
}
