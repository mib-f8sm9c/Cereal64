﻿using System;
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
    public class F3DEX_G_SetZImg : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_SETZIMG; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETZIMG"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set location of depth buffer";
        
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Address for the depth buffer")]
        public DmaAddress ImageAddress { get { return _imageAddress; } set { _imageAddress = value; Updated(); } }
        private DmaAddress _imageAddress;
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_SetZImg(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    ImageAddress.GetAsUInt());
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

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
