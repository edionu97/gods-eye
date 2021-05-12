namespace GodsEye.Utility.Application.Items.Constants.String
{
    public static class StringConstants
    {
        public static string CameraToBussQueueName => "camera-to-buss";

        public static string MasterToSlaveBusQueueName => "master-to-slave-bus";

        public static string SlaveToMasterBusQueueName => "slave-to-master-bus";

        public static class Names
        {
            public static string GeneratedWsClientNameFormat => "gen-ws-client-for_{0}_{1}.html";
        }

        public static class Apis
        {
            public static string GetRealIpAddressApi => "https://api.ipify.org";

            public static string GetGeolocationApi => "https://reallyfreegeoip.org/json/{0}";
        }
    }
}
