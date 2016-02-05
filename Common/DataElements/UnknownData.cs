using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

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
        //Use a list to make adding/removing data easier
        private List<byte> _rawData;

        public UnknownData(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
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
                _rawData = value.ToList();
            }
        }

        public override int RawDataSize { get { return _rawData.Count; } }

        public override TreeNode GetAsTreeNode()
        {
            TreeNode node = new TreeNode();

            node.Text = string.Format("Unknown Block 0x{0:X8}", FileOffset);
            node.Tag = this;

            return node;
        }
    }
}
