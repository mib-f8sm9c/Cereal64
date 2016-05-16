using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cereal64.Microcodes;
using Cereal64.Common;
using Cereal64.Microcodes.F3DZEX.DataElements.Commands;

namespace Cereal64.Microcodes.UnitTests.F3DZEX
{
    [TestClass]
    public class F3DZEXCommandTests
    {

        #region 0x0 codes

        //0x00
        [TestMethod]
        public void TestF3DZEX_G_NoOp()
        {
            //00000000 12345678
            byte[] testVal = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_NoOp cmd = new F3DZEX_G_NoOp(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //00FFFFFF 12345678
            testVal = new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0x12, 0x34, 0x56, 0x78 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0x01
        [TestMethod]
        public void TestF3DZEX_G_Vtx()
        {
            //0101C002 12345678
            byte[] testVal = new byte[] { 0x01, 0x01, 0xC0, 0x02, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_Vtx cmd = new F3DZEX_G_Vtx(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //01F1CF02 12345678
            testVal = new byte[] { 0x01, 0xF1, 0xCF, 0x02, 0x12, 0x34, 0x56, 0x78 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0x02
        [TestMethod]
        public void TestF3DZEX_G_ModifyVtx()
        {
            //02141C1C 12345678
            byte[] testVal = new byte[] { 0x02, 0x14, 0x1C, 0x1C, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_ModifyVtx cmd = new F3DZEX_G_ModifyVtx(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);
        }

        //0x03
        [TestMethod]
        public void TestF3DZEX_G_CullDL()
        {
            //03001C1C 00002E2E
            byte[] testVal = new byte[] { 0x03, 0x00, 0x1C, 0x1C, 0x00, 0x00, 0x2E, 0x2E };

            F3DZEX_G_CullDL cmd = new F3DZEX_G_CullDL(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //03FF1C1C FFFF2E2E
            testVal = new byte[] { 0x03, 0xFF, 0x1C, 0x1C, 0xFF, 0xFF, 0x2E, 0x2E };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0x04
        [TestMethod]
        public void TestF3DZEX_G_Branch_Z()
        {
            //042C611C 12345678
            byte[] testVal = new byte[] { 0x04, 0x2C, 0x61, 0x1C, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_Branch_Z cmd = new F3DZEX_G_Branch_Z(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);
        }

        //0x05
        [TestMethod]
        public void TestF3DZEX_G_Tri1()
        {
            //053A3C3E 00000000
            byte[] testVal = new byte[] { 0x05, 0x3A, 0x3C, 0x3E, 0x00, 0x00, 0x00, 0x00 };

            F3DZEX_G_Tri1 cmd = new F3DZEX_G_Tri1(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //053A3C3E FFFFFFFF
            testVal = new byte[] { 0x05, 0x3A, 0x3C, 0x3E, 0xFF, 0xFF, 0xFF, 0xFF };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0x06
        [TestMethod]
        public void TestF3DZEX_G_Tri2()
        {
            //063A3C3E 0024282C
            byte[] testVal = new byte[] { 0x06, 0x3A, 0x3C, 0x3E, 0x00, 0x24, 0x28, 0x2C };

            F3DZEX_G_Tri2 cmd = new F3DZEX_G_Tri2(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //063A3C3E FF24282C
            testVal = new byte[] { 0x06, 0x3A, 0x3C, 0x3E, 0xFF, 0x24, 0x28, 0x2C };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0x07
        [TestMethod]
        public void TestF3DZEX_G_Quad()
        {
            //073A3C3E 003A3E2C
            byte[] testVal = new byte[] { 0x07, 0x3A, 0x3C, 0x3E, 0x00, 0x3A, 0x3E, 0x2C };

            F3DZEX_G_Quad cmd = new F3DZEX_G_Quad(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //073A3C3E FF3A3E2C
            testVal = new byte[] { 0x07, 0x3A, 0x3C, 0x3E, 0xFF, 0x3A, 0x3E, 0x2C };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        #endregion

        #region 0xD codes

        //0xD3
        [TestMethod]
        public void TestF3DZEX_G_Special_3()
        {
            //D3141C1C 12345678
            byte[] testVal = new byte[] { 0xD3, 0x14, 0x1C, 0x1C, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_Special_3 cmd = new F3DZEX_G_Special_3(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xD4
        [TestMethod]
        public void TestF3DZEX_G_Special_2()
        {
            //D4141C1C 12345678
            byte[] testVal = new byte[] { 0xD4, 0x14, 0x1C, 0x1C, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_Special_2 cmd = new F3DZEX_G_Special_2(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xD5
        [TestMethod]
        public void TestF3DZEX_G_Special_1()
        {
            //D5141C1C 12345678
            byte[] testVal = new byte[] { 0xD5, 0x14, 0x1C, 0x1C, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_Special_1 cmd = new F3DZEX_G_Special_1(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xD6
        [TestMethod]
        public void TestF3DZEX_G_DMA_IO()
        {
            //D6142C1C 12345678
            byte[] testVal = new byte[] { 0xD6, 0x14, 0x2C, 0x1C, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_DMA_IO cmd = new F3DZEX_G_DMA_IO(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //D6143C1C 12345678
            testVal = new byte[] { 0xD6, 0x14, 0x3C, 0x1C, 0x12, 0x34, 0x56, 0x78 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xD7
        [TestMethod]
        public void TestF3DZEX_G_Texture()
        {
            //D7001B01 12345678
            byte[] testVal = new byte[] { 0xD7, 0x00, 0x1B, 0x01, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_Texture cmd = new F3DZEX_G_Texture(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //D7FF1B01 12345678
            testVal = new byte[] { 0xD7, 0xFF, 0x1B, 0x01, 0x12, 0x34, 0x56, 0x78 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xD8
        [TestMethod]
        public void TestF3DZEX_G_PopMtx()
        {
            //D8380002 12345600
            byte[] testVal = new byte[] { 0xD8, 0x38, 0x00, 0x02, 0x12, 0x34, 0x56, 0x00 };

            F3DZEX_G_PopMtx cmd = new F3DZEX_G_PopMtx(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //D8FFFFFF 12345601
            testVal = new byte[] { 0xD8, 0xFF, 0xFF, 0xFF, 0x12, 0x34, 0x56, 0x01 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xD9
        [TestMethod]
        public void TestF3DZEX_G_GeometryMode()
        {
            //D9123456 789ABCDE
            byte[] testVal = new byte[] { 0xD9, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_GeometryMode cmd = new F3DZEX_G_GeometryMode(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xDA
        [TestMethod]
        public void TestF3DZEX_G_Mtx()
        {
            //DA380007 789ABCDE
            byte[] testVal = new byte[] { 0xDA, 0x38, 0x00, 0x07, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_Mtx cmd = new F3DZEX_G_Mtx(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //DAFFFF07 789ABCDE
            testVal = new byte[] { 0xDA, 0xFF, 0xFF, 0x07, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xDB
        [TestMethod]
        public void TestF3DZEX_G_MoveWord()
        {
            //DB123456 789ABCDE
            byte[] testVal = new byte[] { 0xDB, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_MoveWord cmd = new F3DZEX_G_MoveWord(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xDC
        [TestMethod]
        public void TestF3DZEX_G_MoveMem()
        {
            //DC18340E 789ABCDE
            byte[] testVal = new byte[] { 0xDC, 0x18, 0x34, 0x0E, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_MoveMem cmd = new F3DZEX_G_MoveMem(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xDD
        [TestMethod]
        public void TestF3DZEX_G_Load_UCode()
        {
            //DD003456 789ABCDE
            byte[] testVal = new byte[] { 0xDD, 0x00, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_Load_UCode cmd = new F3DZEX_G_Load_UCode(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //DDFF3456 789ABCDE
            testVal = new byte[] { 0xDD, 0xFF, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xDE
        [TestMethod]
        public void TestF3DZEX_G_DL()
        {
            //DE010000 789ABCDE
            byte[] testVal = new byte[] { 0xDE, 0x01, 0x00, 0x00, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_DL cmd = new F3DZEX_G_DL(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //DE01FFFF 789ABCDE
            testVal = new byte[] { 0xDE, 0xFF, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        //0xDF
        [TestMethod]
        public void TestF3DZEX_G_EndDL()
        {
            //DF000000 00000000
            byte[] testVal = new byte[] { 0xDF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            F3DZEX_G_EndDL cmd = new F3DZEX_G_EndDL(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
            Assert.IsTrue(cmd.IsValid);

            //DFFFFFFF FFFFFFFF
            testVal = new byte[] { 0xDF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
            Assert.IsTrue(cmd.IsValid);
        }

        #endregion

        #region 0xE codes

        //0xE0
        [TestMethod]
        public void TestF3DZEX_G_SPNoOp()
        {
            //E0000000 00000000
            byte[] testVal = new byte[] { 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            F3DZEX_G_SPNoOp cmd = new F3DZEX_G_SPNoOp(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E0FFFFFF FFFFFFFF
            testVal = new byte[] { 0xE0, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE1
        [TestMethod]
        public void TestF3DZEX_G_RDPHalf_1()
        {
            //E1000000 12345678
            byte[] testVal = new byte[] { 0xE1, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_RDPHalf_1 cmd = new F3DZEX_G_RDPHalf_1(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E1FFFFFF 12345678
            testVal = new byte[] { 0xE1, 0xFF, 0xFF, 0xFF, 0x12, 0x34, 0x56, 0x78 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE2
        [TestMethod]
        public void TestF3DZEX_G_SetOtherMode_L()
        {
            //E20002345 12345678
            byte[] testVal = new byte[] { 0xE2, 0x00, 0x02, 0x34, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_SetOtherMode_L cmd = new F3DZEX_G_SetOtherMode_L(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E2FF0234 12345678
            testVal = new byte[] { 0xE2, 0xFF, 0x02, 0x34, 0x12, 0x34, 0x56, 0x78 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE3
        [TestMethod]
        public void TestF3DZEX_G_SetOtherMode_H()
        {
            //E30004345 12345678
            byte[] testVal = new byte[] { 0xE3, 0x00, 0x04, 0x34, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_SetOtherMode_H cmd = new F3DZEX_G_SetOtherMode_H(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E3FF0434 12345678
            testVal = new byte[] { 0xE3, 0xFF, 0x04, 0x34, 0x12, 0x34, 0x56, 0x78 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE4
        [TestMethod]
        public void TestF3DZEX_G_TexRect()
        {
            //E4123456 0F56789A
            byte[] testVal = new byte[] { 0xE4, 0x12, 0x34, 0x56, 0x0F, 0x56, 0x78, 0x9A };

            F3DZEX_G_TexRect cmd = new F3DZEX_G_TexRect(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E4123456 FF56789A
            testVal = new byte[] { 0xE4, 0x12, 0x34, 0x56, 0xFF, 0x56, 0x78, 0x9A };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE5
        [TestMethod]
        public void TestF3DZEX_G_TexRectFlip()
        {
            //E5123456 0F56789A
            byte[] testVal = new byte[] { 0xE5, 0x12, 0x34, 0x56, 0x0F, 0x56, 0x78, 0x9A };

            F3DZEX_G_TexRectFlip cmd = new F3DZEX_G_TexRectFlip(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E5123456 FF56789A
            testVal = new byte[] { 0xE5, 0x12, 0x34, 0x56, 0xFF, 0x56, 0x78, 0x9A };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE6
        [TestMethod]
        public void TestF3DZEX_G_RDPLoadSync()
        {
            //E6000000 00000000
            byte[] testVal = new byte[] { 0xE6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            F3DZEX_G_RDPLoadSync cmd = new F3DZEX_G_RDPLoadSync(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E6FFFFFF FFFFFFFF
            testVal = new byte[] { 0xE6, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE7
        [TestMethod]
        public void TestF3DZEX_G_RDPPipeSync()
        {
            //E7000000 00000000
            byte[] testVal = new byte[] { 0xE7, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            F3DZEX_G_RDPPipeSync cmd = new F3DZEX_G_RDPPipeSync(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E7FFFFFF FFFFFFFF
            testVal = new byte[] { 0xE7, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE8
        [TestMethod]
        public void TestF3DZEX_G_RDPTileSync()
        {
            //E8000000 00000000
            byte[] testVal = new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            F3DZEX_G_RDPTileSync cmd = new F3DZEX_G_RDPTileSync(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E8FFFFFF FFFFFFFF
            testVal = new byte[] { 0xE8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xE9
        [TestMethod]
        public void TestF3DZEX_G_RDPFullSync()
        {
            //E9000000 00000000
            byte[] testVal = new byte[] { 0xE9, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            F3DZEX_G_RDPFullSync cmd = new F3DZEX_G_RDPFullSync(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //E9FFFFFF FFFFFFFF
            testVal = new byte[] { 0xE9, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xEA
        [TestMethod]
        public void TestF3DZEX_G_SetKeyGB()
        {
            //EA123456 789ABCDE
            byte[] testVal = new byte[] { 0xEA, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetKeyGB cmd = new F3DZEX_G_SetKeyGB(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
        }

        //0xEB
        [TestMethod]
        public void TestF3DZEX_G_SetKeyR()
        {
            //EB000000 089ABCDE
            byte[] testVal = new byte[] { 0xEB, 0x00, 0x00, 0x00, 0x08, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetKeyR cmd = new F3DZEX_G_SetKeyR(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //EBFFFFFF 089ABCDE
            testVal = new byte[] { 0xEB, 0xFF, 0xFF, 0xFF, 0x08, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xEC
        [TestMethod]
        public void TestF3DZEX_G_SetConvert()
        {
            //EC023456 789ABCDE
            byte[] testVal = new byte[] { 0xEC, 0x02, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetConvert cmd = new F3DZEX_G_SetConvert(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //EC123456 789ABCDE
            testVal = new byte[] { 0xEC, 0x42, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xED
        [TestMethod]
        public void TestF3DZEX_G_SetScissor()
        {
            //ED123456 209ABCDE
            byte[] testVal = new byte[] { 0xED, 0x12, 0x34, 0x56, 0x20, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetScissor cmd = new F3DZEX_G_SetScissor(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //ED123456 2F9ABCDE
            testVal = new byte[] { 0xED, 0x12, 0x34, 0x56, 0x2F, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xEE
        [TestMethod]
        public void TestF3DZEX_G_SetPrimDepth()
        {
            //EE000000 789ABCDE
            byte[] testVal = new byte[] { 0xEE, 0x00, 0x00, 0x00, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetPrimDepth cmd = new F3DZEX_G_SetPrimDepth(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //EEFFFFFF 789ABCDE
            testVal = new byte[] { 0xEE, 0xFF, 0xFF, 0xFF, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xEF
        [TestMethod]
        public void TestF3DZEX_G_RDPSetOtherMode()
        {
            //EF123456 789ABCDE
            byte[] testVal = new byte[] { 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_RDPSetOtherMode cmd = new F3DZEX_G_RDPSetOtherMode(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
        }

        #endregion

        #region 0xF codes

        //0xF0
        [TestMethod]
        public void TestF3DZEX_G_LoadTLut()
        {
            //F0000000 01234000
            byte[] testVal = new byte[] { 0xF0, 0x00, 0x00, 0x00, 0x01, 0x23, 0x40, 0x00 };

            F3DZEX_G_LoadTLut cmd = new F3DZEX_G_LoadTLut(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F0FFFFFF FFFFFFFF
            testVal = new byte[] { 0xF0, 0xFF, 0xFF, 0xFF, 0xF1, 0x23, 0x4F, 0xFF };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF1
        [TestMethod]
        public void TestF3DZEX_G_RDPHalf_2()
        {
            //F1000000 12345678
            byte[] testVal = new byte[] { 0xF1, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_RDPHalf_2 cmd = new F3DZEX_G_RDPHalf_2(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F1FFFFFF 12345678
            testVal = new byte[] { 0xF1, 0xFF, 0xFF, 0xFF, 0x12, 0x34, 0x56, 0x78 };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF2
        [TestMethod]
        public void TestF3DZEX_G_SetTileSize()
        {
            //F2123456 029ABCDE
            byte[] testVal = new byte[] { 0xF2, 0x12, 0x34, 0x56, 0x02, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetTileSize cmd = new F3DZEX_G_SetTileSize(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F2123456 F29ABCDE
            testVal = new byte[] { 0xF2, 0x12, 0x34, 0x56, 0xF2, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF3
        [TestMethod]
        public void TestF3DZEX_G_LoadBlock()
        {
            //F3123456 029ABCDE
            byte[] testVal = new byte[] { 0xF3, 0x12, 0x34, 0x56, 0x02, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_LoadBlock cmd = new F3DZEX_G_LoadBlock(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F3123456 F29ABCDE
            testVal = new byte[] { 0xF3, 0x12, 0x34, 0x56, 0xF2, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF4
        [TestMethod]
        public void TestF3DZEX_G_LoadTile()
        {
            //F4123456 029ABCDE
            byte[] testVal = new byte[] { 0xF4, 0x12, 0x34, 0x56, 0x02, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_LoadTile cmd = new F3DZEX_G_LoadTile(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F4123456 F29ABCDE
            testVal = new byte[] { 0xF4, 0x12, 0x34, 0x56, 0xF2, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF5
        [TestMethod]
        public void TestF3DZEX_G_SetTile()
        {
            //F52A3456 079ABCDE
            byte[] testVal = new byte[] { 0xF5, 0x2A, 0x34, 0x56, 0x07, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetTile cmd = new F3DZEX_G_SetTile(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F52E3456 FF9ABCDE
            testVal = new byte[] { 0xF5, 0x2E, 0x34, 0x56, 0xFF, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF6
        [TestMethod]
        public void TestF3DZEX_G_FillRect()
        {
            //F6123456 009ABCDE
            byte[] testVal = new byte[] { 0xF6, 0x12, 0x34, 0x56, 0x00, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_FillRect cmd = new F3DZEX_G_FillRect(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F6123456 FF9ABCDE
            testVal = new byte[] { 0xF6, 0x12, 0x34, 0x56, 0xFF, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF7
        [TestMethod]
        public void TestF3DZEX_G_SetFillColor()
        {
            //F7000000 789ABCDE
            byte[] testVal = new byte[] { 0xF7, 0x00, 0x00, 0x00, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetFillColor cmd = new F3DZEX_G_SetFillColor(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F7FFFFFF 789ABCDE
            testVal = new byte[] { 0xF7, 0xFF, 0xFF, 0xFF, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF8
        [TestMethod]
        public void TestF3DZEX_G_SetFogColor()
        {
            //F8000000 789ABCDE
            byte[] testVal = new byte[] { 0xF8, 0x00, 0x00, 0x00, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetFogColor cmd = new F3DZEX_G_SetFogColor(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F8FFFFFF 789ABCDE
            testVal = new byte[] { 0xF8, 0xFF, 0xFF, 0xFF, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xF9
        [TestMethod]
        public void TestF3DZEX_G_SetBlendColor()
        {
            //F9000000 789ABCDE
            byte[] testVal = new byte[] { 0xF9, 0x00, 0x00, 0x00, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetBlendColor cmd = new F3DZEX_G_SetBlendColor(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //F9FFFFFF 789ABCDE
            testVal = new byte[] { 0xF9, 0xFF, 0xFF, 0xFF, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xFA
        [TestMethod]
        public void TestF3DZEX_G_SetPrimColor()
        {
            //FA003456 789ABCDE
            byte[] testVal = new byte[] { 0xFA, 0x00, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetPrimColor cmd = new F3DZEX_G_SetPrimColor(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //FAFF3456 789ABCDE
            testVal = new byte[] { 0xFA, 0xFF, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xFB
        [TestMethod]
        public void TestF3DZEX_G_SetEnvColor()
        {
            //FB000000 789ABCDE
            byte[] testVal = new byte[] { 0xFB, 0x00, 0x00, 0x00, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetEnvColor cmd = new F3DZEX_G_SetEnvColor(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //FBFFFFFF 789ABCDE
            testVal = new byte[] { 0xFB, 0xFF, 0xFF, 0xFF, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xFC
        [TestMethod]
        public void TestF3DZEX_G_SetCombine()
        {
            //FC141C1C 12345678
            byte[] testVal = new byte[] { 0xFC, 0x14, 0x1C, 0x1C, 0x12, 0x34, 0x56, 0x78 };

            F3DZEX_G_SetCombine cmd = new F3DZEX_G_SetCombine(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));
        }

        //0xFD
        [TestMethod]
        public void TestF3DZEX_G_SetTImg()
        {
            //FD280123 789ABCDE
            byte[] testVal = new byte[] { 0xFD, 0x28, 0x01, 0x23, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetTImg cmd = new F3DZEX_G_SetTImg(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //FD28F123 789ABCDE
            testVal = new byte[] { 0xFD, 0x28, 0xF1, 0x23, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xFE
        [TestMethod]
        public void TestF3DZEX_G_SetZImg()
        {
            //FE000000 789ABCDE
            byte[] testVal = new byte[] { 0xFE, 0x00, 0x00, 0x00, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetZImg cmd = new F3DZEX_G_SetZImg(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //FEFFFFFF 789ABCDE
            testVal = new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        //0xFF
        [TestMethod]
        public void TestF3DZEX_G_SetCImg()
        {
            //FF280123 789ABCDE
            byte[] testVal = new byte[] { 0xFF, 0x28, 0x01, 0x23, 0x78, 0x9A, 0xBC, 0xDE };

            F3DZEX_G_SetCImg cmd = new F3DZEX_G_SetCImg(0, testVal);

            byte[] outVal = cmd.RawData;

            Assert.AreEqual(testVal.Length, outVal.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(testVal, outVal));

            //FF28F123 789ABCDE
            testVal = new byte[] { 0xFF, 0x28, 0xF1, 0x23, 0x78, 0x9A, 0xBC, 0xDE };

            cmd.RawData = testVal;

            byte[] outVal2 = cmd.RawData;

            Assert.AreEqual(outVal.Length, outVal2.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(outVal, outVal2));
        }

        #endregion

    }
}
