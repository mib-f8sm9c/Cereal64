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
    public class F3DZEX_G_CullDL :F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_CULLDL; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_CULLDL"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "End display list if object is offscreen";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of first vertex for bounding volume"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort BufferIndexStart { get { return _bufferIndexStart; } set { _bufferIndexStart = value; Updated(); } }
        private ushort _bufferIndexStart;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of last vertex for bounding volume"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort BufferIndexEnd { get { return _bufferIndexEnd; } set { _bufferIndexEnd = value; Updated(); } }
        private ushort _bufferIndexEnd;

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

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
