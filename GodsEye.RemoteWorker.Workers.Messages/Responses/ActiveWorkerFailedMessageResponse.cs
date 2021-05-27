using System;
using GodsEye.RemoteWorker.Workers.Messages.Components;

namespace GodsEye.RemoteWorker.Workers.Messages.Responses
{
    public class ActiveWorkerFailedMessageResponse : IRequestResponseMessage
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public FailureSummary FailureSummary { get; set; }
    }
}
