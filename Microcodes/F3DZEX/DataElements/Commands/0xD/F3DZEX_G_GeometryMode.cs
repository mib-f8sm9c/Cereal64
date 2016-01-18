using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_GeometryMode : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_GEOMETRYMODE; } }

        public string CommandName
        { get { return "G_GEOMETRYMODE"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Configures RSP Geometry Mode"; } }

        public uint ClearBits;
        public uint SetBits;

        public bool IsValid { get; private set; }

        public F3DZEX_G_GeometryMode(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((((uint)CommandID) << 24) | (~ClearBits),
                    SetBits);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ClearBits = ~(ByteHelper.ReadUInt(value, 0) & 0x00FFFFFF);
                SetBits = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
