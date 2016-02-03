using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cereal64.Common
{
    public interface IXMLSerializable
    {
        XElement GetAsXML();
    }
}
