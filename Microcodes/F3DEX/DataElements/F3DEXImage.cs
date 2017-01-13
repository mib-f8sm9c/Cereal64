using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DEX.DataElements
{
    public class F3DEXImage : IXMLSerializable
    {
        public const string F3DEXIMAGE = "F3DEXImage";
        private const string TEXTURE = "Texture";
        private const string TEXTURE_DATA = "TextureData";
        private const string PALETTES = "Palettes";
        private const string PALETTE = "Palette";
        private const string PALETTE_DATA = "PaletteData";
        private const string PALETTE_OFFSET = "PaletteOffset";
        private const string EXISTING_PALETTE = "ExistingPalette";

        public Texture Texture { get; private set; }

        public List<Palette> BasePalettes { get; private set; }

        public Palette WorkingPalette { get; private set; }

        public int PaletteOffset { get; set; }

        //Any other information stored by the F3DEX_G_XXXXXX commands needs to be stored in here!!

        public Bitmap Image
        {
            get
            {
                //Create the bitmap only when it needs to be created, to save on memory
                if (_imageNeedsUpdating)
                {
                    Image = GenerateImage();
                }

                return _image;
            }
            private set //Only use post-calculation of image, please
            {
                _image = value;
                ValidImage = (_image != null);
                _imageNeedsUpdating = false;
            }
        }
        private Bitmap _image;

        public bool ValidImage { get; private set; }

        private bool _imageNeedsUpdating = true;

        public F3DEXImage(Texture texture)
            : this(texture, new List<Palette>())
        {
        }

        public F3DEXImage(Texture texture, Palette palette)
            : this(texture, new List<Palette>() { palette })
        {
        }

        public F3DEXImage(Texture texture, IList<Palette> palettes)
        {
            SetupImage(texture, palettes, 0);
        }

        public F3DEXImage(XElement xml)
        {
            XElement textureXml = xml.Element(TEXTURE);
            XElement palettesXml = xml.Element(PALETTES);

            Texture texture = null;
            List<Palette> palettes = new List<Palette>();

            byte[] textureData = Convert.FromBase64String(textureXml.Attribute(TEXTURE_DATA).Value.ToString());
            texture = new Texture(textureXml.Element(typeof(Texture).ToString()), textureData);

            foreach (XElement paletteXml in palettesXml.Elements())
            {
                byte[] paletteData = Convert.FromBase64String(paletteXml.Attribute(PALETTE_DATA).Value.ToString());
                Palette palette = new Palette(paletteXml.Element(typeof(Palette).ToString()), paletteData);
                palettes.Add(palette);
            }

            int paletteOffset = int.Parse(palettesXml.Attribute(PALETTE_OFFSET).Value.ToString());

            SetupImage(texture, palettes, paletteOffset);
        }

        public F3DEXImage(XElement xml, Palette existingPalette)
        {
            //Specialized constructor for MK64 to help reduce filesize on repeated palettes

            XElement textureXml = xml.Element(TEXTURE);
            XElement palettesXml = xml.Element(PALETTES);

            Texture texture = null;
            List<Palette> palettes = new List<Palette>();

            byte[] textureData = Convert.FromBase64String(textureXml.Attribute(TEXTURE_DATA).Value.ToString());
            texture = new Texture(textureXml.Element(typeof(Texture).ToString()), textureData);

            foreach (XElement paletteXml in palettesXml.Elements())
            {
                if (paletteXml.Name.ToString() == EXISTING_PALETTE)
                {
                    palettes.Add(existingPalette);
                }
                else
                {
                    byte[] paletteData = Convert.FromBase64String(paletteXml.Attribute(PALETTE_DATA).Value.ToString());
                    Palette palette = new Palette(paletteXml.Element(typeof(Palette).ToString()), paletteData);
                    palettes.Add(palette);
                }
            }

            int paletteOffset = int.Parse(palettesXml.Attribute(PALETTE_OFFSET).Value.ToString());

            SetupImage(texture, palettes, paletteOffset);
        }

        private void SetupImage(Texture texture, IList<Palette> palettes, int paletteOffset)
        {
            Texture = texture;
            BasePalettes = new List<Palette>();
            foreach (Palette palette in palettes)
            {
                BasePalettes.Add(palette);
                palette.Updated += ImageInfoUpdated;
            }

            PaletteOffset = paletteOffset;

            texture.Updated += ImageInfoUpdated;

            UpdateImage();
        }

        //Need to make texture/palette IUpdateables so they can inform this image class (and MK64Image) that they need to update!
        public void ImageInfoUpdated()
        {
            UpdateImage();
        }

        public void UpdateImage()
        {
            ValidImage = false;
            _image = null;
            _imageNeedsUpdating = true;

            if (BasePalettes.Count == 0)
                WorkingPalette = null;
            else
            {
                //Combine into a new palette
                byte[] newPaletteInfo = new byte[BasePalettes.Sum(p => p.RawDataSize)];
                int offset = 0;
                foreach (Palette p in BasePalettes)
                {
                    Array.Copy(p.RawData, 0, newPaletteInfo, offset, p.RawDataSize);
                    offset += p.RawDataSize;
                }

                WorkingPalette = new Palette(-1, newPaletteInfo);
            }

            //Create the new image from the texture/working palette combo
            //Image = GenerateImage();
            //ValidImage = (Image != null);

            ValidImage = CheckImageValidity();
        }

        private bool CheckImageValidity()
        {
            if (Texture == null || (Texture.Format == DataElements.Texture.ImageFormat.CI && WorkingPalette == null))
                return false;

            //Check texture size
            double multiplier = 1;
            switch(Texture.PixelSize)
            {
                case DataElements.Texture.PixelInfo.Size_4b:
                    multiplier = 0.5;
                    break;
                case DataElements.Texture.PixelInfo.Size_16b:
                    multiplier = 2;
                    break;
                case DataElements.Texture.PixelInfo.Size_32b:
                    multiplier = 4;
                    break;
            }
            int dataCount = (int)Math.Round(Texture.RawDataSize / multiplier);

            if(dataCount != Texture.Width * Texture.Height)
                return false;

            //Test pixel size & format compatibility
            switch (Texture.Format)
            {
                case Texture.ImageFormat.RGBA:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_16b:
                            return true;

                        case Texture.PixelInfo.Size_32b:
                            return true;

                    }
                    break;
                case Texture.ImageFormat.YUV:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_16b:
                            return false;
                            //return TextureConversion.BinaryToYUV16(bytes, Width, Height);
                    }
                    break;
                case Texture.ImageFormat.CI:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_4b:
                            return true;

                        case Texture.PixelInfo.Size_8b:
                            return true;

                    }
                    break;
                case Texture.ImageFormat.IA:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_4b:
                            return true;

                        case Texture.PixelInfo.Size_8b:
                            return true;

                        case Texture.PixelInfo.Size_16b:
                            return true;

                    }
                    break;
                case Texture.ImageFormat.I:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_4b:
                            return true;

                        case Texture.PixelInfo.Size_8b:
                            return true;

                    }
                    break;
            }

            return false;
        }

        private Bitmap GenerateImage()
        {
            if (Texture == null || (Texture.Format == DataElements.Texture.ImageFormat.CI && WorkingPalette == null))
                return null;

            switch (Texture.Format)
            {
                case Texture.ImageFormat.RGBA:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_16b:
                            return TextureConversion.BinaryToRGBA16(Texture.RawData, Texture.Width, Texture.Height);

                        case Texture.PixelInfo.Size_32b:
                            return TextureConversion.BinaryToRGBA32(Texture.RawData, Texture.Width, Texture.Height);

                    }
                    break;
                case Texture.ImageFormat.YUV:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_16b:
                            return null;
                            //return TextureConversion.BinaryToYUV16(bytes, Width, Height);
                    }
                    break;
                case Texture.ImageFormat.CI:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_4b:
                            return TextureConversion.BinaryToCI4(Texture.RawData, WorkingPalette, PaletteOffset, Texture.Width, Texture.Height);

                        case Texture.PixelInfo.Size_8b:
                            return TextureConversion.BinaryToCI8(Texture.RawData, WorkingPalette, PaletteOffset, Texture.Width, Texture.Height);

                    }
                    break;
                case Texture.ImageFormat.IA:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_4b:
                            return TextureConversion.BinaryToIA4(Texture.RawData, Texture.Width, Texture.Height);

                        case Texture.PixelInfo.Size_8b:
                            return TextureConversion.BinaryToIA8(Texture.RawData, Texture.Width, Texture.Height);

                        case Texture.PixelInfo.Size_16b:
                            return TextureConversion.BinaryToIA16(Texture.RawData, Texture.Width, Texture.Height);

                    }
                    break;
                case Texture.ImageFormat.I:
                    switch (Texture.PixelSize)
                    {
                        case Texture.PixelInfo.Size_4b:
                            return TextureConversion.BinaryToI4(Texture.RawData, Texture.Width, Texture.Height);

                        case Texture.PixelInfo.Size_8b:
                            return TextureConversion.BinaryToI8(Texture.RawData, Texture.Width, Texture.Height);

                    }
                    break;
            }

            return null;
        }

        public XElement GetAsXML()
        {
            return this.GetAsXML(null);
        }

        public XElement GetAsXML(Palette existingPalette)
        {
            //Specialized function for MK64, exclude the existing palette data when writing data out
            // to the xml.

            //Add the texture xml and each palette xml
            XElement xml = new XElement(F3DEXIMAGE);

            XElement textureXml = new XElement(TEXTURE);
            textureXml.Add(new XAttribute(TEXTURE_DATA, Convert.ToBase64String(Texture.RawData)));
            textureXml.Add(Texture.GetAsXML());

            xml.Add(textureXml);

            XElement palettesXML = new XElement(PALETTES);

            foreach (Palette palette in BasePalettes)
            {
                if (palette == existingPalette) //existing palette is defined elsewhere, so just use that
                {
                    palettesXML.Add(new XElement(EXISTING_PALETTE));
                }
                else
                {
                    XElement paletteXML = new XElement(PALETTE);
                    paletteXML.Add(new XAttribute(PALETTE_DATA, Convert.ToBase64String(palette.RawData)));
                    paletteXML.Add(palette.GetAsXML());

                    palettesXML.Add(paletteXML);
                }
            }

            palettesXML.Add(new XAttribute(PALETTE_OFFSET, PaletteOffset));
            xml.Add(palettesXML);

            return xml;
        }
    }
}
