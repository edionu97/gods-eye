using Newtonsoft.Json;

namespace GodsEye.Application.Middleware.MessageBroadcaster.Messages
{
    public class OnlineClientMessage
    {
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
    }
}
