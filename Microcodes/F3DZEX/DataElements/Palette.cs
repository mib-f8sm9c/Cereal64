using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cereal64.Common;
using Cereal64.Common.DataElements;
using System.Xml.Linq;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    //Palettes are usually 16-bit RGBA values, either 16 or 256 of them

    public class Palette : N64DataElement
    {
        public Color[] Colors;

        public Palette(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        public Palette(int index, byte[] bytes)
            : base(index, bytes)
        {
            //generate palette
        }

        public override byte[] RawData
        {
            get
            {
                return TextureConversion.PaletteToBinary(Colors);
            }
            set
            {
                //if (value.Length == 32 || value.Length == 512) //Need either of these to be valid
                {
                    Colors = TextureConversion.BinaryToPalette(value, value.Length / 2);
                }
            }
        }

        public override int RawDataSize { get { return 2 * Colors.Length; } }

    }
}
