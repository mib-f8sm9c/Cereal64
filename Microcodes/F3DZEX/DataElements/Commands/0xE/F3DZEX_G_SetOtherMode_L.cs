using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetOtherMode_L : N64DataElement, IF3DZEXCommand
    {
        public enum LMode
        {
            G_MDSFT_ALPHACOMPARE = 0X00,
            G_MDSFT_ZSRCSEL = 0X02,
            G_MDSFT_RENDERMODE = 0X03
        }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETOTHERMODE_L; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public string CommandName
        { get { return "G_SETOTHERMODE_L"; } }
        
        [BrowsableAttribute(false)]
        public string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Configure lower half of RDP Other Modes";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Amount data is shifted by, or number of LSb of mode bits to be changed")]
        public LMode Shift { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Size of data affected, in bits"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort Length { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("New bit settings to be applied"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint Data { get; set; }  //Enumeration available here
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public bool IsValid { get; private set; }

        public F3DZEX_G_SetOtherMode_L(int index, byte[] rawBytes)
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

                Shift = (LMode)ByteHelper.ReadByte(value, 2);
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
