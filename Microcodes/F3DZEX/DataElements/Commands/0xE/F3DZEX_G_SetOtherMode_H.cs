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
    public class F3DZEX_G_SetOtherMode_H : F3DZEXCommand
    {
        public enum HMode
        {
            G_MDSFT_BLENDMASK = 0X00,
            G_MDSFT_ALPHADITHER = 0X04,
            G_MDSFT_RGBDITHER = 0X06,
            G_MDSFT_COMBKEY = 0X08,
            G_MDSFT_TEXTCONV = 0X09,
            G_MDSFT_TEXTFILT = 0X0C,
            G_MDSFT_TEXTLUT = 0X0E,
            G_MDSFT_TEXTLOD = 0X10,
            G_MDSFT_TEXTDETAIL = 0X11,
            G_MDSFT_TEXTPERSP = 0X13,
            G_MDSFT_CYCLETYPE = 0X14,
            G_MDSFT_COLORDITHER = 0X16,
            G_MDSFT_PIPELINE = 0X17,
        }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETOTHERMODE_H; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETOTHERMODE_H"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Configure higher half of RDP Other Modes";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Amount data is shifted by, or number of LSb of mode bits to be changed")]
        public HMode Shift { get { return _shift; } set { _shift = value; Updated(); } }
        private HMode _shift;
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Size of data affected, in bits"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort Length { get { return _length; } set { _length = value; Updated(); } }
        private ushort _length;
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("New bit settings to be applied"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint Data { get { return _data; } set { _data = value; Updated(); } } //Enumeration available here
        private uint _data;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_SetOtherMode_H(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)Shift, (byte)(Length - 1),
                    Data);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                Shift = (HMode)ByteHelper.ReadByte(value, 2);
                Length = (ushort)((ushort)ByteHelper.ReadByte(value, 3) + 1);
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
