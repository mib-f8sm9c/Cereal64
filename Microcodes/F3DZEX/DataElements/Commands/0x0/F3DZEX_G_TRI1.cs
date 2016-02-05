using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Tri1 : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_TRI1; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_TRI1"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Draw a triangle";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of first vertex of the triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex1 { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of second vertex of the triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex2 { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of third vertex of the triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex3 { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_Tri1(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)(Vertex1 * 2), 
                    (byte)(Vertex2 * 2), (byte)(Vertex3 * 2),
                    (uint)0x00000000);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Vertex1 = (byte)(ByteHelper.ReadByte(value, 1) / 2);
                Vertex2 = (byte)(ByteHelper.ReadByte(value, 2) / 2);
                Vertex3 = (byte)(ByteHelper.ReadByte(value, 3) / 2);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
