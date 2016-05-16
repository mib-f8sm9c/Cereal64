using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DEX.DataElements
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

        public static byte[] CI4ToBinary(Bitmap bmp, Palette palette, int paletteIndex, bool generateNewPalette = false)
        {
            //Pixel size is 1/2 bytes
            if (bmp == null)
                return null;

            if (palette == null || palette.Colors.Length < 16)
                return null;

            if (generateNewPalette)
                GenerateNewPalette(palette, bmp, 16);

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

                    int index1 = -1;
                    byte closestIndex = 0;
                    double closestDist = double.MaxValue;
                    for (byte p = 0; p < paletteIDs.Length; p++)
                    {
                        if (paletteIDs[p] == pixelID)
                        {
                            index1 = p;
                            break;
                        }
                        else
                        {
                            //Get the dist to the color, and keep track of which is the best representation
                            double dist = pixel.ColorDistanceFrom(palette.Colors[p + 16 * paletteIndex]);

                            if (dist < closestDist)
                            {
                                closestDist = dist;
                                closestIndex = p;
                            }
                        }
                    }
                    if (index1 == -1)
                        index1 = closestIndex;

                    pixel = bmp.GetPixel(i + 1, j);
                    pixelID = pixel.ToArgb();

                    int index2 = -1;
                    closestIndex = 0;
                    closestDist = double.MaxValue;
                    for (byte p = 0; p < paletteIDs.Length; p++)
                    {
                        
                        if (paletteIDs[p] == pixelID)
                        {
                            index2 = p;
                            break;
                        }
                        else
                        {
                            //Get the dist to the color, and keep track of which is the best representation
                            double dist = pixel.ColorDistanceFrom(palette.Colors[p + 16 * paletteIndex]);

                            if (dist < closestDist)
                            {
                                closestDist = dist;
                                closestIndex = p;
                            }
                        }
                    }
                    if (index2 == -1)
                        index2 = closestIndex;

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
                    for (byte p = 0; p < paletteIDs.Length; p++)
                    {
                        if (paletteIDs[p] == pixelID)
                        {
                            palIndex = p;
                            break;
                        }
                    }

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

        private static void GenerateNewPalette(Palette palette, Bitmap image, int colorCount)
        {
            //This can take a long time (not optimized), so be careful
            List<Color> colors = new List<Color>();

            for (int j = 0; j < image.Height; j++)
            {
                for (int i = 0; i < image.Width; i++)
                {
                    if (!colors.Contains(image.GetPixel(i, j)))
                        colors.Add(image.GetPixel(i, j));
                }
            }

            //Now that we have a huge list of colors, let's start going through them rapidly
            //List<double> minDist = new List<double>();
            //foreach (Color color in colors)
            //    minDist.Add(double.MaxValue);

            //while (colors.Count > colorCount)
            //{
            //    for (int i = 0; i < colors.Count; i++)
            //    {
            //        for (int j = i + 1; j < colors.Count; j++)
            //        {
            //            double dist = colors[i].ColorDistanceFrom(colors[j]);
            //            if (dist < minDist[i])
            //                minDist[i] = dist;
            //        }
            //    }

            //    //Find smallest one, clear it out
            //    double minvalue = minDist.Min();
            //    int removeIndex = minDist.IndexOf(minvalue);
            //    colors.RemoveAt(removeIndex);
            //    minDist.RemoveAt(removeIndex);

            //    //start over
            //    for (int i = 0; i < minDist.Count; i++)
            //        minDist[i] = double.MaxValue;
            //}

            PaletteMedianCutAnalyzer analyzer = new PaletteMedianCutAnalyzer();
            foreach (Color color in colors)
                analyzer.AddColor(color);

            //Now make the new palette
            byte[] newData = PaletteToBinary(analyzer.GetPalette(16));

            palette.RawData = newData;
        }

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

        public static double ColorDistanceFrom(this Color color, Color comparisonColor)
        {
            double dist = Math.Pow(color.R - comparisonColor.R, 2) +
                                    Math.Pow(color.G - comparisonColor.G, 2) +
                                     Math.Pow(color.B - comparisonColor.B, 2);

            if ((color.A == 0 && comparisonColor.A != 0) ||
                (color.A != 0 && comparisonColor.A == 0))
                    dist += 1000000000; //Just make it a worst choice if alphas don't line up

            return dist;
        }

        #endregion
    }

    public class PaletteMedianCutAnalyzer
    {
        private byte _minRed, _maxRed, _minBlue, _maxBlue, _minGreen, _maxGreen;
        private List<Color> _opaqueColors, _transparentColors;

        public PaletteMedianCutAnalyzer()
        {
            _minRed = 0xFF;
            _minBlue = 0xFF;
            _minGreen = 0xFF;

            _maxRed = 0x00;
            _maxBlue = 0x00;
            _maxGreen = 0x00;

            _opaqueColors = new List<Color>();
            _transparentColors = new List<Color>();
        }

        public void AddColor(Color color)
        {
            if (color.A == 0)
            {
                //Add to transparent
                if (!_transparentColors.Contains(color))
                    _transparentColors.Add(color);
            }
            else
            {
                if (!_opaqueColors.Contains(color))
                {
                    _opaqueColors.Add(color);
                    _minRed = (byte)Math.Min(color.R, _minRed);
                    _minGreen = (byte)Math.Min(color.G, _minGreen);
                    _minBlue = (byte)Math.Min(color.B, _minBlue);

                    _maxRed = (byte)Math.Min(color.R, _maxRed);
                    _maxGreen = (byte)Math.Min(color.G, _maxGreen);
                    _maxBlue = (byte)Math.Min(color.B, _maxBlue);
                }
            }
        }

        public Color[] GetPalette(int paletteSize)
        {
            Color[] colors = new Color[paletteSize];

            ColorBox[] boxes = new ColorBox[paletteSize];

            int boxIndex = 0;
            ColorBox box;
            int startIndex = 0;

            if (_transparentColors.Count > 0)
            {
                box = new ColorBox(0);
                foreach (Color color in _transparentColors)
                    box.Colors.Add(color);
                box.ShrinkToFit();
                boxes[boxIndex] = box;
                boxIndex++;
                startIndex++;
            }

            //Handle the rest of the boxes
            box = new ColorBox(0);
            foreach (Color color in _opaqueColors)
                box.Colors.Add(color);
            box.ShrinkToFit();
            boxes[boxIndex] = box;
            boxIndex++;
            while (boxIndex < paletteSize)
            {
                byte longestEdge = 0;
                int longestIndex = startIndex;
                for (int i = startIndex; i < boxIndex; i++)
                {
                    if (boxes[i].LongestSide > longestEdge)
                    {
                        longestEdge = boxes[i].LongestSide;
                        longestIndex = i;
                    }
                }

                Tuple<ColorBox, ColorBox> newBoxes = boxes[longestIndex].SplitBox();

                boxes[longestIndex] = newBoxes.Item1;
                boxes[boxIndex] = newBoxes.Item2;

                boxIndex++;
            }

            for (int i = 0; i < boxes.Length; i++)
                colors[i] = boxes[i].GetCentroidColor();

            return colors;
        }

        private struct ColorBox
        {
            public byte MinRed;
            public byte MaxRed;
            public byte MinBlue;
            public byte MaxBlue;
            public byte MinGreen;
            public byte MaxGreen;

            public List<Color> Colors;

            public ColorBox(int nothing = 0)
            {
                MinRed = 0x00;
                MinBlue = 0x00;
                MinGreen = 0x00;

                MaxRed = 0xFF;
                MaxGreen = 0xFF;
                MaxBlue = 0xFF;

                Colors = new List<Color>();
            }

            public ColorBox(byte minR, byte maxR, byte minB, byte maxB, byte minG, byte maxG)
            {
                MinRed = minR;
                MinBlue = minB;
                MinGreen = minG;

                MaxRed = maxR;
                MaxGreen = maxG;
                MaxBlue = maxB;

                Colors = new List<Color>();
            }

            public bool CanContainColor(Color color)
            {
                if (color.R < MinRed || color.R > MaxRed)
                    return false;
                if (color.B < MinBlue || color.B > MaxBlue)
                    return false;
                if (color.G < MinGreen || color.G > MaxGreen)
                    return false;

                return true;
            }

            public byte LongestSide
            {
                get
                {
                    return (byte)Math.Max(Math.Max(MaxRed - MinRed, MaxGreen - MinGreen), MaxBlue - MinBlue);
                }
            }

            public Tuple<ColorBox, ColorBox> SplitBox()
            {
                byte redLength = (byte)(MaxRed - MinRed);
                byte blueLength = (byte)(MaxBlue - MinBlue);
                byte greenLength = (byte)(MaxGreen - MinGreen);

                ColorBox cb1, cb2;

                if (redLength >= blueLength && redLength >= greenLength)
                {
                    cb1 = new ColorBox(MinRed, (byte)(MinRed + (redLength / 2)), MinBlue, MaxBlue, MinGreen, MaxGreen);
                    cb2 = new ColorBox((byte)(MinRed + (redLength / 2)), MaxRed, MinBlue, MaxBlue, MinGreen, MaxGreen);
                }
                if (blueLength >= redLength && blueLength >= greenLength)
                {
                    cb1 = new ColorBox(MinRed, MaxRed, MinBlue, (byte)(MinBlue + (blueLength / 2)), MinGreen, MaxGreen);
                    cb2 = new ColorBox(MinRed, MaxRed, (byte)(MinBlue + (blueLength / 2)), MaxBlue, MinGreen, MaxGreen);
                }
                else
                {
                    cb1 = new ColorBox(MinRed, MaxRed, MinBlue, MaxBlue, MinGreen, (byte)(MinGreen + (greenLength / 2)));
                    cb2 = new ColorBox(MinRed, MaxRed, MinBlue, MaxBlue, (byte)(MinGreen + (greenLength / 2)), MaxGreen);
                }

                foreach (Color color in Colors)
                {
                    if (cb1.CanContainColor(color))
                        cb1.Colors.Add(color);
                    else
                        cb2.Colors.Add(color);
                }

                cb1.ShrinkToFit();
                cb2.ShrinkToFit();

                return new Tuple<ColorBox, ColorBox>(cb1, cb2);
            }

            public Color GetCentroidColor()
            {
                int red = 0, blue = 0, green = 0, alpha = 0;
                foreach (Color color in Colors)
                {
                    red += color.R;
                    green += color.G;
                    blue += color.B;
                    alpha += color.A;
                }

                if (Colors.Count > 0)
                {
                    red = (int)Math.Round(red / (double)Colors.Count);
                    blue = (int)Math.Round(blue / (double)Colors.Count);
                    green = (int)Math.Round(green / (double)Colors.Count);
                    alpha = alpha > 0 ? 255 : 0;
                }

                return Color.FromArgb(alpha, red, green, blue);
            }

            public void ShrinkToFit()
            {
                //Compress the box
                MinRed = 0xFF;
                MinGreen = 0xFF;
                MinBlue = 0xFF;

                MaxRed = 0x00;
                MaxGreen = 0x00;
                MaxBlue = 0x00;

                foreach (Color color in Colors)
                {
                    MinRed = (byte)Math.Min(color.R, MinRed);
                    MinGreen = (byte)Math.Min(color.G, MinGreen);
                    MinBlue = (byte)Math.Min(color.B, MinBlue);

                    MaxRed = (byte)Math.Max(color.R, MaxRed);
                    MaxGreen = (byte)Math.Max(color.G, MaxGreen);
                    MaxBlue = (byte)Math.Max(color.B, MaxBlue);
                }
            }
        }
    }
}
