using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Cereal64.Common.Utils;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Cereal64.Common
{
    public abstract class N64DataElement : IN64DataElement
    {
        //Address assigned by the DMAManager
        //public N64Address DMAAddress;

        private const string FILEOFFSET = "FileOffset";
        private const string LENGTH = "Length";
        private const string TAGINFO = "TagInfo";

        [CategoryAttribute("N64 Element"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Offset within the RomFile"),
        TypeConverter(typeof(Int32HexTypeConverter))]
        public int FileOffset { get; set; }

        public N64DataElement(int offset, byte[] rawData)
        {
            //Address = new N64Address(address);
            FileOffset = offset;
            RawData = rawData;
        }

        [CategoryAttribute("N64 Element"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Raw data for the N64 Element"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public abstract byte[] RawData { get; set; }

        [CategoryAttribute("N64 Element"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Size in bytes of the raw data"),
        TypeConverter(typeof(Int32HexTypeConverter))]
        public abstract int RawDataSize { get; }

        public bool ContainsOffset(int offset) { return offset >= FileOffset && offset < FileOffset + RawDataSize; }

        [CategoryAttribute("N64 Element"),
        DescriptionAttribute("User defined tag string")]
        public string UserTag { get; set; }

        public XElement GetAsXML()
        {
            XElement xml = new XElement(this.GetType().ToString());

            xml.Add(new XAttribute(FILEOFFSET, FileOffset));
            xml.Add(new XAttribute(LENGTH, RawDataSize));
            xml.Add(new XAttribute(TAGINFO, UserTag));

            return xml;
        }

        public virtual TreeNode GetAsTreeNode()
        {
            TreeNode node = new TreeNode();

            node.Text = string.Format("Element 0x{0:X8}", FileOffset);
            node.Tag = this;

            return node;
        }

    }
}
