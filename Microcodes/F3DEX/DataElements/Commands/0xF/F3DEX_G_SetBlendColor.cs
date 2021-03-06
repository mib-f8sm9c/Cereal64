﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using System.Drawing;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DEX.DataElements.Commands
{
    public class F3DEX_G_SetBlendColor : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_SETBLENDCOLOR; } }

        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETBLENDCOLOR"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set blend color register";
            
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Blend color to be set")]
        public Color BlendColor { get { return _blendColor; } set { _blendColor = value; Updated(); } }
        private Color _blendColor;
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_SetBlendColor(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    BlendColor.R, BlendColor.G, BlendColor.B, BlendColor.A);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                byte R = ByteHelper.ReadByte(value, 4);
                byte G = ByteHelper.ReadByte(value, 5);
                byte B = ByteHelper.ReadByte(value, 6);
                byte A = ByteHelper.ReadByte(value, 7);

                BlendColor = Color.FromArgb(A, R, G, B);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
