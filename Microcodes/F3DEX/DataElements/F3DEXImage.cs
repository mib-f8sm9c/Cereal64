using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Cereal64.Microcodes.F3DEX.DataElements
{
    public class F3DEXImage
    {
        public Texture Texture { get; private set; }

        public List<Palette> BasePalettes { get; private set; }

        public Palette WorkingPalette { get; private set; }

        public int PaletteOffset { get; set; }

        //Any other information stored by the F3DEX_G_XXXXXX commands needs to be stored in here!!

        public Bitmap Image { get; private set; }

        public bool ValidImage { get; private set; }

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
            Texture = texture;
            BasePalettes = new List<Palette>();
            foreach (Palette palette in palettes)
            {
                BasePalettes.Add(palette);
                palette.Updated -= ImageInfoUpdated;
            }

            PaletteOffset = 0;

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
            Image = null;

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
            }

            //Create the new image from the texture/working palette combo
            Image = GenerateImage();
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
        
    }
}
