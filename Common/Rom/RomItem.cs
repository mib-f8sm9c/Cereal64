using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cereal64.Common.Rom
{
    public abstract class RomItem : IXMLSerializable, IXMLRomProjectItem
    {
        public RomItem()
        {

        }

        public virtual XElement GetAsXML()
        {
            XElement xml = new XElement(this.GetType().ToString());

            return xml;
        }

        public virtual string GetXMLPath()
        {
            return this.GetType().ToString();
        }

        //Keep this please!
        public string GetXMLName()
        {
            return this.GetType().ToString();
        }
    }
}
