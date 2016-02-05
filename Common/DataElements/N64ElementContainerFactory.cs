using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;

namespace Cereal64.Common.DataElements
{
    /// <summary>
    /// A factory to allow for IN64ElementContainer types to be added from outside .dll files,
    ///  and yet still function with the xml saving/loading functionality of the
    ///  RomProject. Slightly dirty method, but it allows for future development of
    ///  IN64ElementContainer types without changing Cereal64.Common
    /// </summary>
    public static class N64ElementContainerFactory
    {
        //Default the element types to the ones that exist in Common
        private static Dictionary<string, Type> _elementTypes = new Dictionary<string, Type>() { 
            { typeof(MiscN64ElementContainer).ToString(), typeof(MiscN64ElementContainer) } };

        public static void AddN64ElementContainersFromAssembly(Assembly assembly)
        {
            Type[] iDataTypes = (from t in assembly.GetExportedTypes()
                     where !t.IsInterface && !t.IsAbstract
                     where typeof(IN64ElementContainer).IsAssignableFrom(t)
                     select t).ToArray();
            foreach (Type type in iDataTypes)
            {
                if (!_elementTypes.ContainsValue(type))
                    _elementTypes.Add(type.ToString(), type);
            }
        }

        public static IN64ElementContainer CreateN64ElementContainerFromType(string typeName)
        {
            if(_elementTypes.ContainsKey(typeName))
            {
                return (IN64ElementContainer)Activator.CreateInstance(_elementTypes[typeName]);
            }

            return null;
        }

        public static N64DataElement CreateN64DataElementFromType(string typeName, XElement xml, byte[] fileData)
        {
            if (_elementTypes.ContainsKey(typeName))
                return (N64DataElement)Activator.CreateInstance(_elementTypes[typeName], xml, fileData);

            return null;
        }
    }
}
