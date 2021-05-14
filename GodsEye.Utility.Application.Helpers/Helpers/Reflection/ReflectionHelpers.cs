using System;
using System.Collections.Generic;
using System.Linq;

namespace GodsEye.Utility.Application.Helpers.Helpers.Reflection
{
    public static class ReflectionHelpers
    {
        /// <summary>
        /// This function it is used for getting all the properties from an object that have a specific type
        /// </summary>
        /// <typeparam name="T">type of the property</typeparam>
        /// <param name="object">the object</param>
        /// <returns>a list with all the values from the object's properties</returns>
        public static IEnumerable<T> GetPropertyValuesOfType<T>(object @object) where T : class
        {
            //treat the null case
            if (@object == null)
            {
                yield break;
            }

            //iterate through properties
            foreach (var propertyInfo in @object.GetType().GetProperties())
            {
                //get the value of the property
                var propertyValue = propertyInfo.GetValue(@object);
                
                //ignore null values
                if (propertyValue == null)
                {
                    continue;
                }

                //get only properties with the searched type
                if (!IsChildOf<T>(propertyValue.GetType()) && typeof(T) != propertyValue.GetType())
                {
                    continue;
                }

                //get the value of the property
                yield return propertyValue as T;
            }
        }


        /// <summary>
        /// Check if a type is child of another type
        /// </summary>
        /// <typeparam name="T">parent type</typeparam>
        /// <param name="type">verified type</param>
        /// <returns>true if the type is child of T or false otherwise</returns>
        public static bool IsChildOf<T>(Type type)
        {
            //get the type of T
            var tType = typeof(T);

            //treat the case of interface
            if (tType.IsInterface)
            {
                return type.GetInterfaces().Any(x => x == tType);
            }

            //check if the type is subtype of T
            if (type.BaseType == null)
            {
                return false;
            }

            //verify types
            return type.BaseType == tType || IsChildOf<T>(type.BaseType);
        }
    }
}
