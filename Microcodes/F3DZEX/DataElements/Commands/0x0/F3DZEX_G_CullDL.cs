using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_CullDL : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_CULLDL; } }

        public string CommandName
        { get { return "G_CULLDL"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "End display list if object is offscreen"; } }

        public ushort BufferIndexStart;
        public ushort BufferIndexEnd;

        public bool IsValid { get; private set; }

        public F3DZEX_G_CullDL(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (ushort)(BufferIndexStart * 2),
                    (byte)0x00, (byte)0x00, (ushort)(BufferIndexEnd * 2));
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                BufferIndexStart = (ushort)(ByteHelper.ReadUShort(value, 2) / 2);
                BufferIndexEnd = (ushort)(ByteHelper.ReadUShort(value, 6) / 2);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
