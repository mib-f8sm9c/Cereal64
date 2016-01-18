using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Cereal64.Common
{
    public enum Endianness
    {
        BigEndian,
        LittleEndian
    }

    public static class ByteHelper
    {
        public static string DisplayValue(uint val, bool displayInHex = false, bool displayAllBytes = false)
        {
            if (displayInHex)
            {
                if (displayAllBytes)
                    return string.Format("0x{0:X8}", val);
                else
                    return string.Format("0x{0:X}", val);
            }

            return val.ToString();
        }

        public static string DisplayValue(int val, bool displayInHex = false, bool displayAllBytes = false)
        {
            if (displayInHex)
            {
                if (displayAllBytes)
                    return string.Format("0x{0:X8}", val);
                else
                    return string.Format("0x{0:X}", val);
            }

            return val.ToString();
        }

        #region Byte[] to Byte/SByte/Short/UShort/Int/UInt/Float

        public static uint ReadUInt(byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            return ReadUInt(data, (int)position, endian);
        }

        public static uint ReadUInt(byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 4)
                throw new Exception();

            byte[] bytes = new byte[4];
            Array.Copy(data, position, bytes, 0, 4);

            if (BitConverter.IsLittleEndian ^ endian == Endianness.LittleEndian)
                bytes = bytes.Reverse().ToArray();

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static int ReadInt(byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            return ReadInt(data, (int)position, endian);
        }

        public static int ReadInt(byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 4)
                throw new Exception();

            byte[] bytes = new byte[4];
            Array.Copy(data, position, bytes, 0, 4);

            if (BitConverter.IsLittleEndian ^ endian == Endianness.LittleEndian)
                bytes = bytes.Reverse().ToArray();

            return BitConverter.ToInt32(bytes, 0);
        }

        public static ushort ReadUShort(byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            return ReadUShort(data, (int)position, endian);
        }

        public static ushort ReadUShort(byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 2)
                throw new Exception();

            byte[] bytes = new byte[2];
            Array.Copy(data, position, bytes, 0, 2);

            if (BitConverter.IsLittleEndian ^ endian == Endianness.LittleEndian)
                bytes = bytes.Reverse().ToArray();

            return BitConverter.ToUInt16(bytes, 0);
        }

        public static short ReadShort(byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            return ReadShort(data, (int)position, endian);
        }

        public static short ReadShort(byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 2)
                throw new Exception();

            byte[] bytes = new byte[2];
            Array.Copy(data, position, bytes, 0, 2);

            if (BitConverter.IsLittleEndian ^ endian == Endianness.LittleEndian)
                bytes = bytes.Reverse().ToArray();

            return BitConverter.ToInt16(bytes, 0);
        }

        public static byte ReadByte(byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            return ReadByte(data, (int)position, endian);
        }

        public static byte ReadByte(byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position)
                throw new Exception();

            return data[position];
        }

        public static sbyte ReadSByte(byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            return ReadSByte(data, (int)position, endian);
        }

        public static sbyte ReadSByte(byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position)
                throw new Exception();

            return (sbyte)data[position];
        }

        public static float ReadFloat(byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            return ReadFloat(data, (int)position, endian);
        }

        public static float ReadFloat(byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 4)
                throw new Exception();

            byte[] bytes = new byte[4];
            Array.Copy(data, position, bytes, 0, 4);

            if (BitConverter.IsLittleEndian ^ endian == Endianness.LittleEndian)
                bytes = bytes.Reverse().ToArray();

            return BitConverter.ToSingle(bytes, 0);
        }

        public static void WriteUInt(uint value, byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            WriteUInt(value, data, (int)position, endian);
        }

        public static void WriteUInt(uint value, byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 4)
                throw new Exception();

            switch(endian)
            {
                case Endianness.LittleEndian:
                    data[position + 3] = (byte)(value >> 24 & 0xFF);
                    data[position + 2] = (byte)(value >> 16 & 0xFF);
                    data[position + 1] = (byte)(value >> 8 & 0xFF);
                    data[position] = (byte)(value & 0xFF);
                    break;
                case Endianness.BigEndian:
                    data[position] = (byte)(value >> 24 & 0xFF);
                    data[position + 1] = (byte)(value >> 16 & 0xFF);
                    data[position + 2] = (byte)(value >> 8 & 0xFF);
                    data[position + 3] = (byte)(value & 0xFF);
                    break;
            }

            return;
        }

        public static void WriteInt(int value, byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            WriteInt(value, data, (int)position, endian);
        }

        public static void WriteInt(int value, byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 4)
                throw new Exception();


            switch (endian)
            {
                case Endianness.LittleEndian:
                    data[position + 3] = (byte)(value >> 24 & 0xFF);
                    data[position + 2] = (byte)(value >> 16 & 0xFF);
                    data[position + 1] = (byte)(value >> 8 & 0xFF);
                    data[position] = (byte)(value & 0xFF);
                    break;
                case Endianness.BigEndian:
                    data[position] = (byte)(value >> 24 & 0xFF);
                    data[position + 1] = (byte)(value >> 16 & 0xFF);
                    data[position + 2] = (byte)(value >> 8 & 0xFF);
                    data[position + 3] = (byte)(value & 0xFF);
                    break;
            }

            return;
        }

        public static void WriteUShort(ushort value, byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            WriteUShort(value, data, (int)position, endian);
        }

        public static void WriteUShort(ushort value, byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 2)
                throw new Exception();


            switch (endian)
            {
                case Endianness.LittleEndian:
                    data[position + 1] = (byte)(value >> 8 & 0xFF);
                    data[position] = (byte)(value & 0xFF);
                    break;
                case Endianness.BigEndian:
                    data[position] = (byte)(value >> 8 & 0xFF);
                    data[position + 1] = (byte)(value & 0xFF);
                    break;
            }

            return;
        }

        public static void WriteShort(short value, byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            WriteShort(value, data, (int)position, endian);
        }

        public static void WriteShort(short value, byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 2)
                throw new Exception();


            switch (endian)
            {
                case Endianness.LittleEndian:
                    data[position + 1] = (byte)(value >> 8 & 0xFF);
                    data[position] = (byte)(value & 0xFF);
                    break;
                case Endianness.BigEndian:
                    data[position] = (byte)(value >> 8 & 0xFF);
                    data[position + 1] = (byte)(value & 0xFF);
                    break;
            }

            return;
        }

        public static void WriteByte(byte value, byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            WriteByte(value, data, (int)position, endian);
        }

        public static void WriteByte(byte value, byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position)
                throw new Exception();


            data[position] = (byte)(value & 0xFF);

            return;
        }

        public static void WriteSByte(sbyte value, byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            WriteSByte(value, data, (int)position, endian);
        }

        public static void WriteSByte(sbyte value, byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position)
                throw new Exception();


            data[position] = (byte)(value & 0xFF);

            return;
        }

        public static void WriteFloat(float value, byte[] data, uint position, Endianness endian = Endianness.BigEndian)
        {
            WriteFloat(value, data, (int)position, endian);
        }

        public static void WriteFloat(float value, byte[] data, int position, Endianness endian = Endianness.BigEndian)
        {
            if (data == null || data.Length < position + 4)
                throw new Exception();

            byte[] bytes = BitConverter.GetBytes(value);
            if(BitConverter.IsLittleEndian ^ endian == Endianness.LittleEndian)
                bytes = bytes.Reverse().ToArray();

            data[position] = bytes[0];
            data[position + 1] = bytes[1];
            data[position + 2] = bytes[2];
            data[position + 3] = bytes[3];

            return;
        }

        #endregion

        public static byte[] CombineIntoBytes(params object[] values)
        {
            return CombineIntoBytes(Endianness.BigEndian, values);
        }

        public static byte[] CombineIntoBytes(Endianness endian, params object[] values)
        {
            //To do: allow arrays to be passed in

            List<byte> bytes = new List<byte>();

            foreach (object value in values)
            {
                if (value is IList)
                {
                    IList list = (IList)value;
                    bytes.AddRange(CombineIntoBytes(endian, list.Cast<object>().ToArray()));
                }
                else
                {
                    //Single value
                    switch (Type.GetTypeCode(value.GetType()))
                    {
                        case TypeCode.Int32:
                            switch (endian)
                            {
                                case Endianness.LittleEndian:
                                    bytes.Add((byte)((int)value & 0xFF));
                                    bytes.Add((byte)((int)value >> 8 & 0xFF));
                                    bytes.Add((byte)((int)value >> 16 & 0xFF));
                                    bytes.Add((byte)((int)value >> 24 & 0xFF));
                                    break;
                                case Endianness.BigEndian:
                                    bytes.Add((byte)((int)value >> 24 & 0xFF));
                                    bytes.Add((byte)((int)value >> 16 & 0xFF));
                                    bytes.Add((byte)((int)value >> 8 & 0xFF));
                                    bytes.Add((byte)((int)value & 0xFF));
                                    break;
                            }
                            break;
                        case TypeCode.UInt32:
                            switch (endian)
                            {
                                case Endianness.LittleEndian:
                                    bytes.Add((byte)((uint)value & 0xFF));
                                    bytes.Add((byte)((uint)value >> 8 & 0xFF));
                                    bytes.Add((byte)((uint)value >> 16 & 0xFF));
                                    bytes.Add((byte)((uint)value >> 24 & 0xFF));
                                    break;
                                case Endianness.BigEndian:
                                    bytes.Add((byte)((uint)value >> 24 & 0xFF));
                                    bytes.Add((byte)((uint)value >> 16 & 0xFF));
                                    bytes.Add((byte)((uint)value >> 8 & 0xFF));
                                    bytes.Add((byte)((uint)value & 0xFF));
                                    break;
                            }
                            break;
                        case TypeCode.Int16:
                            switch (endian)
                            {
                                case Endianness.LittleEndian:
                                    bytes.Add((byte)((short)value & 0xFF));
                                    bytes.Add((byte)((short)value >> 8 & 0xFF));
                                    break;
                                case Endianness.BigEndian:
                                    bytes.Add((byte)((short)value >> 8 & 0xFF));
                                    bytes.Add((byte)((short)value & 0xFF));
                                    break;
                            }
                            break;
                        case TypeCode.UInt16:
                            switch (endian)
                            {
                                case Endianness.LittleEndian:
                                    bytes.Add((byte)((ushort)value & 0xFF));
                                    bytes.Add((byte)((ushort)value >> 8 & 0xFF));
                                    break;
                                case Endianness.BigEndian:
                                    bytes.Add((byte)((ushort)value >> 8 & 0xFF));
                                    bytes.Add((byte)((ushort)value & 0xFF));
                                    break;
                            }
                            break;
                        case TypeCode.Char:
                            switch (endian)
                            {
                                case Endianness.LittleEndian:
                                    bytes.Add((byte)((char)value & 0xFF));
                                    bytes.Add((byte)((char)value >> 8 & 0xFF));
                                    break;
                                case Endianness.BigEndian:
                                    bytes.Add((byte)((char)value >> 8 & 0xFF));
                                    bytes.Add((byte)((char)value & 0xFF));
                                    break;
                            }
                            break;
                        case TypeCode.Byte:
                            bytes.Add((byte)value);
                            break;
                        case TypeCode.SByte:
                            bytes.Add((byte)((sbyte)value));
                            break;
                        case TypeCode.Single:
                            byte[] floatBytes = BitConverter.GetBytes((float)value);
                            if (BitConverter.IsLittleEndian ^ endian == Endianness.LittleEndian)
                                floatBytes = floatBytes.Reverse().ToArray();
                            bytes.AddRange(floatBytes);
                            break;
                    }
                }
            }

            return bytes.ToArray();
        }

    }
}
