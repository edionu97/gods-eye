namespace GodsEye.Utility.Application.Items.Messages.MasterToSlave.Impl.Requests
{
    public class SearchForPersonMessage : IMasterToSlaveMessage
    {
        public string MessageContent { get; set; }
        public string IdentificationNumber { get; set; }
    }
}
