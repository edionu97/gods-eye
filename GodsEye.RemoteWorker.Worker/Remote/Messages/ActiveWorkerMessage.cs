using System;
using System.Collections.Generic;
using GodsEye.RemoteWorker.Workers.Messages;

namespace GodsEye.RemoteWorker.Worker.Remote.Messages
{
    public class ActiveWorkerMessage : IRequestResponseMessage
    {
        public string MessageId { get; set; }

        public (Guid, IList<string>) MessageContent { get; set; }
    }
}
