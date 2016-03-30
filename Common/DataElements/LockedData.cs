using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common.Rom;
using System.Xml.Linq;

namespace Cereal64.Common.DataElements
{
    /// <summary>
    /// LockedData is similar to UnknownData as it stores data whose function is not yet known.
    /// However Unknown data may be overwritten and split up, whereas LockedData is 
    /// </summary>
    public class LockedData : N64DataElement
    {
        private List<byte> _rawData;

        public LockedData(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        public LockedData(int index, byte[] rawData)
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

    }
}
