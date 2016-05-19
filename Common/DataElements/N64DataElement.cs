using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Cereal64.Common.Utils;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Cereal64.Common.DataElements
{
    public abstract class N64DataElement : ITreeNodeElement, IXMLSerializable
    {
        //Address assigned by the DMAManager
        //public N64Address DMAAddress;

        public const string FILEOFFSET = "FileOffset";
        public const string LENGTH = "Length";
        private const string TAGINFO = "TagInfo";
        private const string GUID_STRING = "Guid";

        [CategoryAttribute("N64 Element"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Offset within the RomFile"),
        TypeConverter(typeof(Int32HexTypeConverter))]
        public int FileOffset { get; set; }

        public N64DataElement(XElement xml, byte[] fileData)
        {
            //Assume that the xml is perfectly formatted
            int offset, length;
            byte[] rawData;

            XAttribute att = xml.Attribute(FILEOFFSET);
            offset = int.Parse(att.Value);

            att = xml.Attribute(LENGTH);
            length = int.Parse(att.Value);

            rawData = new byte[length];
            Array.Copy(fileData, offset, rawData, 0, length);

            //Copy of constructor
            FileOffset = offset;
            RawData = rawData;

            att = xml.Attribute(TAGINFO);
            if (att != null)
                UserTag = att.Value;
            else
                UserTag = string.Empty;
        }

        public N64DataElement(int offset, byte[] rawData)
        {
            FileOffset = offset;
            RawData = rawData;
            UserTag = string.Empty;
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

        public virtual XElement GetAsXML()
        {
            XElement xml = new XElement(this.GetType().ToString()); //Can derive actual type from name with N64DataElementFactory

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

        public virtual void PostXMLLoad()
        {
            //If anything needs to be done post-loading via xml, do it here
        }
    }
}
