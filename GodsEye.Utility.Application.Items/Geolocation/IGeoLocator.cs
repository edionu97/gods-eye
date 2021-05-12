using System.Threading.Tasks;
using GodsEye.Utility.Application.Items.Geolocation.Model;

namespace GodsEye.Utility.Application.Items.Geolocation
{
    public interface IGeoLocator
    {
        /// <summary>
        /// This method it is used for getting the geolocation
        /// </summary>
        /// <returns>the geolocation</returns>
        public Task<GeolocationInfo> GetLocationAsync();
    }
}
