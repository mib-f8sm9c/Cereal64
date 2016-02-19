using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cereal64.Common;
using Cereal64.Common.DataElements;
using System.Xml.Linq;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public class Texture : N64DataElement
    {
        private const string FORMAT = "Format";
        private const string PIXEL_SIZE = "PixelSize";
        private const string WIDTH = "Width";
        private const string HEIGHT = "Height";
        private const string IMAGE_PALETTE = "ImagePalette";
        private const string PALETTE_INDEX = "PaletteIndex";

        public enum ImageFormat
        {
            RGBA = 0,
            YUV = 1,
            CI = 2,
            IA = 3,
            I = 4
        }

        public enum PixelInfo
        {
            Size_4b,
            Size_8b,
            Size_16b,
            Size_32b
        }

        [CategoryAttribute("Texture Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Format of the texture")]
        public ImageFormat Format { get; set; }

        [CategoryAttribute("Texture Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Size in bits of each pixel of the texture")]
        public PixelInfo PixelSize { get; set; }

        [CategoryAttribute("Texture Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Width in pixels of the texture")]
        public int Width { get; set; }

        [CategoryAttribute("Texture Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Height in pixels of the texture")]
        public int Height { get; set; }

        [CategoryAttribute("Texture Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Palette used for texture (available only for format CI)")]
        public Palette ImagePalette
        {
            get { return _imagePalette; }
            set { _imagePalette = value; if (_unconvertedData != null) RawData = _unconvertedData; } //Try reloading the data if it failed previously
        }
        private Palette _imagePalette;

        [CategoryAttribute("Texture Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Height in pixels of the texture")]
        public int PaletteIndex { get; set; } //Used for CI4b

        [CategoryAttribute("Texture Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Texture in final bitmap form")]
        public Bitmap Image { get; private set; }

        private bool _initializing = true;
        public Guid MatchedPaletteGUID; //Used to link the palette to the texture
        private byte[] _unconvertedData;

        [CategoryAttribute("Texture Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the texture pixel size/format are in an N64-compatible state")]
        public bool IsValidFormat
        {
            get
            {
                switch (Format)
                {
                    case ImageFormat.RGBA:
                        if(PixelSize == PixelInfo.Size_16b || PixelSize == PixelInfo.Size_32b)
                            return true;
                        break;
                    case ImageFormat.YUV:
                        if(PixelSize == PixelInfo.Size_16b)
                            return true;
                        break;
                    case ImageFormat.CI:
                        if(PixelSize == PixelInfo.Size_4b || PixelSize == PixelInfo.Size_8b)
                            return true;
                        break;
                    case ImageFormat.IA:
                        if(PixelSize == PixelInfo.Size_4b || PixelSize == PixelInfo.Size_8b ||
                            PixelSize == PixelInfo.Size_16b)
                            return true;
                        break;
                    case ImageFormat.I:
                        if(PixelSize == PixelInfo.Size_4b || PixelSize == PixelInfo.Size_8b)
                            return true;
                        break;
                }
                return false;
            }
        }

        public Texture(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
            //More information needed to be saved/loaded
            Format = (ImageFormat)Enum.Parse(typeof(ImageFormat), xml.Attribute(FORMAT).Value);
            PixelSize = (PixelInfo)Enum.Parse(typeof(PixelInfo), xml.Attribute(PIXEL_SIZE).Value);
            Width = int.Parse(xml.Attribute(WIDTH).Value);
            Height = int.Parse(xml.Attribute(HEIGHT).Value);

            XAttribute att = xml.Attribute(IMAGE_PALETTE);
            if (att != null)
                MatchedPaletteGUID = new Guid(att.Value);
            else
                MatchedPaletteGUID = Guid.Empty;

            PaletteIndex = int.Parse(xml.Attribute(PALETTE_INDEX).Value);

            //generate image
            _initializing = false;
            RawData = _unconvertedData;
        }

        public Texture(int index, byte[] bytes, ImageFormat format = ImageFormat.RGBA,
            PixelInfo pixel = PixelInfo.Size_32b, int width = 0, int height = 0, Palette palette = null,
            int paletteIndex = 0)
            : base(index, bytes)
        {
            Format = format;
            PixelSize = pixel;
            Width = width;
            Height = height;

            ImagePalette = palette;
            PaletteIndex = paletteIndex;
            
            //generate image
            _initializing = false;
            RawData = bytes;
        }

        public override byte[] RawData
        {
            get
            {
                return ConvertImage();
            }
            set
            {
                if (_initializing) //Wait til more info (width, height) comes in
                {
                    _unconvertedData = value;
                    return;
                }

                if (IsValidFormat)
                {
                    //generate image
                    Image = GenerateImage(value);

                    if(Image == null)
                        _unconvertedData = value;
                    else
                        _unconvertedData = null;
                }
                else
                {
                    _unconvertedData = value; //Save for later
                }
            }
        }

        public override int RawDataSize 
        { 
            get 
            {
                int length = 0;

                if (Image != null)
                {
                    length = Image.Width * Image.Height;

                    switch (PixelSize)
                    {
                        case PixelInfo.Size_4b:
                            length /= 2;
                            break;
                        case PixelInfo.Size_16b:
                            length *= 2;
                            break;
                        case PixelInfo.Size_32b:
                            length *= 4;
                            break;
                    }
                }

                return length;
            }
        }

        public override XElement GetAsXML()
        {
            XElement baseElement = base.GetAsXML();

            baseElement.Add(new XAttribute(FORMAT, Format));
            baseElement.Add(new XAttribute(PIXEL_SIZE, PixelSize));
            baseElement.Add(new XAttribute(WIDTH, Width));
            baseElement.Add(new XAttribute(HEIGHT, Height));
            
            if(ImagePalette != null)
                baseElement.Add(new XAttribute(IMAGE_PALETTE, ImagePalette.GUID.ToString()));
            baseElement.Add(new XAttribute(PALETTE_INDEX, PaletteIndex));

            return baseElement;
        }

        private byte[] ConvertImage()
        {
            if (Image == null)
                return new byte[0];

            switch (Format)
            {
                case ImageFormat.RGBA:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_16b:
                            return TextureConversion.RGBA16ToBinary(Image);

                        case PixelInfo.Size_32b:
                            return TextureConversion.RGBA32ToBinary(Image);

                    }
                    break;
                case ImageFormat.YUV:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_16b:
                            return null;
                            //return TextureConversion.YUV16ToBinary(_image);
                    }
                    break;
                case ImageFormat.CI:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_4b:
                            return TextureConversion.CI4ToBinary(Image, ImagePalette, PaletteIndex);

                        case PixelInfo.Size_8b:
                            return TextureConversion.CI8ToBinary(Image, ImagePalette);

                    }
                    break;
                case ImageFormat.IA:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_4b:
                            return TextureConversion.IA4ToBinary(Image);

                        case PixelInfo.Size_8b:
                            return TextureConversion.IA8ToBinary(Image);

                        case PixelInfo.Size_16b:
                            return TextureConversion.IA16ToBinary(Image);

                    }
                    break;
                case ImageFormat.I:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_4b:
                            return TextureConversion.I4ToBinary(Image);

                        case PixelInfo.Size_8b:
                            return TextureConversion.I8ToBinary(Image);

                    }
                    break;
            }

            return null;
        }

        private Bitmap GenerateImage(byte[] bytes)
        {
            if (bytes == null)
                return null;

            switch (Format)
            {
                case ImageFormat.RGBA:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_16b:
                            return TextureConversion.BinaryToRGBA16(bytes, Width, Height);

                        case PixelInfo.Size_32b:
                            return TextureConversion.BinaryToRGBA32(bytes, Width, Height);

                    }
                    break;
                case ImageFormat.YUV:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_16b:
                            return null;
                            //return TextureConversion.BinaryToYUV16(bytes, Width, Height);
                    }
                    break;
                case ImageFormat.CI:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_4b:
                            return TextureConversion.BinaryToCI4(bytes, ImagePalette, PaletteIndex, Width, Height);

                        case PixelInfo.Size_8b:
                            return TextureConversion.BinaryToCI8(bytes, ImagePalette, Width, Height);

                    }
                    break;
                case ImageFormat.IA:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_4b:
                            return TextureConversion.BinaryToIA4(bytes, Width, Height);

                        case PixelInfo.Size_8b:
                            return TextureConversion.BinaryToIA8(bytes, Width, Height);

                        case PixelInfo.Size_16b:
                            return TextureConversion.BinaryToIA16(bytes, Width, Height);

                    }
                    break;
                case ImageFormat.I:
                    switch (PixelSize)
                    {
                        case PixelInfo.Size_4b:
                            return TextureConversion.BinaryToI4(bytes, Width, Height);

                        case PixelInfo.Size_8b:
                            return TextureConversion.BinaryToI8(bytes, Width, Height);

                    }
                    break;
            }

            return null;
        }
    }
}
