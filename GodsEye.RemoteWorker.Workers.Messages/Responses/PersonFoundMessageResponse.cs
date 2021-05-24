using System;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Utility.Application.Items.Geolocation.Model;

namespace GodsEye.RemoteWorker.Workers.Messages.Responses
{
    public class PersonFoundMessageResponse : IRequestResponseMessage
    {
        public Guid FindByWorkerId { get; set; }
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public bool IsFound { get; set; }
        public GeolocationInfo FromLocation { get; set; }
        public string SearchedPersonImageBase64 { get; set; }
        public (SearchForPersonResponse, string, FacialAttributeAnalysisResponse) MessageContent { get; set; }
    }
}
