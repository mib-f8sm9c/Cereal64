﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_RDPPipeSync : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_RDPPIPESYNC; } }

        public string CommandName
        { get { return "G_RDPPIPESYNC"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Synchronize with rendering to safely update RDP attributes. Does not indicate start of display list!"; } }

        public bool IsValid { get; private set; }

        public F3DZEX_G_RDPPipeSync(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (ushort)0x0000,
                    (uint)0x00000000);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
