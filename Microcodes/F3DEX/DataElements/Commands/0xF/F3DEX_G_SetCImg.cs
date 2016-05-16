using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;
using Cereal64.Common.Rom;

namespace Cereal64.Microcodes.F3DEX.DataElements.Commands
{
    public class F3DEX_G_SetCImg : F3DEXCommand
    {
        [CategoryAttribute("F3DX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_SETCIMG; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETCIMG"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set location of color buffer";
            
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Format of color buffer to be pointed to")]
        public Texture.ImageFormat Format { get { return _format; } set { _format = value; Updated(); } }
        private Texture.ImageFormat _format;
        
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Bit size of pixels in color buffer to be pointed to")]
        public Texture.PixelInfo PixelSize { get { return _pixelSize; } set { _pixelSize = value; Updated(); } }
        private Texture.PixelInfo _pixelSize;
        
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Width of the color buffer")]
        public ushort Width { get { return _width; } set { _width = value; Updated(); } }
        private ushort _width;
        
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("RAM address of color buffer")]
        public DmaAddress ImageAddress { get { return _imageAddress; } set { _imageAddress = value; Updated(); } }
        private DmaAddress _imageAddress;
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_SetCImg(int index, byte[] rawBytes)
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
