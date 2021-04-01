using System;
using System.Collections.Generic;

namespace GodsEye.Utility.Application.Config.BaseConfig.Metadata
{
    public class ConfigMetadata
    {
        public IDictionary<Type, IConfig> ObjectTree { get; set; } = null;
    }

}
