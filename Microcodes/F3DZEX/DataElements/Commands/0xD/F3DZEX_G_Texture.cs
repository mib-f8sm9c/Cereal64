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
    public class F3DZEX_G_Texture : F3DZEXCommand
    {
        //To do: reduce 'on' to a bool

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_TEXTURE; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_TEXTURE"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Enables tile descriptor, sets its scaling factor and mipmap levels";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Maximum number of mipmap levels aside from the first"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Level { get { return _level; } set { _level = value; Updated(); } }
        private byte _level;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Tile descriptor number"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Tile { get { return _tile; } set { _tile = value; Updated(); } }
        private byte _tile;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Decides whether to turn the given texture on or off "),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte TurnOn { get { return _turnOn; } set { _turnOn = value; Updated(); } }
        private byte _turnOn;
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Scaling factor for the S-axis (horizontal)"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort ScaleS { get { return _scaleS; } set { _scaleS = value; Updated(); } }
        private ushort _scaleS;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Scaling factor for the T-axis (vertical)"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort ScaleT { get { return _scaleT; } set { _scaleT = value; Updated(); } }
        private ushort _scaleT;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_Texture(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)Level) & 0x07) << 11) |
                                ((((uint)Tile) & 0x07) << 8) |
                                (uint)(TurnOn));
                return ByteHelper.CombineIntoBytes(firstHalf, ScaleS, ScaleT);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                byte middle = ByteHelper.ReadByte(value, 2);
                Level = (byte)((middle >> 3) & 0x07);
                Tile = (byte)(middle & 0x07);

                TurnOn = ByteHelper.ReadByte(value, 3);
                ScaleS = ByteHelper.ReadUShort(value, 4);
                ScaleT = ByteHelper.ReadUShort(value, 6);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
