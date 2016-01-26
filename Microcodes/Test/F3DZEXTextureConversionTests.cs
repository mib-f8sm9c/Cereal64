using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cereal64.Microcodes.F3DZEX.DataElements;
using Cereal64.Microcodes.UnitTests.Properties;
using System.Drawing;

namespace Cereal64.Microcodes.UnitTests
{
    //To do: improve the quality of the unit test images here

    [TestClass]
    public class F3DZEXTextureConversionTests
    {
        [TestMethod]
        public void TestRGB16()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA16ToBinary(Resources.Test_255And0Only);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA16(bytes, Resources.Test_255And0Only.Width, Resources.Test_255And0Only.Height);

            Assert.AreEqual(Resources.Test_255And0Only.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_255And0Only.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_255And0Only.Width; i++)
            {
                for (int j = 0; j < Resources.Test_255And0Only.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_255And0Only.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }

        [TestMethod]
        public void TestRGB32()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA32ToBinary(Resources.Test_FullRange);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA32(bytes, Resources.Test_FullRange.Width, Resources.Test_FullRange.Height);

            Assert.AreEqual(Resources.Test_FullRange.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_FullRange.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_FullRange.Width; i++)
            {
                for (int j = 0; j < Resources.Test_FullRange.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_FullRange.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }

        //[TestMethod]
        //public void TestYUV16();

        [TestMethod]
        public void TestCI4()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA32ToBinary(Resources.Test_255And0Only_BWOnly);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA32(bytes, Resources.Test_255And0Only_BWOnly.Width, Resources.Test_255And0Only_BWOnly.Height);

            Assert.AreEqual(Resources.Test_255And0Only_BWOnly.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_255And0Only_BWOnly.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_255And0Only_BWOnly.Width; i++)
            {
                for (int j = 0; j < Resources.Test_255And0Only_BWOnly.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_255And0Only_BWOnly.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }
        
        [TestMethod]
        public void TestCI8()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA32ToBinary(Resources.Test_FullRange_BWOnly);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA32(bytes, Resources.Test_FullRange_BWOnly.Width, Resources.Test_FullRange_BWOnly.Height);

            Assert.AreEqual(Resources.Test_FullRange_BWOnly.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_FullRange_BWOnly.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_FullRange_BWOnly.Width; i++)
            {
                for (int j = 0; j < Resources.Test_FullRange_BWOnly.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_FullRange_BWOnly.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }

        [TestMethod]
        public void TestIA4()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA32ToBinary(Resources.Test_255And0Only_BWOnly);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA32(bytes, Resources.Test_255And0Only_BWOnly.Width, Resources.Test_255And0Only_BWOnly.Height);

            Assert.AreEqual(Resources.Test_255And0Only_BWOnly.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_255And0Only_BWOnly.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_255And0Only_BWOnly.Width; i++)
            {
                for (int j = 0; j < Resources.Test_255And0Only_BWOnly.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_255And0Only_BWOnly.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }

        [TestMethod]
        public void TestIA8()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA32ToBinary(Resources.Test_255And0Only_BWOnly);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA32(bytes, Resources.Test_255And0Only_BWOnly.Width, Resources.Test_255And0Only_BWOnly.Height);

            Assert.AreEqual(Resources.Test_255And0Only_BWOnly.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_255And0Only_BWOnly.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_255And0Only_BWOnly.Width; i++)
            {
                for (int j = 0; j < Resources.Test_255And0Only_BWOnly.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_255And0Only_BWOnly.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }

        [TestMethod]
        public void TestIA16()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA32ToBinary(Resources.Test_FullRange_BWOnly);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA32(bytes, Resources.Test_FullRange_BWOnly.Width, Resources.Test_FullRange_BWOnly.Height);

            Assert.AreEqual(Resources.Test_FullRange_BWOnly.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_FullRange_BWOnly.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_FullRange_BWOnly.Width; i++)
            {
                for (int j = 0; j < Resources.Test_FullRange_BWOnly.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_FullRange_BWOnly.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }

        [TestMethod]
        public void TestI4()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA32ToBinary(Resources.Test_255And0Only_BWOnly);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA32(bytes, Resources.Test_255And0Only_BWOnly.Width, Resources.Test_255And0Only_BWOnly.Height);

            Assert.AreEqual(Resources.Test_255And0Only_BWOnly.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_255And0Only_BWOnly.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_255And0Only_BWOnly.Width; i++)
            {
                for (int j = 0; j < Resources.Test_255And0Only_BWOnly.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_255And0Only_BWOnly.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }

        [TestMethod]
        public void TestI8()
        {
            //Convert from bmp to bytes
            byte[] bytes = TextureConversion.RGBA32ToBinary(Resources.Test_FullRange_BWOnly);

            Bitmap newBitmap = TextureConversion.BinaryToRGBA32(bytes, Resources.Test_FullRange_BWOnly.Width, Resources.Test_FullRange_BWOnly.Height);

            Assert.AreEqual(Resources.Test_FullRange_BWOnly.Width, newBitmap.Width);
            Assert.AreEqual(Resources.Test_FullRange_BWOnly.Height, newBitmap.Height);

            for (int i = 0; i < Resources.Test_FullRange_BWOnly.Width; i++)
            {
                for (int j = 0; j < Resources.Test_FullRange_BWOnly.Height; j++)
                {
                    Assert.AreEqual(Resources.Test_FullRange_BWOnly.GetPixel(i, j), newBitmap.GetPixel(i, j));
                }
            }
        }
        
    }
}
