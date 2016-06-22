using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;

namespace Cereal64.Common.Rom
{
    //To do: Delete this, I don't need it with the way DMAManager is implemented
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DmaAddress
    {
        [CategoryAttribute("Dma Address"),
        DescriptionAttribute("Ram segment the address resides in"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Segment { get; set; }

        [CategoryAttribute("Dma Address"),
        DescriptionAttribute("Offset of address from start of Ram segment"),
        TypeConverter(typeof(Int32HexTypeConverter))]
        public int Offset { get; set; }

        [CategoryAttribute("Dma Address"),
        DescriptionAttribute("Referenced data element (if found)"),
        TypeConverter(typeof(Int32HexTypeConverter))]
        public N64DataElement ReferenceElement { get; set; }

        public DmaAddress(byte segment, int offset)
        {
            Segment = segment;
            Offset = offset;
        }

        public DmaAddress(byte segment, uint offset)
        {
            Segment = segment;
            Offset = (int)offset;
        }

        public DmaAddress(uint address)
        {
            Segment = (byte)((address & 0xFF000000) >> 24);
            Offset = (int)(address & 0x00FFFFFF);
        }

        public DmaAddress(int address)
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

        public byte[] GetAsBytes()
        {
            byte[] bytes = new byte[4];
            bytes[0] = Segment;
            bytes[1] = (byte)((Offset & 0x00FF0000) >> 16);
            bytes[2] = (byte)((Offset & 0x0000FF00) >> 8);
            bytes[3] = (byte)(Offset & 0x000000FF);

            return bytes;
        }

        public override string ToString()
        {
            return "Address " + ByteHelper.DisplayValue(GetAsUInt(), true, true);
        }
    }
}
