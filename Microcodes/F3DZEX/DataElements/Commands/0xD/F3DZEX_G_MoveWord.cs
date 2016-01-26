using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_MoveWord : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_MOVEWORD; } }

        public string CommandName
        { get { return "G_MOVEWORD"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Change \"word\" (32 bits) of data in DMEM"; } }

        public enum G_MW_INDEX
        {
            G_MW_MATRIX = 0x00,
            G_MW_NUMLIGHT = 0x02,
            G_MW_CLIP = 0x04,
            G_MW_SEGMENT = 0x06,
            G_MW_FOG = 0x08,
            G_MW_LIGHTCOL = 0x0A,
            G_MW_FORCEMTX = 0x0C,
            G_MW_PERSPNORM = 0x0E,
        }

        public G_MW_INDEX WordIndex;
        public ushort Offset;
        public uint Data;

        public bool IsValid { get; private set; }

        public F3DZEX_G_MoveWord(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)WordIndex, Offset,
                    Data);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                WordIndex = (G_MW_INDEX)ByteHelper.ReadByte(value, 1);
                Offset = ByteHelper.ReadUShort(value, 2);
                Data = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
