using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common.Utils;
using System.ComponentModel;

namespace Cereal64.Common.Rom
{
    //To do: Delete this, I don't need it with the way DMAManager is implemented
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct DmaAddress
    {
        [CategoryAttribute("Dma Address"),
        DescriptionAttribute("Ram segment the address resides in"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Segment { get; set; }

        [CategoryAttribute("Dma Address"),
        DescriptionAttribute("Offset of address from start of Ram segment"),
        TypeConverter(typeof(Int32HexTypeConverter))]
        public int Offset { get; set; }

        public DmaAddress(byte segment, int offset)
            : this()
        {
            Segment = segment;
            Offset = offset;
        }

        public DmaAddress(byte segment, uint offset)
            : this()
        {
            Segment = segment;
            Offset = (int)offset;
        }

        public DmaAddress(uint address)
            : this()
        {
            Segment = (byte)((address & 0xFF000000) >> 24);
            Offset = (int)(address & 0x00FFFFFF);
        }

        public DmaAddress(int address)
            : this()
        {
            Segment = (byte)((address & 0xFF000000) >> 24);
            Offset = (address & 0x00FFFFFF);
        }

        public uint GetAsUInt()
        {
            return (uint)(Segment << 24) | (uint)(Offset & 0x00FFFFFF);
        }

        public int GetAsInt()
        {
            return (Segment << 24) | (Offset & 0x00FFFFFF);
        }

        public override string ToString()
        {
            return "Address " + ByteHelper.DisplayValue(GetAsUInt(), true, true);
        }
    }
}
