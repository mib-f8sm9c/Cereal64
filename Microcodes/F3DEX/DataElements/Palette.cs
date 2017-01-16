using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cereal64.Common;
using Cereal64.Common.DataElements;
using System.Xml.Linq;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DEX.DataElements
{
    //Palettes are usually 16-bit RGBA values, either 16 or 256 of them

    public class Palette : N64DataElement, IUpdatable
    {
        [CategoryAttribute("Palette Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("All colors used in the palette")]
        public Color[] Colors { get; set; }

        public Palette(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        public Palette(int index, byte[] bytes)
            : base(index, bytes)
        {
        }

        public Palette Combine(Palette secondPalette)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(RawData);
            bytes.AddRange(secondPalette.RawData);
            return new Palette(-1, bytes.ToArray());
        }

        public Palette Duplicate()
        {
            return new Palette(-1, RawData);
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

        public UpdateEvent Updated { get { return _updated; } set { _updated = value; } }

        private UpdateEvent _updated = delegate { };

    }
}
