using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Cereal64.Common.Utils;

namespace Cereal64.Common
{
    public abstract class N64DataElement
    {
        //Address assigned by the DMAManager
        //public N64Address DMAAddress;

        [CategoryAttribute("N64 Element"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Offset within the RomFile"),
        TypeConverter(typeof(Int32HexTypeConverter))]
        public int FileOffset { get; set; }

        public N64DataElement(int offset, byte[] rawData)
        {
            //Address = new N64Address(address);
            FileOffset = offset;
            RawData = rawData;
        }

        [CategoryAttribute("N64 Element"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Raw data for the N64 Element"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public abstract byte[] RawData { get; set; }

        [CategoryAttribute("N64 Element"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Size in bytes of the raw data"),
        TypeConverter(typeof(Int32HexTypeConverter))]
        public abstract int RawDataSize { get; }

        public bool ContainsOffset(int offset) { return offset >= FileOffset && offset < FileOffset + RawDataSize; }

        [CategoryAttribute("N64 Element"),
        DescriptionAttribute("User defined tag string")]
        public string UserTag { get; set; }

    }
}
