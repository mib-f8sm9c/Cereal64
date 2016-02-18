﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetKeyGB : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETKEYGB; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETKEYGB"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set green and blue components of chroma key";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Scaled width of half the key window for green")]
        public qushort WidthG; //Actually 4.8 float

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Scaled width of half the key window for blue")]
        public qushort WidthB; //Actually 4.8 float

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Intensity of active key for green"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte CenterG;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Reciprocal of size of soft edge, normalized to 0..0xFF, for green"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte ScaleG;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Intensity of active key for blue"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte CenterB;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Reciprocal of size of soft edge, normalized to 0..0xFF, for blue"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte ScaleB;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_SetKeyGB(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)WidthG.RawValue) & 0x0FFF) << 12) |
                                (((uint)WidthB.RawValue) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, 
                    CenterG, ScaleG, CenterB, ScaleB);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                WidthG = new qushort("4.8", (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) & 0x0FFF));
                WidthB = new qushort("4.8", (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF));

                CenterG = ByteHelper.ReadByte(value, 4);
                ScaleG = ByteHelper.ReadByte(value, 5);
                CenterB = ByteHelper.ReadByte(value, 6);
                ScaleB = ByteHelper.ReadByte(value, 7);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
