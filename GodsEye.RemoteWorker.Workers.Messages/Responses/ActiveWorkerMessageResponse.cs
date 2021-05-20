using System;
using System.Collections.Generic;
using GodsEye.RemoteWorker.Workers.Messages.Components;
using GodsEye.Utility.Application.Items.Geolocation.Model;

namespace GodsEye.RemoteWorker.Workers.Messages.Responses
{
    public class ActiveWorkerMessageResponse : IRequestResponseMessage
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public GeolocationInfo Geolocation { get; set; }
        public DateTime StartedAt { get; set; }
        public (Guid, IList<JobSummary>) MessageContent { get; set; }
    }
}
