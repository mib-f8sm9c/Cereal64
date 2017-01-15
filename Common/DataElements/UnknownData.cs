using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.ComponentModel;
using Cereal64.Common.Utils;

namespace Cereal64.Common.DataElements
{
    /// <summary>
    /// UnknownData is a special Element which allows for users to overwrite it in the N64DataElementCollection.
    /// 
    /// Essentially when first reading a file, all data will be tagged Unknown,
    /// and when it becomes known the user can add in the specific type of data
    /// (texture, F3D Command, etc.) on top of the Unknown data, and
    /// N64DataElementCollection will automatically manipulate the data to make it work.
    /// </summary>
    public class UnknownData : N64DataElement
    {
        private const string LOCKED = "Locked";

        private ShrinkingList<byte> _rawData;

        [CategoryAttribute("Element Settings"),
        DescriptionAttribute("When locked, known data found inside the UnknownData may not extract itself as a new N64Element")]
        public bool Locked { get; set; }

        public UnknownData(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
            XAttribute att = xml.Attribute(LOCKED);
            if (att != null)
                Locked = bool.Parse(att.Value);
        }

        public UnknownData(int index, byte[] rawData)
            : base(index, rawData)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return _rawData.ToArray();
            }
            set
            {
                _rawData = new ShrinkingList<byte>(value);
            }
        }

        public override int RawDataSize { get { return _rawData.Count; } }

        public bool TruncateData(int startIndexToKeep, int lengthToKeep)
        {
            if (startIndexToKeep < 0 || lengthToKeep <= 0 || startIndexToKeep + lengthToKeep > _rawData.Count)
            {
                return false;
            }

            int removeAtBeginningCount = startIndexToKeep;
            int removeAtEndCount = _rawData.Count - (startIndexToKeep + lengthToKeep);

            _rawData.RemoveFromStart(removeAtBeginningCount);
            _rawData.RemoveFromEnd(removeAtEndCount);

            FileOffset += removeAtBeginningCount;

            return true;
        }

        public override TreeNode GetAsTreeNode()
        {
            TreeNode node = new TreeNode();

            node.Text = string.Format("Unknown Block 0x{0:X8}", FileOffset);
            node.Tag = this;

            return node;
        }

        public override XElement GetAsXML()
        {
            XElement element = base.GetAsXML();
            element.Add(new XAttribute(LOCKED, Locked));
            return element;
        }

    }
}
