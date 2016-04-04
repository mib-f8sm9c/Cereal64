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
    public class F3DZEX_G_Tri2 : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_TRI2; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_TRI2"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Draw two triangles";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of first vertex of the first triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex1 { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of second vertex of the first triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex2 { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of third vertex of the first triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex3 { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of first vertex of the second triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex4 { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of second vertex of the second triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex5 { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of third vertex of the second triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex6 { get; set; }

        [BrowsableAttribute(false)]
        public Texture TextureReference { get; set; }

        [BrowsableAttribute(false)]
        public Vertex Vertex1Reference { get; set; }

        [BrowsableAttribute(false)]
        public Vertex Vertex2Reference { get; set; }

        [BrowsableAttribute(false)]
        public Vertex Vertex3Reference { get; set; }

        [BrowsableAttribute(false)]
        public Vertex Vertex4Reference { get; set; }

        [BrowsableAttribute(false)]
        public Vertex Vertex5Reference { get; set; }

        [BrowsableAttribute(false)]
        public Vertex Vertex6Reference { get; set; }


        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_Tri2(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)(Vertex1 * 2),
                    (byte)(Vertex2 * 2), (byte)(Vertex3 * 2),
                    (byte)0x00, (byte)(Vertex4 * 2), (byte)(Vertex5 * 2), 
                    (byte)(Vertex6 * 2));
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Vertex1 = (byte)(ByteHelper.ReadByte(value, 1) / 2);
                Vertex2 = (byte)(ByteHelper.ReadByte(value, 2) / 2);
                Vertex3 = (byte)(ByteHelper.ReadByte(value, 3) / 2);
                //0x00
                Vertex4 = (byte)(ByteHelper.ReadByte(value, 5) / 2);
                Vertex5 = (byte)(ByteHelper.ReadByte(value, 6) / 2);
                Vertex6 = (byte)(ByteHelper.ReadByte(value, 7) / 2);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
