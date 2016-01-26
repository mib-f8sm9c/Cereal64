using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Mtx : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_MTX; } }

        public string CommandName
        { get { return "G_MTX"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Push new modelview or projection matrix"; } }

        public F3DZEX_G_Mtx_Params Params;
        public uint MatrixAddress;

        public bool IsValid { get; private set; }

        public F3DZEX_G_Mtx(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x38, (byte)0x00, Params.GetAsByte(),
                    MatrixAddress);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                Params = new F3DZEX_G_Mtx_Params(ByteHelper.ReadByte(value, 3));
                MatrixAddress = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }

    public struct F3DZEX_G_Mtx_Params
    {
        public enum G_MTX_PUSH_PARAM
        {
            G_MTX_NOPUSH = 0x00,
            G_MTX_PUSH = 0x01
        }

        public enum G_MTX_LOAD_PARAM
        {
            G_MTX_MUL = 0x00,
            G_MTX_LOAD = 0x02
        }

        public enum G_MTX_VIEW_PARAM
        {
            G_MTX_MODELVIEW = 0x00,
            G_MTX_PROJECTION = 0x04
        }

        public G_MTX_PUSH_PARAM PushParam;
        public G_MTX_LOAD_PARAM LoadParam;
        public G_MTX_VIEW_PARAM ViewParam;

        public F3DZEX_G_Mtx_Params(byte flags)
        {
            PushParam = (G_MTX_PUSH_PARAM)(flags & (byte)G_MTX_PUSH_PARAM.G_MTX_PUSH);
            LoadParam = (G_MTX_LOAD_PARAM)(flags & (byte)G_MTX_LOAD_PARAM.G_MTX_LOAD);
            ViewParam = (G_MTX_VIEW_PARAM)(flags & (byte)G_MTX_VIEW_PARAM.G_MTX_PROJECTION);
        }

        public byte GetAsByte()
        {
            return (byte)((byte)PushParam | (byte)LoadParam | (byte)ViewParam);
        }
    }
}
