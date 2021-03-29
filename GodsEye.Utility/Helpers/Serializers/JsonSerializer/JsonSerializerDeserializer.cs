using Newtonsoft.Json;

namespace GodsEye.Utility.Helpers.Serializers.JsonSerializer
{
    public static class JsonSerializerDeserializer<T>
    {
        /// <summary>
        /// This function it is used for serializing an object
        /// </summary>
        /// <param name="object">the object that will be serialized</param>
        /// <returns>a string representing the json or null if the object is null</returns>
        public static string Serialize(T @object)
        {
            //treat the null case
            if (@object == null)
            {
                return null;
            }

            //convert to json
            var ignoreOptions = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert
                .SerializeObject(@object, Formatting.Indented, ignoreOptions);
        }

        /// <summary>
        /// Deserializes an string object into an instance of T
        /// </summary>
        /// <param name="object">the object representation as json</param>
        /// <returns>the instance of the object</returns>
        public static T Deserialize(string @object)
        {
            return string.IsNullOrEmpty(@object) 
                ? default 
                : JsonConvert.DeserializeObject<T>(@object);
        }
    }
}
