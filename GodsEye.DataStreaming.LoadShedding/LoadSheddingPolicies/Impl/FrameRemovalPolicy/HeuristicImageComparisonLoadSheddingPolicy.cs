using ImageMagick;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Helpers.Helpers.Image;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Args;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl.EdgeRemovalPolicy;

namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl.FrameRemovalPolicy
{
    public class HeuristicImageComparisonLoadSheddingPolicy : HeuristicImageSimilarityLoadSheddingPolicy
    {
        public HeuristicImageComparisonLoadSheddingPolicy(
            IConfig config,
            ILogger<HeuristicImageComparisonLoadSheddingPolicy> logger) : base(config, logger)
        {
        }

        protected override IEnumerable<(MagickImage, int)> ApplyTheRemovalOfRemainingFramesPhase(
            IList<(MagickImage, int)> frames,
            int itemsToRemove,
            LoadSheddingPolicyArgs args)
        {
            //handle the case in which we are removing all the frames in the first round
            if (itemsToRemove <= 0 || args == null)
            {
                return frames;
            }

            //read the heuristic config 
            var (w, h) = HeuristicConfig;

            //resize the image and convert it to base64
            var resizedImage = ImageHelpers
                .ResizeBase64Image(args.SearchedImageBase64, (w, h));

            //construct the image from byte array
            var searchedImage = new MagickImage(
                ImageHelpers.ConvertImageFromBase64(resizedImage));

            //create the similarity heap
            var similarityHeap = CreateSimilarityHeap();

            //iterate the resized frames
            foreach (var (image, originalImgIndex) in frames)
            {
                //compute the similarity of current image with the searched one
                var (_, similarity) = ImageHelpers.CompareImages(image, searchedImage);

                //add the image in max heap
                similarityHeap.Add((image, similarity, originalImgIndex));
            }

            //get the items that remain
            var itemsThatRemain = new List<(MagickImage, int)>();

            //get the items to keep
            var itemsToKeep = frames.Count - itemsToRemove;
            while (itemsToKeep-- > 0 && similarityHeap.Any())
            {
                //deleting the max value means the most similar image with the searched one
                var (image, similarity, index) = similarityHeap.DeleteMax();

                //add the item into the list
                itemsThatRemain.Add((image, index));
            }

            //keep the same order as on input
            return itemsThatRemain.OrderBy(x => x.Item2);
        }

        /// <summary>
        /// Create the similarity heap
        /// </summary>
        /// <returns></returns>
        private static C5.IntervalHeap<(MagickImage, double, int)> CreateSimilarityHeap()
        {
            //create a new heap
            return new C5.IntervalHeap<(MagickImage, double, int)>(Comparer<(MagickImage, double, int)>
                .Create((x, y) =>
                {
                    //unpack the objects
                    var (_, valueA, _) = x;
                    var (_, valueB, _) = y;

                    //compare the values
                    return valueB.CompareTo(valueA);
                }));
        }
    }
}
