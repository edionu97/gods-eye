using System;
using System.Text;
using WatsonWebsocket;
using System.Net.WebSockets;
using GodsEye.Application.Middleware.MessageBroadcaster.Messages;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;

namespace GodsEye.Application.Middleware.MessageBroadcaster.Impl
{
    public partial class MessageBroadcasterMiddleware
    {
        private void OnMessageFromClient(object _, MessageReceivedEventArgs e)
        {
            //get the ws session id
            var sessionId = e.IpPort;

            //ignore other messages than text
            if (e.MessageType != WebSocketMessageType.Text)
            {
                return;
            }

            //get the message string
            var messageString = Encoding.UTF8.GetString(e.Data);
            try
            {
                //deserialized the message
                var deserializedMessage =
                    JsonSerializerDeserializer<OnlineClientMessage>
                        .Deserialize(messageString);

                //get the client id
                var userId = deserializedMessage.ClientId;

                //remove the previous instance
                _wsSessionIdToUserId.TryRemove(sessionId, out var _);

                //map the user id into ws session id
                _wsSessionIdToUserId.TryAdd(sessionId, userId);
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void OnClientDisconnected(object _, ClientDisconnectedEventArgs e)
        {
            //get the session id
            var sessionId = e.IpPort;

            //remove the session id from the list
            _wsSessionIdToUserId.TryRemove(sessionId, out var _);
        }
    }
}
