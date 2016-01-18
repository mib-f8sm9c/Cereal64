﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Branch_Z : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_BRANCH_Z; } }

        public string CommandName
        { get { return "G_BRANCH_Z"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Branch to another display list if vertex is close enough to screen"; } }

        public ushort BufferIndex1;
        public ushort BufferIndex2;
        public uint ZVal;

        public bool IsValid { get; private set; }

        public F3DZEX_G_Branch_Z(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)BufferIndex1 * 5) & 0x0FFF) << 12) |
                                ((uint)(BufferIndex2 * 2) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, ZVal);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                BufferIndex1 = (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) / 5);
                BufferIndex2 = (ushort)((ByteHelper.ReadUShort(value, 2) & 0xFFF) / 2);
                ZVal = ByteHelper.ReadUInt(value, 4);

                if (BufferIndex1 != BufferIndex2)
                    return; //Invalid, the two indices need to be equal

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
