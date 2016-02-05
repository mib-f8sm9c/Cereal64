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
    public class Texture : N64DataElement
    {
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

        public ImageFormat Format;
        public PixelInfo PixelSize;

        public int Width;
        public int Height;

        public Palette ImagePalette;
        private Bitmap Image;
        //private byte[] _rawData;

        private bool _initializing = true;

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
        }

        public Texture(int index, byte[] bytes, ImageFormat format = ImageFormat.RGBA,
            PixelInfo pixel = PixelInfo.Size_32b, int width = 0, int height = 0, Palette palette = null)
            : base(index, bytes)
        {
            Format = format;
            PixelSize = pixel;
            Width = width;
            Height = height;

            ImagePalette = palette;
            
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
                    return;

                if (IsValidFormat)
                {
                    //generate image
                    Image = GenerateImage(value);
                }
            }
        }

        public override int RawDataSize 
        { 
            get 
            {
                int length = 0;
                switch (Format)
                {
                    case ImageFormat.RGBA:
                        switch (PixelSize)
                        {
                            case PixelInfo.Size_16b:

                                break;
                            case PixelInfo.Size_32b:

                                break;
                        }
                        break;
                    case ImageFormat.YUV:
                        switch (PixelSize)
                        {
                            case PixelInfo.Size_16b:

                                break;
                        }
                        break;
                    case ImageFormat.CI:
                        switch (PixelSize)
                        {
                            case PixelInfo.Size_4b:

                                break;
                            case PixelInfo.Size_8b:

                                break;
                        }
                        break;
                    case ImageFormat.IA:
                        switch (PixelSize)
                        {
                            case PixelInfo.Size_4b:

                                break;
                            case PixelInfo.Size_8b:

                                break;
                            case PixelInfo.Size_16b:

                                break;
                        }
                        break;
                    case ImageFormat.I:
                        switch (PixelSize)
                        {
                            case PixelInfo.Size_4b:

                                break;
                            case PixelInfo.Size_8b:

                                break;
                        }
                        break;
                }
                return length;
            }
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
                            return TextureConversion.CI4ToBinary(Image, ImagePalette);

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
                            return TextureConversion.BinaryToCI4(bytes, ImagePalette, Width, Height);

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
