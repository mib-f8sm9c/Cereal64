using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common
{
    public abstract class N64DataElement
    {
        //Address assigned by the DMAManager
        public N64Address DMAAddress;

        public int FileOffset;

        public N64DataElement(int offset, byte[] rawData)
        {
            //Address = new N64Address(address);
            FileOffset = offset;
            RawData = rawData;
        }

        public abstract byte[] RawData { get; set; }

        public abstract int RawDataSize { get; }

        public bool ContainsOffset(int offset) { return offset >= FileOffset && offset < FileOffset + RawDataSize; }
    }
}
