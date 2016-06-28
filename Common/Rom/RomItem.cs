using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cereal64.Common.Rom
{
    public abstract class RomItem : IXMLSerializable
    {
        public RomItem(XElement xml)
        {

        }

        public virtual XElement GetAsXML()
        {
            XElement xml = new XElement(this.GetType().ToString());

            return xml;
        }
    }
}
