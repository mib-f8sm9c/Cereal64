using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common
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

        //Need this so it can split 
        //private RomFile _parent;

        public UnknownData(/*RomFile parent, */int index, byte[] rawData)
            : base(index, rawData)
        {
            //_parent = parent;
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

        //Permanently drop some data. Not used currently
        //public bool Shorten(int fileOffset, int length)
        //{
        //    int startOffset = fileOffset - FileOffset;
        //    int endOffset = _rawData.Count - (startOffset + length);

        //    if (startOffset < 0 || endOffset < 0 ||
        //        startOffset + endOffset > _rawData.Count)
        //        return false;

        //    _rawData.RemoveRange(startOffset + length, endOffset);
        //    _rawData.RemoveRange(0, startOffset);
        //    return true;
        //}
    }
}
