using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;

namespace Cereal64.Common.Rom
{
    public static class RomItemFactory
    {
        private static Dictionary<string, Type> _itemTypes = new Dictionary<string, Type>();

        public static void AddRomItemsFromAssembly(Assembly assembly)
        {
            Type[] iDataTypes = (from t in assembly.GetExportedTypes()
                                 where !t.IsInterface && !t.IsAbstract
                                 where typeof(RomItem).IsAssignableFrom(t)
                                 select t).ToArray();
            foreach (Type type in iDataTypes)
            {
                if (!_itemTypes.ContainsValue(type))
                    _itemTypes.Add(type.ToString(), type);
            }
        }

        public static RomItem CreateRomItemFromType(string typeName, XElement xml)
        {
            if (_itemTypes.ContainsKey(typeName))
                return (RomItem)Activator.CreateInstance(_itemTypes[typeName], xml);

            return null;
        }
    }
}
