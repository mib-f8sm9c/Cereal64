using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Mtx : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_MTX; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_MTX"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Push new modelview or projection matrix";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Parameters controlling nature of matrix addition")]
        public F3DZEX_G_Mtx_Params Params { get { return _params; } set { _params = value; Updated(); } }
        private F3DZEX_G_Mtx_Params _params;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("RAM address of new matrix"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint MatrixAddress { get { return _matrixAddress; } set { _matrixAddress = value; Updated(); } }
        private uint _matrixAddress;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

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
