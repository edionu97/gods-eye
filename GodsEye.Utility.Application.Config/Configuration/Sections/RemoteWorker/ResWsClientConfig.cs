using GodsEye.Utility.Application.Helpers.Helpers.Paths;
using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class ResWsClientConfig : AbstractConfig
    {
        public bool AutoGenerate { get; set; }

        private string _wsGenClientLocation;
        public string WsGenClientLocation
        {
            get => _wsGenClientLocation;
            set
            {
                if (string.IsNullOrEmpty(_wsGenClientLocation = value))
                {
                    return;
                }

                _wsGenClientLocation = PathHelpers.ResolvePath(value);
            }
        }

        private string _wsClientTemplateLocation;
        public string WsClientTemplateLocation
        {
            get => _wsClientTemplateLocation;
            set
            {
                if (string.IsNullOrEmpty(_wsClientTemplateLocation = value))
                {
                    return;
                }

                _wsClientTemplateLocation = PathHelpers.ResolvePath(value);
            }
        }

        public void Deconstruct(
            out string wsGenClientLocation, 
            out string wsClientTemplateLocation, out bool autoGenerate)
        {
            wsGenClientLocation = _wsGenClientLocation;
            wsClientTemplateLocation = _wsClientTemplateLocation;
            autoGenerate = AutoGenerate;
        }
    }
}
