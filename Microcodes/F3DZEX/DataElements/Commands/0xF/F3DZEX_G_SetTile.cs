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
    public class F3DZEX_G_SetTile : F3DZEXCommand
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
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETTILE; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETTILE"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set various parameters of a tile descriptor";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Sets color format")]
        public Texture.ImageFormat Format { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Sets bit size of pixel")]
        public Texture.PixelInfo PixelSize { get; set; }
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Number of 64-bit values per row"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort Line { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Offset of texture in TMEM"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort TMem { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute(" Tile descriptor being modified "),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Tile { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Which palette to use for colors (if relevant)"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Palette { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Mirror flag for the T axis")]
        public TextureMirrorSetting CMTMirror { get; set; }
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Mirror flag for the S axis")]
        public TextureMirrorSetting CMSMirror { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Clamp flag for the S axis")]
        public TextureWrapSetting CMTWrap { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Clamp flag for the T axis")]
        public TextureWrapSetting CMSWrap { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Sets how much of T axis is shown before wrapping")]
        public byte MaskT { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Sets the amount to shift T axis values after perspective division")]
        public byte ShiftT { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Sets how much of S axis is shown before wrapping")]
        public byte MaskS { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Sets the amount to shift S axis values after perspective division")]
        public byte ShiftS { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

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
