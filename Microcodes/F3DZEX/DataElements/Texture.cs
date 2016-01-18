using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cereal64.Common;

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

        private Bitmap _image;
        private byte[] _rawData;

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

        public Texture(int index, byte[] bytes, ImageFormat format = ImageFormat.RGBA,
            PixelInfo pixel = PixelInfo.Size_32b, int width = 0, int height = 0)
            : base(index, bytes)
        {
            Format = format;
            PixelSize = pixel;
            Width = width;
            Height = height;
            
            //generate image
        }

        public override byte[] RawData
        {
            get
            {
                return _rawData;
            }
            set
            {
                _rawData = value;

                //Since default is RGBA 4b, the constructor will not set this
                if (IsValidFormat)
                {
                    //generate image
                }
            }
        }

        public override int RawDataSize { get { return _rawData.Length; } }

        public Bitmap Image
        {
            get
            {
                if (_image == null)
                {
                    //generate
                }
                return _image;
            }
            set
            {
                _image = value;
            }
        }
    }
}
