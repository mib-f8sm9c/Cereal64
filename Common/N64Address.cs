using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.Common
{
    public class N64Address
    {
        public byte Segment;

        public int Offset;

        public N64Address(byte segment, int offset)
        {
            Segment = segment;
            Offset = offset;
        }

        public N64Address(uint address)
        {
            Segment = (byte)((address & 0xFF000000) >> 24);
            Offset = (int)(address & 0x00FFFFFF);
        }

        public N64Address(int address)
        {
            Segment = (byte)((address & 0xFF000000) >> 24);
            Offset = (address & 0x00FFFFFF);
        }
    }
}
