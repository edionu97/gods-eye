using System;
using System.Collections.Generic;
using GodsEye.Utility.Application.Items.Geolocation.Model;

namespace GodsEye.RemoteWorker.Workers.Messages.Responses
{
    public class ActiveWorkerMessageResponse : IRequestResponseMessage
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public GeolocationInfo Geolocation { get; set; }
        public DateTime StartedAt { get; set; }
        public (Guid, IList<string>) MessageContent { get; set; }
    }
}
