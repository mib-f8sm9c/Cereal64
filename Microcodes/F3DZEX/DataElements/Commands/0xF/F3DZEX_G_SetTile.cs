using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetTile : N64DataElement, IF3DZEXCommand
    {
        public enum TextureMirrorSetting
        {
            G_TX_NOMIRROR = 0x00,
            G_TX_MIRROR = 0x01
        }

        public enum TextureWrapSetting
        {
            G_TX_WRAP = 0x00,
            G_TX_CLAMP = 0x02
        }

        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETTILE; } }

        public string CommandName
        { get { return "G_SETTILE"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set various parameters of a tile descriptor"; } }

        public Texture.ImageFormat Format;
        public Texture.PixelInfo PixelSize;
        public ushort Line, TMem;
        public byte Tile, Palette;
        public TextureMirrorSetting CMTMirror, CMSMirror;
        public TextureWrapSetting CMTWrap, CMSWrap;
        public byte MaskT, ShiftT, MaskS, ShiftS;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetTile(int index, byte[] rawBytes)
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
                                ((((uint)Line) & 0x1FF) << 9) |
                                (((uint)TMem) & 0x1FF));
                uint secondHalf = (uint)(((((uint)Tile) & 0x07) << 24) |
                                ((((uint)Palette) & 0x0F) << 20) |
                                ((((uint)CMTMirror | (uint)CMTWrap) & 0x03) << 18) |
                                ((((uint)MaskT) & 0x0F) << 14) |
                                ((((uint)ShiftT) & 0x0F) << 10) |
                                ((((uint)CMSMirror | (uint)CMSWrap) & 0x03) << 8) |
                                ((((uint)MaskS) & 0x0F) << 4) |
                                (((uint)ShiftS) & 0x0F));
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                byte tempByte = ByteHelper.ReadByte(value, 1);
                Format = (Texture.ImageFormat)((tempByte >> 5) & 0x07);
                PixelSize = (Texture.PixelInfo)((tempByte >> 3) & 0x03);
                Line = (ushort)((ByteHelper.ReadUShort(value, 1) >> 1) & 0x01FF);
                TMem = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x01FF);

                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x07);
                Palette = (byte)((ByteHelper.ReadByte(value, 5) >> 4) & 0x0F);
                ushort temp = ByteHelper.ReadUShort(value, 5);
                tempByte = (byte)((temp >> 10) & 0x03);
                CMTMirror = (TextureMirrorSetting)(tempByte & 0x01);
                CMTWrap = (TextureWrapSetting)(tempByte & 0x02);
                MaskT = (byte)((temp >> 6) & 0x0F);
                ShiftT = (byte)((temp >> 2) & 0x0F);
                tempByte = (byte)(temp & 0x03);
                CMSMirror = (TextureMirrorSetting)(tempByte & 0x01);
                CMSWrap = (TextureWrapSetting)(tempByte & 0x02);
                tempByte = ByteHelper.ReadByte(value, 7);
                MaskS = (byte)((tempByte >> 4) & 0x0F);
                ShiftS = (byte)(tempByte & 0x0F);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
