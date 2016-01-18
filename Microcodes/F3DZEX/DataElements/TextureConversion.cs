using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public static class TextureConversion
    {
        #region RGBA

        public static Bitmap BinaryToRGBA16(byte[] imgData, int width, int height)
        {
            //Pixel size is 2 bytes

            if (width * height * 2 != imgData.Length)
                return null;

            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i + j * width;
                    byte R, G, B, A;

                    R = (byte)Math.Round(((((ushort)ByteHelper.ReadByte(imgData, index)) >> 3) & 0x1F) * 255.0 / 31.0);
                    G = (byte)Math.Round(((((ushort)ByteHelper.ReadUShort(imgData, index)) >> 6) & 0x1F) * 255.0 / 31.0);
                    B = (byte)Math.Round(((((ushort)ByteHelper.ReadByte(imgData, index + 1)) >> 3) & 0x1F) * 255.0 / 31.0);
                    A = (byte)((ByteHelper.ReadByte(imgData, index + 1) & 0x01) * 0xFF);

                    bmp.SetPixel(i, j, Color.FromArgb(A, R, G, B));
                }
            }

            return bmp;
        }

        public static byte[] RGBA16ToBinary(Bitmap bmp)
        {
            //Pixel size is 2 bytes

            if (bmp == null)
                return null;

            byte[] imgData = new byte[bmp.Width * bmp.Height * 2];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = (i + j * bmp.Width) * 2;

                    Color pixel = bmp.GetPixel(i, j);

                    byte byte1, byte2;
                    byte R = (byte)Math.Round(pixel.R * 31.0 / 255.0);
                    byte G = (byte)Math.Round(pixel.G * 31.0 / 255.0);
                    byte B = (byte)Math.Round(pixel.B * 31.0 / 255.0);
                    byte A = (byte)(pixel.A == 0x00 ? 0x00 : 0x01);

                    byte1 = (byte)((R << 3) | (G >> 2));
                    byte2 = (byte)((G << 6) | (B << 1) | (A));

                    ByteHelper.WriteByte(byte1, imgData, index);
                    ByteHelper.WriteByte(byte2, imgData, index + 1);
                }
            }

            return imgData;
        }

        public static Bitmap BinaryToRGBA32(byte[] imgData, int width, int height)
        {
            //Pixel size is 4 bytes

            if (width * height * 4 != imgData.Length)
                return null;

            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i + j * width;
                    byte R, G, B, A;

                    R = ByteHelper.ReadByte(imgData, index);
                    G = ByteHelper.ReadByte(imgData, index + 1);
                    B = ByteHelper.ReadByte(imgData, index + 2);
                    A = ByteHelper.ReadByte(imgData, index + 3);
                    
                    bmp.SetPixel(i, j, Color.FromArgb(A, R, G, B));
                }
            }

            return bmp;
        }

        public static byte[] RGBA32ToBinary(Bitmap bmp)
        {
            //Pixel size is 4 bytes

            if (bmp == null)
                return null;

            byte[] imgData = new byte[bmp.Width * bmp.Height * 4];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = (i + j * bmp.Width) * 4;
                    
                    Color pixel = bmp.GetPixel(i, j);

                    ByteHelper.WriteByte(pixel.R, imgData, index);
                    ByteHelper.WriteByte(pixel.G, imgData, index + 1);
                    ByteHelper.WriteByte(pixel.B, imgData, index + 2);
                    ByteHelper.WriteByte(pixel.A, imgData, index + 3);
                }
            }

            return imgData;
        }

        #endregion

        #region YUV

        //16b

        #endregion

        #region CI

        //4b, 8b

        #endregion

        #region IA

        //4b, 8b, 16b

        #endregion

        #region I

        //4b, 8b

        #endregion
    }
}
