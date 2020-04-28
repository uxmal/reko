using NUnit.Framework;
using System;
using Reko.Libraries.Microchip;

namespace Reko.UnitTests.Arch.Microchip
{
    [TestFixture]
    public class ConvUtilsTests
    {

        [Test]
        public void ConvUtils_ToUInt32()
        {

            Assert.AreEqual(0U, ConvUtils.ToUInt32Ex(""), "Result differs");
            Assert.AreEqual(55U, ConvUtils.ToUInt32Ex("55"), "Result differs");
            Assert.AreEqual(0x55U, ConvUtils.ToUInt32Ex("0x55"), "Result differs");
            Assert.AreEqual(0x55U, ConvUtils.ToUInt32Ex("0X55"), "Result differs");
            Assert.AreEqual(3U, ConvUtils.ToUInt32Ex("0b11"), "Result differs");
            Assert.AreEqual(5U, ConvUtils.ToUInt32Ex("0B101"), "Result differs");
            Assert.AreEqual(0xFEDCBAU, ConvUtils.ToUInt32Ex("0XFEDCBA"), "Result differs");

            uint? nres;
            nres = ConvUtils.ToNullableUInt32Ex(null);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableUInt32Ex(String.Empty);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableUInt32Ex("");
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableUInt32Ex("0xAA");
            Assert.AreEqual(0xAAU, nres, "Result differs");

            Assert.Throws<OverflowException>(() => ConvUtils.ToUInt32Ex("0xFFFFFFFFF"));
            Assert.Throws<OverflowException>(() => ConvUtils.ToUInt32Ex("-1"));
        }

        [Test]
        public void ConvUtils_ToInt32()
        {

            Assert.AreEqual(0, ConvUtils.ToInt32Ex(""), "Result differs");
            Assert.AreEqual(55, ConvUtils.ToInt32Ex("55"), "Result differs");
            Assert.AreEqual(55, ConvUtils.ToInt32Ex("+55"), "Result differs");
            Assert.AreEqual(-55, ConvUtils.ToInt32Ex("-55"), "Result differs");
            Assert.AreEqual(0x55, ConvUtils.ToInt32Ex("0x55"), "Result differs");
            Assert.AreEqual(0x55, ConvUtils.ToInt32Ex("0X55"), "Result differs");
            Assert.AreEqual(3, ConvUtils.ToInt32Ex("0b11"), "Result differs");
            Assert.AreEqual(5, ConvUtils.ToInt32Ex("0B101"), "Result differs");
            Assert.AreEqual(0xFEDC, ConvUtils.ToInt32Ex("0XFEDC"), "Result differs");
            Assert.AreEqual(-0xFEDC, ConvUtils.ToInt32Ex("-0xFEDC"), "Result differs");

            int? nres;

            nres = ConvUtils.ToNullableInt32Ex(null);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableInt32Ex(String.Empty);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableInt32Ex("");
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableInt32Ex("0xAA");
            Assert.AreEqual(0xAA, nres, "Result differs");

            Assert.Throws<OverflowException>(() => ConvUtils.ToUInt32Ex("0xFFFFFFFFF"));
        }

        [Test]
        public void ConvUtils_ToUShort()
        {
            Assert.AreEqual((ushort)55, ConvUtils.ToUInt16Ex("55"), "Result differs");
            Assert.AreEqual((ushort)0x55, ConvUtils.ToUInt16Ex("0x55"), "Result differs");
            Assert.AreEqual((ushort)0x55, ConvUtils.ToUInt16Ex("0X55"), "Result differs");
            Assert.AreEqual((ushort)3, ConvUtils.ToUInt16Ex("0b11"), "Result differs");
            Assert.AreEqual((ushort)5, ConvUtils.ToUInt16Ex("0B101"), "Result differs");
            Assert.AreEqual((ushort)0xFEDC, ConvUtils.ToUInt16Ex("0XFEDC"), "Result differs");

            ushort? nres;

            nres = ConvUtils.ToNullableUInt16Ex(null);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableUInt16Ex(String.Empty);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableUInt16Ex("");
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableUInt16Ex("0xAA");
            Assert.AreEqual((ushort)0xAAU, nres, "Result differs");

            Assert.Throws<OverflowException>(() => ConvUtils.ToUInt16Ex("0xFFFFF"));
            Assert.Throws<OverflowException>(() => ConvUtils.ToUInt16Ex("-1"));
        }

