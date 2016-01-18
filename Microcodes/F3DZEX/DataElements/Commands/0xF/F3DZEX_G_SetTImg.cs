using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetTImg : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETTIMG; } }

        public string CommandName
        { get { return "G_SETTIMG"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set location of texture image in RAM"; } }

        public Texture.ImageFormat Format;
        public Texture.PixelInfo PixelSize;
        public ushort Width;
        public uint ImageAddress;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetTImg(int index, byte[] rawBytes)
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
                return ByteHelper.CombineIntoBytes(firstHalf, ImageAddress);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                Format = (Texture.ImageFormat)(ByteHelper.ReadByte(value, 1) >> 5);
                PixelSize = (Texture.PixelInfo)((ByteHelper.ReadByte(value, 1) >> 3) & 0x03);
                Width = (ushort)((ByteHelper.ReadUShort(value, 2) & 0x0FFF) + 1);
                ImageAddress = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
