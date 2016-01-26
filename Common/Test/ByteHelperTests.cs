using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Common.UnitTests
{
    [TestClass]
    public class ByteHelperTests
    {

        [TestMethod]
        public void DisplayValue()
        {
            int intV = 66740;
            int negIntV = -8844933;
            uint uintV = 99334857;

            //Hex
            string val = ByteHelper.DisplayValue(intV, true, false);
            string expected = "0x104B4";
            Assert.AreEqual(expected, val);

            val = ByteHelper.DisplayValue(intV, true, true);
            expected = "0x000104B4";
            Assert.AreEqual(expected, val);

            val = ByteHelper.DisplayValue(negIntV, true);
            expected = "0xFF79097B";
            Assert.AreEqual(expected, val);

            val = ByteHelper.DisplayValue(uintV, true);
            expected = "0x5EBBAC9";
            Assert.AreEqual(expected, val);

            //Dec
            val = ByteHelper.DisplayValue(intV, false);
            expected = "66740";
            Assert.AreEqual(expected, val);

            val = ByteHelper.DisplayValue(negIntV, false);
            expected = "-8844933";
            Assert.AreEqual(expected, val);

            val = ByteHelper.DisplayValue(uintV, false);
            expected = "99334857";
            Assert.AreEqual(expected, val);
        }

        #region ReadingTests

        [TestMethod]
        public void TestReadInt()
        {
            byte[] bytes = new byte[] { 0xFF, 0x80, 0x37, 0x12, 0x40, 0xFF };
            //Big endian
            int expected = unchecked((int)0x80371240);
            int testVal = ByteHelper.ReadInt(bytes, 1, Endianness.BigEndian);
            Assert.AreEqual(expected, testVal);

            //Little endian
            expected = unchecked((int)0x40123780);
            testVal = ByteHelper.ReadInt(bytes, 1, Endianness.LittleEndian);
            Assert.AreEqual(expected, testVal);
        }

        [TestMethod]
        public void TestReadUInt()
        {
            byte[] bytes = new byte[] { 0xFF, 0x80, 0x37, 0x12, 0x40, 0xFF };
            //Big endian
            uint expected = 0x80371240;
            uint testVal = ByteHelper.ReadUInt(bytes, 1, Endianness.BigEndian);
            Assert.AreEqual(expected, testVal);

            //Little endian
            expected = 0x40123780;
            testVal = ByteHelper.ReadUInt(bytes, 1, Endianness.LittleEndian);
            Assert.AreEqual(expected, testVal);
        }

        [TestMethod]
        public void TestReadShort()
        {
            byte[] bytes = new byte[] { 0xFF, 0x80, 0x37, 0x12, 0x40, 0xFF };
            //Big endian
            short expected = unchecked((short)0x8037);
            short testVal = ByteHelper.ReadShort(bytes, 1, Endianness.BigEndian);
            Assert.AreEqual(expected, testVal);

            //Little endian
            expected = unchecked((short)0x3780);
            testVal = ByteHelper.ReadShort(bytes, 1, Endianness.LittleEndian);
            Assert.AreEqual(expected, testVal);
        }

        [TestMethod]
        public void TestReadUShort()
        {
            byte[] bytes = new byte[] { 0xFF, 0x80, 0x37, 0x12, 0x40, 0xFF };
            //Big endian
            ushort expected = (ushort)0x8037;
            ushort testVal = ByteHelper.ReadUShort(bytes, 1, Endianness.BigEndian);
            Assert.AreEqual(expected, testVal);

            //Little endian
            expected = (ushort)0x3780;
            testVal = ByteHelper.ReadUShort(bytes, 1, Endianness.LittleEndian);
            Assert.AreEqual(expected, testVal);
        }

        [TestMethod]
        public void TestReadSByte()
        {
            byte[] bytes = new byte[] { 0xFF, 0x80, 0x37, 0x12, 0x40, 0xFF };
            //Big endian
            sbyte expected = unchecked((sbyte)0x80);
            sbyte testVal = ByteHelper.ReadSByte(bytes, 1, Endianness.BigEndian);
            Assert.AreEqual(expected, testVal);

            //Little endian
            testVal = ByteHelper.ReadSByte(bytes, 1, Endianness.LittleEndian);
            Assert.AreEqual(expected, testVal);
        }

        [TestMethod]
        public void TestReadByte()
        {
            byte[] bytes = new byte[] { 0xFF, 0x80, 0x37, 0x12, 0x40, 0xFF };
            //Big endian
            byte expected = (byte)0x80;
            byte testVal = ByteHelper.ReadByte(bytes, 1, Endianness.BigEndian);
            Assert.AreEqual(expected, testVal);

            //Little endian
            testVal = ByteHelper.ReadByte(bytes, 1, Endianness.LittleEndian);
            Assert.AreEqual(expected, testVal);
        }

        [TestMethod]
        public void TestReadFloat()
        {
            byte[] bytes = new byte[] { 0xFF, 0x42, 0xFA, 0xB0, 0x3B, 0xFF };
            //Big endian
            float expected = (float)125.344200;
            float testVal = ByteHelper.ReadFloat(bytes, 1, Endianness.BigEndian);
            Assert.AreEqual(expected, testVal, 0.00001);

            //Little endian
            bytes = bytes.Reverse().ToArray(); //Flip around for little endian
            testVal = ByteHelper.ReadFloat(bytes, 1, Endianness.LittleEndian);
            Assert.AreEqual(expected, testVal, 0.00001);
        }

        #endregion

        #region WritingTests

        [TestMethod]
        public void TestWriteInt()
        {
            byte[] bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] ExpectedBytes = new byte[] { 0xFF, 0x80, 0x37, 0x12, 0x40, 0xFF };

            //Big endian
            ByteHelper.WriteInt(unchecked((int)0x80371240),
                bytes, 1, Endianness.BigEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0xFF, 0x40, 0x12, 0x37, 0x80, 0xFF };
            ByteHelper.WriteInt(unchecked((int)0x80371240),
                bytes, 1, Endianness.LittleEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);
        }

        [TestMethod]
        public void TestWriteUInt()
        {
            byte[] bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] ExpectedBytes = new byte[] { 0xFF, 0x80, 0x37, 0x12, 0x40, 0xFF };

            //Big endian
            ByteHelper.WriteUInt((uint)0x80371240,
                bytes, 1, Endianness.BigEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0xFF, 0x40, 0x12, 0x37, 0x80, 0xFF };
            ByteHelper.WriteUInt((uint)0x80371240,
                bytes, 1, Endianness.LittleEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);
        }

        [TestMethod]
        public void TestWriteShort()
        {
            byte[] bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] ExpectedBytes = new byte[] { 0xFF, 0x80, 0x37, 0xFF, 0xFF, 0xFF };

            //Big endian
            ByteHelper.WriteShort(unchecked((short)0x8037),
                bytes, 1, Endianness.BigEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0xFF, 0x80, 0x37, 0x40, 0x12, 0xFF };
            ByteHelper.WriteShort(unchecked((short)0x1240),
                bytes, 3, Endianness.LittleEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);
        }

        [TestMethod]
        public void TestWriteUShort()
        {
            byte[] bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] ExpectedBytes = new byte[] { 0xFF, 0x80, 0x37, 0xFF, 0xFF, 0xFF };

            //Big endian
            ByteHelper.WriteUShort((ushort)0x8037,
                bytes, 1, Endianness.BigEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0xFF, 0x80, 0x37, 0x40, 0x12, 0xFF };
            ByteHelper.WriteUShort((ushort)0x1240,
                bytes, 3, Endianness.LittleEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);
        }

        [TestMethod]
        public void TestWriteSByte()
        {
            byte[] bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] ExpectedBytes = new byte[] { 0xFF, 0x80, 0xFF, 0xFF, 0xFF, 0xFF };

            //Big endian
            ByteHelper.WriteSByte(unchecked((sbyte)0x80),
                bytes, 1, Endianness.BigEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0xFF, 0x80, 0x37, 0xFF, 0xFF, 0xFF };
            ByteHelper.WriteSByte(unchecked((sbyte)0x37),
                bytes, 2, Endianness.LittleEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);
        }

        [TestMethod]
        public void TestWriteByte()
        {
            byte[] bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] ExpectedBytes = new byte[] { 0xFF, 0x80, 0xFF, 0xFF, 0xFF, 0xFF };

            //Big endian
            ByteHelper.WriteByte((byte)0x80,
                bytes, 1, Endianness.BigEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0xFF, 0x80, 0x37, 0xFF, 0xFF, 0xFF };
            ByteHelper.WriteByte((byte)0x37,
                bytes, 2, Endianness.LittleEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);
        }

        [TestMethod]
        public void TestWriteFloat()
        {
            byte[] bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] ExpectedBytes = new byte[] { 0xFF, 0x42, 0xFA, 0xB0, 0x3B, 0xFF };

            //Big endian
            ByteHelper.WriteFloat((float)125.344200,
                bytes, 1, Endianness.BigEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0xFF, 0x3B, 0xB0, 0xFA, 0x42, 0xFF };
            ByteHelper.WriteFloat((float)125.344200,
                bytes, 1, Endianness.LittleEndian);
            CollectionAssert.AreEqual(ExpectedBytes, bytes);
        }

        #endregion

        [TestMethod]
        public void CombineIntoBytes1()
        {
            //Test each data type in a long chained list
            int intV = (int)0x12345678;
            uint uintV = (uint)0x9ABCDEF0;
            short shortV = (short)0x1234;
            ushort ushortV = (ushort)0x5678;
            byte byteV = (byte)0x9A;
            sbyte sbyteV = unchecked((sbyte)0xBC);
            float floatV = (float)125.344200;


            //Big endian
            byte[] ExpectedBytes = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
            0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0x42, 0xFA, 0xB0, 0x3B };

            byte[] bytes = ByteHelper.CombineIntoBytes(Endianness.BigEndian, intV, uintV, shortV, ushortV, 
                byteV, sbyteV, floatV);

            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0x78, 0x56, 0x34, 0x12, 0xF0, 0xDE, 0xBC, 0x9A,
            0x34, 0x12, 0x78, 0x56, 0x9A, 0xBC, 0x3B, 0xB0, 0xFA, 0x42 };

            bytes = ByteHelper.CombineIntoBytes(Endianness.LittleEndian, intV, uintV, shortV, ushortV,
                byteV, sbyteV, floatV);

            CollectionAssert.AreEqual(ExpectedBytes, bytes);
        }

        [TestMethod]
        public void CombineIntoBytes2()
        {
            //Test with passing in lists, arrays, and lists of lists of bytes
            List<byte> byteList = new List<byte>();
            byteList.Add(0x80);
            byteList.Add(0x17);
            ushort[] ushortArray = new ushort[2] { 0xFFFE, 0xFFFD };

            List<List<byte>> byteListList = new List<List<byte>>();
            byteListList.Add(new List<byte>());
            byteListList[0].Add(0x34);
            byteListList.Add(new List<byte>());
            byteListList[1].Add(0x56);
            byteListList[1].Add(0x78);


            //Big endian
            byte[] ExpectedBytes = new byte[] { 0x80, 0x17, 0xFF, 0xFE, 0xFF, 0xFD, 0x34, 0x56, 0x78 };

            byte[] bytes = ByteHelper.CombineIntoBytes(Endianness.BigEndian, byteList, ushortArray, byteListList);

            CollectionAssert.AreEqual(ExpectedBytes, bytes);

            //Little endian
            ExpectedBytes = new byte[] { 0x80, 0x17, 0xFE, 0xFF, 0xFD, 0xFF, 0x34, 0x56, 0x78 };

            bytes = ByteHelper.CombineIntoBytes(Endianness.LittleEndian, byteList, ushortArray, byteListList);

            CollectionAssert.AreEqual(ExpectedBytes, bytes);

        }


    }
}
