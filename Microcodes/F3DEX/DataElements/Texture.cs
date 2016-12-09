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
    public class Texture : N64DataElement, IUpdatable
    {
        private const string FORMAT = "Format";
        private const string PIXEL_SIZE = "PixelSize";
        private const string WIDTH = "Width";
        private const string HEIGHT = "Height";

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

        private bool _initializing = true;
        private byte[] _textureData;

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

            //generate image
            _initializing = false;
            RawData = _textureData;
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
            _initializing = false;
            RawData = bytes;
        }

        public override byte[] RawData
        {
            get
            {
                return _textureData;
            }
            set
            {
                _textureData = value;

                //if (IsValidFormat)
                //{
                //    //generate image
                //    Image = GenerateImage(value);

                //    if(Image == null) //Catch if it failed
                //        _textureData = value;
                //    else
                //        _textureData = null;
                //}
            }
        }

        public override int RawDataSize 
        { 
            get 
            {
                return _textureData.Length;
            }
        }

        public override XElement GetAsXML()
        {
            XElement baseElement = base.GetAsXML();

            baseElement.Add(new XAttribute(FORMAT, Format));
            baseElement.Add(new XAttribute(PIXEL_SIZE, PixelSize));
            baseElement.Add(new XAttribute(WIDTH, Width));
            baseElement.Add(new XAttribute(HEIGHT, Height));

            return baseElement;
        }

        public UpdateEvent Updated { get { return _updated; } set { _updated = value; } }

        private UpdateEvent _updated = delegate { };
    }
}