        [Test]
        public void ConvUtils_ToShort()
        {
            Assert.AreEqual((short)55, ConvUtils.ToInt16Ex("55"), "Result differs");
            Assert.AreEqual((short)-55, ConvUtils.ToInt16Ex("-55"), "Result differs");
            Assert.AreEqual((short)0x55, ConvUtils.ToInt16Ex("0x55"), "Result differs");
            Assert.AreEqual((short)0x55, ConvUtils.ToInt16Ex("0X55"), "Result differs");
            Assert.AreEqual((short)3, ConvUtils.ToInt16Ex("0b11"), "Result differs");
            Assert.AreEqual((short)5, ConvUtils.ToInt16Ex("0B101"), "Result differs");
            Assert.AreEqual((short)0x7EDC, ConvUtils.ToInt16Ex("0x7EDC"), "Result differs");
            Assert.AreEqual((short)-0x7EDC, ConvUtils.ToInt16Ex("-0x7EDC"), "Result differs");

            short? nres;

            nres = ConvUtils.ToNullableInt16Ex(null);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableInt16Ex(String.Empty);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableInt16Ex("");
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableInt16Ex("0xAA");
            Assert.AreEqual((short)0xAA, nres, "Result differs");

            Assert.Throws<OverflowException>(() => ConvUtils.ToInt16Ex("0xFFFF"));
            Assert.Throws<OverflowException>(() => ConvUtils.ToInt16Ex("-0xFFFF"));
        }

        [Test]
        public void ConvUtils_ToByte()
        {
            Assert.AreEqual((byte)55, ConvUtils.ToByteEx("55"), "Result differs");
            Assert.AreEqual((byte)0x55, ConvUtils.ToByteEx("0x55"), "Result differs");
            Assert.AreEqual((byte)0x55, ConvUtils.ToByteEx("0X55"), "Result differs");
            Assert.AreEqual((byte)3, ConvUtils.ToByteEx("0b11"), "Result differs");
            Assert.AreEqual((byte)5, ConvUtils.ToByteEx("0B101"), "Result differs");
            Assert.AreEqual((byte)0xFE, ConvUtils.ToByteEx("0XFE"), "Result differs");

            byte? nres;

            nres = ConvUtils.ToNullableByteEx(null);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableByteEx(String.Empty);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableByteEx("");
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.ToNullableByteEx("0xAA");
            Assert.AreEqual((byte)0xAAU, nres, "Result differs");

            Assert.Throws<OverflowException>(() => ConvUtils.ToByteEx("0xFFF"));
            Assert.Throws<OverflowException>(() => ConvUtils.ToByteEx("-1"));
        }

        [Test]
        public void ConvUtils_ToBoolean()
        {

            Assert.IsTrue(ConvUtils.ToBooleanEx("true"), "Result differs");
            Assert.IsTrue(ConvUtils.ToBooleanEx("TRUE"), "Result differs");
            Assert.IsTrue(ConvUtils.ToBooleanEx("1"), "Result differs");
            Assert.IsFalse(ConvUtils.ToBooleanEx("false"), "Result differs");
            Assert.IsFalse(ConvUtils.ToBooleanEx(""), "Result differs");

            bool? nres;

            nres = ConvUtils.NullableToBooleanEx(null);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.NullableToBooleanEx(String.Empty);
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.NullableToBooleanEx("");
            Assert.IsNull(nres, "Not null result");
            nres = ConvUtils.NullableToBooleanEx("true");
            Assert.AreEqual(true, nres, "Result differs");
            nres = ConvUtils.NullableToBooleanEx("0");
            Assert.AreEqual(false, nres, "Result differs");

        }

    }
}
