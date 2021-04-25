using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.LoadShedding;


namespace GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl
{
    public class RandomLoadSheddingPolicy : ILoadSheddingPolicy
    {
        public Task<Queue<T>> ApplyPolicyAsync<T>(IList<T> data, int itemsToKeep)
        {
            //positions to keep
            var generatePositionsToKeep = 
                GeneratePositionsThatWillBeKept(data.Count, itemsToKeep);

            //load shed the data
            var dataAfterLoadShedding = generatePositionsToKeep
                .Select(itemPosition => data[itemPosition]);

            //iterate the position indexes and remove the values
            return Task.FromResult(new Queue<T>(dataAfterLoadShedding));
        }

        private static IEnumerable<int> GeneratePositionsThatWillBeKept(int maxPositionValue, int itemsCount)
        {
            //create the random generator
            var randomGenerator = new Random();

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
                var item = randomGenerator.Next(maxPositionValue);

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
