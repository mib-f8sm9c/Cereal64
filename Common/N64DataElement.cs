using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common
{
    public abstract class N64DataElement
    {
        public N64Address Address;

        public N64DataElement(int address, byte[] rawData)
        {
            Address = new N64Address(address);
            RawData = rawData;
        }

        public abstract byte[] RawData { get; set; }

        public abstract int RawDataSize { get; }
    }

}
