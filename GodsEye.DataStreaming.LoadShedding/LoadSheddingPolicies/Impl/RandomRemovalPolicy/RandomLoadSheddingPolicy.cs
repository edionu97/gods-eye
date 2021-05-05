using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Args;
using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.LoadShedding;


namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl.RandomRemovalPolicy
{
    public class RandomLoadSheddingPolicy : ILoadSheddingPolicy
    {
        private readonly Random _randomGenerator;

        public RandomLoadSheddingPolicy()
        {
            _randomGenerator = new Random();
        }

        public Task<Queue<(DateTime, NetworkImageFrameMessage)>> 
            ApplyPolicyAsync(
                IEnumerable<(DateTime, NetworkImageFrameMessage)> dataToProcess, 
                int itemsToKeep,
                LoadSheddingPolicyArgs _)
        {
            //convert the IEnumerable in list 
            var data = dataToProcess.ToList();

            //positions to keep
            var generatePositionsToKeep = 
                GeneratePositionsThatWillBeKept(data.Count, itemsToKeep);

            //load shed the data
            var dataAfterLoadShedding = generatePositionsToKeep
                .Select(itemPosition => data[itemPosition]);

            //iterate the position indexes and remove the values
            return Task.FromResult(new Queue<(DateTime, NetworkImageFrameMessage)>(dataAfterLoadShedding));
        }

        private IEnumerable<int> 
            GeneratePositionsThatWillBeKept(int maxPositionValue, int itemsCount)
        {
            //keeps track of generated numbers
            var generatedNumbers = new HashSet<int>();

            //verify if the request can be fulfilled
            if (maxPositionValue < itemsCount)
            {
                throw new ArgumentException(string
                    .Format(Constants.WrongGenerationParametersMessage, maxPositionValue));
            }

            //generate the items
            while(generatedNumbers.Count != itemsCount)
            {
                //generate a new number
                var item = _randomGenerator.Next(maxPositionValue);

                //if the number was already generated skip
                if (generatedNumbers.Contains(item))
                {
                    continue;
                }

                //mark the item as generated
                generatedNumbers.Add(item);

                //return it
                yield return item;
            }
        }
    }
}
