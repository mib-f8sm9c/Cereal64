﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Quad : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_QUAD; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_QUAD"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Draw quadrilateral (deprecated)";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of first vertex of the quadrangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex1 { get { return _vertex1; } set { _vertex1 = value; Updated(); } }
        private byte _vertex1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of second vertex of the quadrangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex2 { get { return _vertex2; } set { _vertex2 = value; Updated(); } }
        private byte _vertex2;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of third vertex of the quadrangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex3 { get { return _vertex3; } set { _vertex3 = value; Updated(); } }
        private byte _vertex3;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of fourth vertex of the quadrangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex4 { get { return _vertex4; } set { _vertex4 = value; Updated(); } }
        private byte _vertex4;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_Quad(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)(Vertex1 * 2),
                    (byte)(Vertex2 * 2), (byte)(Vertex3 * 2),
                    (byte)0x00, (byte)(Vertex1 * 2), (byte)(Vertex3 * 2),
                    (byte)(Vertex4 * 2));
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Vertex1 = (byte)(ByteHelper.ReadByte(value, 1) / 2);
                Vertex2 = (byte)(ByteHelper.ReadByte(value, 2) / 2);
                Vertex3 = (byte)(ByteHelper.ReadByte(value, 3) / 2);
                //0x00
                if (Vertex1 != (byte)(ByteHelper.ReadByte(value, 5) / 2)) return;
                if (Vertex3 != (byte)(ByteHelper.ReadByte(value, 6) / 2)) return;
                Vertex4 = (byte)(ByteHelper.ReadByte(value, 7) / 2);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
