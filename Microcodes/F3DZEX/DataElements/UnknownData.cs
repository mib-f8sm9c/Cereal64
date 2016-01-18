using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public class UnknownData : N64DataElement
    {
        private byte[] _rawData;

        public UnknownData(int index, byte[] rawData)
            : base(index, rawData)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return _rawData;
            }
            set
            {
                _rawData = value;
            }
        }

        public override int RawDataSize { get { return _rawData.Length; } }
    }
}
