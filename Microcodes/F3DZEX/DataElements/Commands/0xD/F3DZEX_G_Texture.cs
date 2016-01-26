using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Texture : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_TEXTURE; } }

        public string CommandName
        { get { return "G_TEXTURE"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Enables tile descriptor, sets its scaling factor and mipmap levels"; } }

        public byte Level;
        public byte Tile;
        public byte TurnOn;
        public ushort ScaleS;
        public ushort ScaleT;

        public bool IsValid { get; private set; }

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
