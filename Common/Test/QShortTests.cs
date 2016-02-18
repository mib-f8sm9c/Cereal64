using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cereal64.Common.Utils;

namespace Cereal64.Common.UnitTests
{
    [TestClass]
    public class QShortTests
    {
        [TestMethod]
        public void QShortTest1()
        {
            qshort qs = new qshort("4.8", 0.5);
            Assert.AreEqual(4, qs.IntegerBitCount);
            Assert.AreEqual(8, qs.FractionalBitCount);
            Assert.AreEqual(0.5, qs.Value);
            Assert.AreEqual(128, qs.RawValue);

            qs.Value = -9.375;
            Assert.AreEqual(-9.375, qs.Value);
            Assert.AreEqual(5792, qs.RawValue);

            qs.Value = -20; //Truncation, should be (-20) - (-16) = (-4)
            Assert.IsTrue(qs.Value < 0);

            qs.Value = 0.50000001;
            Assert.AreEqual(0.5, qs.Value);
        }

        [TestMethod]
        public void QUShortTest1()
        {
            qushort qs = new qushort("4.8", 0.5);
            Assert.AreEqual(4, qs.IntegerBitCount);
            Assert.AreEqual(8, qs.FractionalBitCount);
            Assert.AreEqual(0.5, qs.Value);
            Assert.AreEqual(128, qs.RawValue);

            qs.Value = 9.375;
            Assert.AreEqual(9.375, qs.Value);
            Assert.AreEqual(2400, qs.RawValue);

            qs.Value = 0.50000001;
            Assert.AreEqual(0.5, qs.Value);
        }
    }
}
