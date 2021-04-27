using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using GodsEye.Utility.Application.Helpers.Helpers.Image;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public partial class HeuristicLoadSheddingPolicy
    {
        /// <summary>
        /// Resized all the input images to a fixed size to speed up the image comparision
        /// </summary>
        /// <param name="dataToBeResized">the data that will be resized</param>
        /// <param name="resizeTo">the width and height</param>
        /// <returns>the lists with all the resized images as base64</returns>
        private static IList<string> ResizeAllTheB64ImagesToFixedSize(
            IEnumerable<(DateTime, NetworkImageFrameMessage)> dataToBeResized, (int Width, int Height) resizeTo)
        {
            //the new data bag
            var newDataBag = new ConcurrentBag<(string Value, int Position)>();

            //do the the image resizing in parallel
            Parallel.ForEach(
                dataToBeResized
                    .Select((item, index) => (item, index)),
                p =>
                {
                    //unpack the information
                    var ((_, imageFrame), index) = p;

                    //resize the image
                    var resizedImage = ImageHelpers
                        .ResizeBase64Image(imageFrame.ImageBase64EncodedBytes, resizeTo);

                    //add data into the bag
                    newDataBag.Add((resizedImage, index));
                });

            //reconstruct the data from bag, based on the index value
            return newDataBag
                .OrderBy(x => x.Position)
                .Select(x => x.Value)
                .ToList();
        }

        /// <summary>
        /// This function it is used in order to find the most similar image relative to the data borders
        /// </summary>
        /// <param name="videoFrameImages">the list of similar videoFrameImages</param>
        /// <returns></returns>
        public static int FindMostSimilarTwoImagesRelativeToListBorders(IList<string> videoFrameImages)
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

                //compute the similarity of li image with the mid image
                var (_, similarityLeftToMid) = ImageHelpers
                    .CompareImages(videoFrameImages[leftImageIndex], videoFrameImages[mid]);

                //compute the similarity of the mid image with the ls one
                var (_, similarityMidToRight) = ImageHelpers
                    .CompareImages(videoFrameImages[mid], videoFrameImages[rightImageIndex]);

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
