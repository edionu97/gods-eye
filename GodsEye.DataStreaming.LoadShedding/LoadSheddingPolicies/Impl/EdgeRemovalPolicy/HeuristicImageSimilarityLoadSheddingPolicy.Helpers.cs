using ImageMagick;
using System.Linq;
using System.Collections.Generic;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Args;
using GodsEye.Utility.Application.Helpers.Helpers.Image;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl.EdgeRemovalPolicy
{
    public partial class HeuristicImageSimilarityLoadSheddingPolicy
    {
        /// <summary>
        /// In this round will be remove images that are not so similar (based on their similarity)
        /// </summary>
        /// <param name="frames">the frames that need to be load shed</param>
        /// <param name="itemsToRemove">the items that need to be removed from that list</param>
        /// <param name="_"> the policy args, unused for this policy</param>
        /// <returns></returns>
        protected virtual IEnumerable<(MagickImage, int)> 
            ApplyTheRemovalOfRemainingFramesPhase(
                IList<(MagickImage, int)> frames, 
                int itemsToRemove, LoadSheddingPolicyArgs _)
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
        private static int 
            FindMostSimilarTwoImagesRelativeToListBorders(IList<(MagickImage, int)> videoFrameImages)
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
