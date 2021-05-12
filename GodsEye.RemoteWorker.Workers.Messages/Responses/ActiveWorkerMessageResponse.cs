using System;
using System.Collections.Generic;

namespace GodsEye.RemoteWorker.Workers.Messages.Responses
{
    public class ActiveWorkerMessageResponse : IRequestResponseMessage
    {
        public string MessageId { get; set; }

        public (Guid, IList<string>) MessageContent { get; set; }
    }
}
