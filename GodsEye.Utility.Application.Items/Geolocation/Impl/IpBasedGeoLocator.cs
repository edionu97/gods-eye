using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Items.Geolocation.Model;

using static GodsEye.Utility.Application.Items.Constants.String.StringConstants.Apis;

namespace GodsEye.Utility.Application.Items.Geolocation.Impl
{
    public class IpBasedGeoLocator : IGeoLocator
    {
        public Task<GeolocationInfo> GetLocationAsync()
        {
            return Task.Run(() =>
            {
                //get the public ip
                var publicIp = new WebClient()
                    .DownloadString(GetRealIpAddressApi);

                //get the geo location
                var geoLocationJson = new WebClient()
                    .DownloadString(string
                        .Format(GetGeolocationApi, publicIp));

                //deserialize object
                var instance = JsonConvert
                    .DeserializeObject<GeolocationInfo>(geoLocationJson);

                //get the instance
                return instance;
            });
        }
    }
}
