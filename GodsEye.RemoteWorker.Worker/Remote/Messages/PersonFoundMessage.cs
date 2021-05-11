using System;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.RemoteWorker.Workers.Messages;

namespace GodsEye.RemoteWorker.Worker.Remote.Messages
{
    public class PersonFoundMessage : IRequestResponseMessage
    {
        public string MessageId { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public bool IsFound { get; set; }
        public (SearchForPersonResponse, FacialAttributeAnalysisResponse, string) MessageContent { get; set; }
    }
}
