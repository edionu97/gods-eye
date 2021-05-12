using System;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;

namespace GodsEye.RemoteWorker.Workers.Messages.Responses
{
    public class PersonFoundMessageResponse : IRequestResponseMessage
    {
        public string MessageId { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public bool IsFound { get; set; }
        public (SearchForPersonResponse, string, FacialAttributeAnalysisResponse) MessageContent { get; set; }
    }
}
