using System;
using System.Collections.Generic;
using GodsEye.Utility.Application.Config.BaseConfig.Metadata;

namespace GodsEye.Utility.Application.Config.BaseConfig.Abstract
{
    public abstract class AbstractConfig : IConfig
    {
        private static readonly object LockObject = new object();

        protected ConfigMetadata Metadata { get; private set; }

        public T Get<T>() where T : class, IConfig
        {
            //verify the value of the metadata
            if (Metadata == null)
            {
                //critical section
                lock (LockObject)
                {
                    Metadata ??= InitializeMetadata();
                }
            }

            //get the value 
            IConfig value = null;

            Metadata.ObjectTree?.TryGetValue(typeof(T), out value);

            //return the value
            return value as T;
        }

        /// <summary>
        /// This function it is used for building up the object tree
        /// Sets the metadata of the current object
        /// </summary>
        /// <returns>the metadata</returns>
        protected ConfigMetadata InitializeMetadata()
        {
            //only a single thread can set this
            //initialize the metadata
            Metadata = new ConfigMetadata
            {
                ObjectTree = new Dictionary<Type, IConfig>
                {
                    [GetType()] = this
                }
            };

            //iterate the properties
            foreach (var propertyInfo in GetType().GetProperties())
            {
                //get the value of the property
                var propertyValue = propertyInfo.GetValue(this);

                //check if property value is extends the IConfig class
                if (!(propertyValue is AbstractConfig abstractConfig))
                {
                    continue;
                }

                //add the property in the object tree list
                foreach (var (type, reference) in abstractConfig.InitializeMetadata().ObjectTree)
                {
                    Metadata.ObjectTree.Add(type, reference);
                }
            }

            //set the object tree
            return Metadata;
        }

    }
}
