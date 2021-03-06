﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DEX.DataElements.Commands
{
    public class F3DEX_G_TexRectFlip : F3DEXCommand
    {
        //THIS COMMAND IS FOLLOWED BY 0xE1 AND 0xF1 COMMANDS THAT PROVIDE EXTRA DATA
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_TEXRECTFLIP; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_TEXRECTFLIP"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Draw textured rectangle onto screen, with flipped texture axes";

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Lower-right corner X coordinate ")]
        public ushort LRX { get { return _lrx; } set { _lrx = value; Updated(); } }
        private ushort _lrx;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Lower-right corner Y coordinate ")]
        public ushort LRY { get { return _lry; } set { _lry = value; Updated(); } }
        private ushort _lry;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Tile descriptor to use for rectangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Tile { get { return _tile; } set { _tile = value; Updated(); } }
        private byte _tile;
        
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Upper-left corner X coordinate ")]
        public ushort ULX { get { return _ulx; } set { _ulx = value; Updated(); } }
        private ushort _ulx;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Upper-left corner Y coordinate ")]
        public ushort ULY { get { return _uly; } set { _uly = value; Updated(); } }
        private ushort _uly;
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_TexRectFlip(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {

                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)LRX) & 0x0FFF) << 12) |
                                (((uint)LRY) & 0x0FFF));
                uint secondHalf = (uint)((uint)(Tile & 0x0F) << 24 |
                                ((((uint)ULX) & 0x0FFF) << 12) |
                                (((uint)ULY) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                LRX = (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) & 0x0FFF);
                LRY = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF);

                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                ULX = (ushort)((ByteHelper.ReadUShort(value, 5) >> 4) & 0x0FFF);
                ULY = (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
