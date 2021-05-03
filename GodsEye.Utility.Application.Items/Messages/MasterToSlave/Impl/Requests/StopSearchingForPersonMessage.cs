namespace GodsEye.Utility.Application.Items.Messages.MasterToSlave.Impl.Requests
{
    public class StopSearchingForPersonMessage : IMasterToSlaveMessage
    {
        public string IdentificationNumber { get; set; }
    }
}
