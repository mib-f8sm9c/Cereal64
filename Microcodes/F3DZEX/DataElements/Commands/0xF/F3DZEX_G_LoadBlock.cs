﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_LoadBlock : N64DataElement, IF3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_LOADBLOCK; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public string CommandName
        { get { return "G_LOADBLOCK"; } }
        
        [BrowsableAttribute(false)]
        public string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Load texture into TMEM as a continuous stream";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner of texture to load, S-axis")]
        public ushort ULS { get; set; } //10.2 fixed point

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner of texture to load, T-axis")]
        public ushort ULT { get; set; } //10.2 fixed point

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Tile descriptor to load into"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Tile { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Number of texels to load to TMEM, minus one")]
        public ushort Texels { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Change in T-axis per scanline")]
        public ushort DXT { get; set; } //1.11 fixed point
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public bool IsValid { get; private set; }

        public F3DZEX_G_LoadBlock(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)ULS) & 0x0FFF) << 12) |
                                ((uint)ULT) & 0x0FFF);
                uint secondHalf = (uint)(((((uint)Tile) & 0x0F) << 24) |
                                ((((uint)Texels) & 0x0FFF) << 12) |
                                ((uint)DXT) & 0x0FFF);
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ULS = (ushort)(ByteHelper.ReadUShort(value, 1) >> 4);
                ULT = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF);
                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                Texels = (ushort)(ByteHelper.ReadUShort(value, 5) >> 4);
                DXT = (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
