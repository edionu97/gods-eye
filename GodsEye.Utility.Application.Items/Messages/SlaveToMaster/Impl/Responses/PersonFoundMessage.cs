using System;

namespace GodsEye.Utility.Application.Items.Messages.SlaveToMaster.Impl.Responses
{
    public class PersonFoundMessage<T>
    { 
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }

        public bool IsFound { get; set; }
        public string IdentificationNumber { get; set; }
        public T MessageContent { get; set; }
    }
}
