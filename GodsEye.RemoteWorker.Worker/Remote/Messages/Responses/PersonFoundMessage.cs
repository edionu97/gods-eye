using System;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;

namespace GodsEye.RemoteWorker.Worker.Remote.Messages.Responses
{
    public class PersonFoundMessage : IMessage
    {
        public string MessageId { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public bool IsFound { get; set; }
        public SearchForPersonResponse MessageContent { get; set; }
    }
}
