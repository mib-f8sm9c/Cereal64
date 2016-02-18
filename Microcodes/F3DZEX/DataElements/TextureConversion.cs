using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cereal64.Common;
using Cereal64.Common.Utils;

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

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int index = (i + j * width) * 2;
                    byte R, G, B, A;

                    R = (byte)Math.Round(((((ushort)ByteHelper.ReadByte(imgData, index)) >> 3) & 0x1F) * 255.0 / 31.0);
                    G = (byte)Math.Round(((((ushort)ByteHelper.ReadUShort(imgData, index)) >> 6) & 0x1F) * 255.0 / 31.0);
                    B = (byte)Math.Round(((((ushort)ByteHelper.ReadByte(imgData, index + 1)) >> 1) & 0x1F) * 255.0 / 31.0);
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

            for (int j = 0; j < bmp.Height; j++)
            {
                for (int i = 0; i < bmp.Width; i++)
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
                    int index = (i + j * width) * 4;
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
        public static Bitmap BinaryToCI4(byte[] imgData, Palette palette, int paletteIndex, int width, int height)
        {
            //Pixel size is 1/2 byte
            if (width / 2 * height != imgData.Length)
                return null;

            if (palette == null || palette.Colors.Length < 16)
                return null;
            
            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i += 2)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = (i + j * width) / 2;

                    byte CI = ByteHelper.ReadByte(imgData, index);
                    byte CI1 = (byte)(CI >> 4);
                    byte CI2 = (byte)(CI & 0x0F);

                    bmp.SetPixel(i, j, palette.Colors[CI1 + 16 * paletteIndex]);
                    bmp.SetPixel(i + 1, j, palette.Colors[CI2 + 16 * paletteIndex]);

                }
            }

            return bmp;
        }

        public static byte[] CI4ToBinary(Bitmap bmp, Palette palette, int paletteIndex)
        {
            //Pixel size is 1/2 bytes

            if (bmp == null)
                return null;

            if (palette == null || palette.Colors.Length < 16)
                return null;

            byte[] imgData = new byte[bmp.Width * bmp.Height / 2];

            int[] paletteIDs = new int[16];
            for(int k = 0; k < 16; k++)
            {
                paletteIDs[k] = palette.Colors[k + 16 * paletteIndex].ToArgb();
            }

            for (int i = 0; i < bmp.Width; i+=2)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = (i + j * bmp.Width) / 2;

                    Color pixel = bmp.GetPixel(i, j);
                    int pixelID = pixel.ToArgb();

                    byte index1 = 0x00;
                    if (paletteIDs.Contains(pixelID))
                        index1 = (byte)paletteIDs.First(p => p == pixelID);

                    pixel = bmp.GetPixel(i + 1, j);
                    pixelID = pixel.ToArgb();

                    byte index2 = 0x00;
                    if (paletteIDs.Contains(pixelID))
                        index2 = (byte)paletteIDs.First(p => p == pixelID);

                    ByteHelper.WriteByte((byte)((index1 << 4) | index2), imgData, index);
                }
            }

            return imgData;
        }

        public static Bitmap BinaryToCI8(byte[] imgData, Palette palette, int width, int height)
        {
            //Pixel size is 1 byte
            if (width * height != imgData.Length)
                return null;

            if (palette == null || palette.Colors.Length < 256)
                return null;

            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = (i + j * width);

                    byte CI = ByteHelper.ReadByte(imgData, index);

                    bmp.SetPixel(i, j, palette.Colors[CI]);
                }
            }

            return bmp;
        }

        public static byte[] CI8ToBinary(Bitmap bmp, Palette palette)
        {
            //Pixel size is 1 byte

            if (bmp == null)
                return null;

            if (palette == null || palette.Colors.Length < 256)
                return null;

            byte[] imgData = new byte[bmp.Width * bmp.Height];

            int[] paletteIDs = new int[256];
            for (int k = 0; k < 256; k++)
            {
                paletteIDs[k] = palette.Colors[k].ToArgb();
            }

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = i + j * bmp.Width;

                    Color pixel = bmp.GetPixel(i, j);
                    int pixelID = pixel.ToArgb();

                    byte palIndex = 0x00;
                    if (paletteIDs.Contains(pixelID))
                        palIndex = (byte)paletteIDs.First(p => p == pixelID);

                    ByteHelper.WriteByte(palIndex, imgData, index);
                }
            }

            return imgData;
        }

        #endregion

        #region IA

        //4b, 8b, 16b

        public static Bitmap BinaryToIA4(byte[] imgData, int width, int height)
        {
            //Pixel size is 1/2 byte

            if (width / 2 * height != imgData.Length)
                return null;

            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i += 2)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = (i + j * width);

                    byte byt = ByteHelper.ReadByte(imgData, index);
                    byte halfB1 = (byte)(byt >> 4);
                    byte halfB2 = (byte)(byt & 0x0F);

                    byte C1 = (byte)Math.Round((halfB1 >> 1) * 255.0 / 7.0);
                    byte A1 = (byte)Math.Round((halfB1 & 0x01) * 255.0);
                    byte C2 = (byte)Math.Round((halfB2 >> 1) * 255.0 / 7.0);
                    byte A2 = (byte)Math.Round((halfB2 & 0x01) * 255.0);

                    bmp.SetPixel(i, j, Color.FromArgb(A1, C1, C1, C1));
                    bmp.SetPixel(i, j, Color.FromArgb(A2, C2, C2, C2));

                }
            }

            return bmp;
        }

        public static byte[] IA4ToBinary(Bitmap bmp)
        {
            //Pixel size is 1/2 bytes

            if (bmp == null)
                return null;

            byte[] imgData = new byte[bmp.Width * bmp.Height / 2];

            for (int i = 0; i < bmp.Width; i += 2)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = (i + j * bmp.Width) / 2;

                    Color pixel = bmp.GetPixel(i, j);

                    byte halfB1 = (byte)(((uint)Math.Round(pixel.R * 7.0 / 255.0) << 1) | (byte)(pixel.A >> 7));
                    
                    pixel = bmp.GetPixel(i + 1, j);

                    byte halfB2 = (byte)(((uint)Math.Round(pixel.R * 7.0 / 255.0) << 1) | (byte)(pixel.A >> 7));

                    ByteHelper.WriteByte((byte)((halfB1 << 4) | halfB2), imgData, index);
                }
            }

            return imgData;
        }

        public static Bitmap BinaryToIA8(byte[] imgData, int width, int height)
        {
            //Pixel size is 1 byte

            if (width * height != imgData.Length)
                return null;

            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = (i + j * width);

                    byte byt = ByteHelper.ReadByte(imgData, index);
                    byte halfB1 = (byte)(byt >> 4);
                    byte halfB2 = (byte)(byt & 0x0F);

                    byte C = (byte)Math.Round(halfB1 * 255.0 / 15.0);
                    byte A = (byte)Math.Round(halfB2 * 255.0 / 15.0);

                    bmp.SetPixel(i, j, Color.FromArgb(A, C, C, C));

                }
            }

            return bmp;
        }

        public static byte[] IA8ToBinary(Bitmap bmp)
        {
            //Pixel size is 1 bytes

            if (bmp == null)
                return null;

            byte[] imgData = new byte[bmp.Width * bmp.Height];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = (i + j * bmp.Width);

                    Color pixel = bmp.GetPixel(i, j);

                    byte halfB1 = (byte)Math.Round(pixel.R * 15.0 / 255.0);
                    byte halfB2 = (byte)Math.Round(pixel.A * 15.0 / 255.0);

                    ByteHelper.WriteByte((byte)((halfB1 << 4) | halfB2), imgData, index);
                }
            }

            return imgData;
        }

        public static Bitmap BinaryToIA16(byte[] imgData, int width, int height)
        {
            //Pixel size is 2 bytes

            if (width * height * 2 != imgData.Length)
                return null;

            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = (i + j * width) * 2;

                    byte C = ByteHelper.ReadByte(imgData, index);
                    byte A = ByteHelper.ReadByte(imgData, index + 1);

                    bmp.SetPixel(i, j, Color.FromArgb(A, C, C, C));
                }
            }

            return bmp;
        }

        public static byte[] IA16ToBinary(Bitmap bmp)
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

                    ByteHelper.WriteByte(pixel.R, imgData, index);
                    ByteHelper.WriteByte(pixel.A, imgData, index + 1);
                }
            }

            return imgData;
        }

        #endregion

        #region I

        //4b, 8b

        public static Bitmap BinaryToI4(byte[] imgData, int width, int height)
        {
            //Pixel size is 1/2 byte

            if (width / 2 * height != imgData.Length)
                return null;

            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i+=2)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = (i + j * width) / 2;

                    byte I = ByteHelper.ReadByte(imgData, index);
                    byte I1 = (byte)Math.Round((I >> 4) * 255.0 / 15.0);
                    byte I2 = (byte)Math.Round((I & 0x0F) * 255.0 / 15.0);

                    bmp.SetPixel(i, j, Color.FromArgb(I1, I1, I1, I1));
                    bmp.SetPixel(i, j, Color.FromArgb(I2, I2, I2, I2));

                }
            }

            return bmp;
        }

        public static byte[] I4ToBinary(Bitmap bmp)
        {
            //Pixel size is 1/2 bytes

            if (bmp == null)
                return null;

            byte[] imgData = new byte[bmp.Width * bmp.Height / 2];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = (i + j * bmp.Width) / 2;

                    Color pixel = bmp.GetPixel(i, j);

                    byte I = (byte)Math.Round(pixel.R * 15.0 / 255.0);
                    if (i % 2 == 0)
                        I = (byte)(I << 4);

                    ByteHelper.WriteByte((byte)(imgData[index] | I), imgData, index);
                }
            }

            return imgData;
        }

        public static Bitmap BinaryToI8(byte[] imgData, int width, int height)
        {
            //Pixel size is 1 byte

            if (width * height != imgData.Length)
                return null;

            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i + j * width;

                    byte I = ByteHelper.ReadByte(imgData, index);

                    bmp.SetPixel(i, j, Color.FromArgb(I, I, I, I));
                }
            }

            return bmp;
        }

        public static byte[] I8ToBinary(Bitmap bmp)
        {
            //Pixel size is 1 byte

            if (bmp == null)
                return null;

            byte[] imgData = new byte[bmp.Width * bmp.Height];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = i + j * bmp.Width;

                    Color pixel = bmp.GetPixel(i, j);

                    ByteHelper.WriteByte(pixel.R, imgData, index);
                }
            }

            return imgData;
        }


        #endregion

        #region Palette

        public static Color[] BinaryToPalette(byte[] imgData, int count)
        {
            //Color size is 2 bytes

            //if (count != 16 && count != 256)
            //    return null;

            Color[] colors = new Color[count];

            for (int i = 0; i < count; i++)
            {
                int index = i * 2;
                byte R, G, B, A;

                R = (byte)Math.Round(((((ushort)ByteHelper.ReadByte(imgData, index)) >> 3) & 0x1F) * 255.0 / 31.0);
                G = (byte)Math.Round(((((ushort)ByteHelper.ReadUShort(imgData, index)) >> 6) & 0x1F) * 255.0 / 31.0);
                B = (byte)Math.Round(((((ushort)ByteHelper.ReadByte(imgData, index + 1)) >> 1) & 0x1F) * 255.0 / 31.0);
                A = (byte)((ByteHelper.ReadByte(imgData, index + 1) & 0x01) * 0xFF);

                colors[i] = Color.FromArgb(A, R, G, B);
            }

            return colors;
        }

        public static byte[] PaletteToBinary(Color[] colors)
        {
            //Pixel size is 2 bytes

            //if (colors.Length != 16 && colors.Length != 256)
            //    return null;

            byte[] paletteData = new byte[colors.Length * 2];

            for (int i = 0; i < colors.Length; i++)
            {
                int index = i * 2;

                byte byte1, byte2;
                byte R = (byte)Math.Round(colors[i].R * 31.0 / 255.0);
                byte G = (byte)Math.Round(colors[i].G * 31.0 / 255.0);
                byte B = (byte)Math.Round(colors[i].B * 31.0 / 255.0);
                byte A = (byte)(colors[i].A == 0x00 ? 0x00 : 0x01);

                byte1 = (byte)((R << 3) | (G >> 2));
                byte2 = (byte)((G << 6) | (B << 1) | (A));

                ByteHelper.WriteByte(byte1, paletteData, index);
                ByteHelper.WriteByte(byte2, paletteData, index + 1);
            }

            return paletteData;
        }

        #endregion
    }
}
