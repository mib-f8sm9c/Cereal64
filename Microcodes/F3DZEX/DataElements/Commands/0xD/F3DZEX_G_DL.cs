﻿using System;
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
    public class F3DZEX_G_DL : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_DL; } }

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "F3DZEX_G_DL"; } }

        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Jump or \"call\" another display list";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Force the DL stack to reset")]
        public bool ForceJump { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Ram address for the new display list")]
        public DmaAddress DLAddress { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_DL(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (ForceJump ? (byte)0x01 : (byte)0x00), (byte)0x00, (byte)0x00,
                    DLAddress.GetAsUInt());
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ForceJump = (ByteHelper.ReadByte(value, 1) & 0x01) == 0x01;
                DLAddress = new DmaAddress(ByteHelper.ReadUInt(value, 4));

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
