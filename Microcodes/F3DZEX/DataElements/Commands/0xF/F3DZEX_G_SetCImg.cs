using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;
using Cereal64.Common.Rom;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetCImg : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETCIMG; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETCIMG"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set location of color buffer";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Format of color buffer to be pointed to")]
        public Texture.ImageFormat Format { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Bit size of pixels in color buffer to be pointed to")]
        public Texture.PixelInfo PixelSize { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Width of the color buffer")]
        public ushort Width { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("RAM address of color buffer")]
        public DmaAddress ImageAddress { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_SetCImg(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)Format) & 0x07) << 21) |
                                ((((uint)PixelSize) & 0x03) << 19) |
                                ((uint)Width - 1) & 0x0FFF);
                return ByteHelper.CombineIntoBytes(firstHalf, ImageAddress.GetAsUInt());
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                Format = (Texture.ImageFormat)(ByteHelper.ReadByte(value, 1) >> 5);
                PixelSize = (Texture.PixelInfo)((ByteHelper.ReadByte(value, 1) >> 3) & 0x03);
                Width = (ushort)((ByteHelper.ReadUShort(value, 2) & 0x0FFF) + 1);
                ImageAddress = new DmaAddress(ByteHelper.ReadUInt(value, 4));

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
