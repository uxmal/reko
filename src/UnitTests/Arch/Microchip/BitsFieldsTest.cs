using NUnit.Framework;
using Reko.Libraries.Microchip;

namespace Reko.UnitTests.Arch.Microchip
{
    [TestFixture]
    public class BitsFieldsTests
    {
        [Test]
        public void Bits_SignExtendInt()
        {
            int res;

            res = ((byte)1).SignExtend(2);
            Assert.AreEqual(1, res, "Result differs ' 1 / 2-bit");
            res = ((byte)3).SignExtend(2);
            Assert.AreEqual(-1, res, "Result differs ' 3 / 2-bit");
            res = ((byte)1).SignExtend(8);
            Assert.AreEqual(1, res, "Result differs ' 1 / 8-bit");
            res = ((byte)255).SignExtend(8);
            Assert.AreEqual(-1, res, "Result differs ' 255 / 8-bit");

            res = ((ushort)1).SignExtend(16);
            Assert.AreEqual(1, res, "Result differs ' 1 / 16-bit");
            res = ((ushort)0x7FFF).SignExtend(16);
            Assert.AreEqual(0x7FFF, res, "Result differs ' 32767 / 16-bit");
            res = ((ushort)0xFFFF).SignExtend(16);
            Assert.AreEqual(-1, res, "Result differs ' 65535 / 16-bit");

            res = ((uint)1).SignExtend(17);
            Assert.AreEqual(1, res, "Result differs ' 1 / 17-bit");
            res = ((uint)0x7FFF).SignExtend(18);
            Assert.AreEqual(0x7FFF, res, "Result differs ' 32767 / 17-bit");
            res = ((uint)0xFFFFFFFF).SignExtend(16);
            Assert.AreEqual(-65537, res, "Result differs ' 65535 / 16-bit");
        }

        [Test]
        public void Bits_ExtractByte()
        {
            byte val;

            val = 0;
            Assert.AreEqual((byte)0, val.Extract(0, 1), "Result differs : val=0[0:1]");
            val = 1;
            Assert.AreEqual((byte)1, val.Extract(0, 1), "Result differs : val=1[0:1]");
            val = 255;
            Assert.AreEqual((byte)7, val.Extract(2, 3), "Result differs : val=255[2:3]");
            val = 0xF7;
            Assert.AreEqual((byte)5, val.Extract(2, 3), "Result differs : val=0xF7[2:3]");
            val = 0xAA;
            Assert.AreEqual((byte)2, val.Extract(2, 3), "Result differs : val=0xAA[2:3]");
            Assert.AreEqual((byte)1, val.Extract(3, 2), "Result differs : val=0xAA[3:2]");
        }

        [Test]
        public void Bits_ExtractSByte()
        {
            sbyte val;

            val = 0;
            Assert.AreEqual((sbyte)0, val.Extract(0, 1), "Result differs : val=0[0:1]");
            val = 1;
            Assert.AreEqual((sbyte)1, val.Extract(0, 1), "Result differs : val=1[0:1]");
            val = -1;
            Assert.AreEqual((sbyte)7, val.Extract(2, 3), "Result differs : val=255[2:3]");
            val = -9;
            Assert.AreEqual((sbyte)5, val.Extract(2, 3), "Result differs : val=0xF7[2:3]");
            val = -86;
            Assert.AreEqual((sbyte)2, val.Extract(2, 3), "Result differs : val=0xAA[2:3]");
            Assert.AreEqual((sbyte)1, val.Extract(3, 2), "Result differs : val=0xAA[3:2]");
        }

        [Test]
        public void Bits_ExtractUShort()
        {
            ushort val;

            val = 0;
            Assert.AreEqual((ushort)0, val.Extract(0, 1), "Result differs : val=0[0:1]");
            val = 1;
            Assert.AreEqual((ushort)1, val.Extract(0, 1), "Result differs : val=1[0:1]");
            val = 0xFFFF;
            Assert.AreEqual((ushort)7, val.Extract(2, 3), "Result differs : val=0xFFFF<16>[2:3]");
            Assert.AreEqual((ushort)15, val.Extract(5, 4), "Result differs : val=0xFFFF<16>[5:4]");
            val = 0xF7F7;
            Assert.AreEqual((ushort)5, val.Extract(2, 3), "Result differs : val=0xF7F7<16>[2:3]");
            Assert.AreEqual((ushort)5, val.Extract(10, 3), "Result differs : val=0xF7F7<16>[10:3]");
            val = 0xAAAA;
            Assert.AreEqual((ushort)2, val.Extract(2, 3), "Result differs : val=0xAAAA<16>[2:3]");
            Assert.AreEqual((ushort)1, val.Extract(3, 2), "Result differs : val=0xAAAA<16>[3:2]");
            Assert.AreEqual((ushort)2, val.Extract(10, 3), "Result differs : val=0xAAAA<16>[10:3]");
            Assert.AreEqual((ushort)1, val.Extract(11, 2), "Result differs : val=0xAAAA<16>[11:2]");
        }

        [Test]
        public void Bits_ExtractShort()
        {
            short val;

            val = 0;
            Assert.AreEqual((ushort)0, val.Extract(0, 1), "Result differs : val=0[0:1]");
            val = 1;
            Assert.AreEqual((ushort)1, val.Extract(0, 1), "Result differs : val=1[0:1]");
            val = -1;
            Assert.AreEqual((ushort)7, val.Extract(2, 3), "Result differs : val=0xFFFF<16>[2:3]");
            Assert.AreEqual((ushort)15, val.Extract(5, 4), "Result differs : val=0xFFFF<16>[5:4]");
            val = -2057;
            Assert.AreEqual((ushort)5, val.Extract(2, 3), "Result differs : val=0xF7F7<16>[2:3]");
            Assert.AreEqual((ushort)5, val.Extract(10, 3), "Result differs : val=0xF7F7<16>[10:3]");
            val = -21846;
            Assert.AreEqual((ushort)2, val.Extract(2, 3), "Result differs : val=0xAAAA<16>[2:3]");
            Assert.AreEqual((ushort)1, val.Extract(3, 2), "Result differs : val=0xAAAA<16>[3:2]");
            Assert.AreEqual((ushort)2, val.Extract(10, 3), "Result differs : val=0xAAAA<16>[10:3]");
            Assert.AreEqual((ushort)1, val.Extract(11, 2), "Result differs : val=0xAAAA<16>[11:2]");
        }

        [Test]
        public void Bits_ExtractUInt()
        {
            uint val;

            val = 0;
            Assert.AreEqual(0U, val.Extract(0, 1), "Result differs : val=0[0:1]");
            val = 1;
            Assert.AreEqual(1U, val.Extract(0, 1), "Result differs : val=1[0:1]");
            val = 0xFFFF;
            Assert.AreEqual(7U, val.Extract(2, 3), "Result differs : val=0xFFFF<16>[2:3]");
            Assert.AreEqual(15U, val.Extract(5, 4), "Result differs : val=0xFFFF<16>[5:4]");
            val = 0xF7F7F7F7;
            Assert.AreEqual(5U, val.Extract(2, 3), "Result differs : val=0xF7F7F7F7<32>[2:3]");
            Assert.AreEqual(5U, val.Extract(10, 3), "Result differs : val=0xF7F7F7F7<32>[10:3]");
            Assert.AreEqual(5U, val.Extract(18, 3), "Result differs : val=0xF7F7F7F7<32>[18:3]");
            val = 0xAAAAAAAA;
            Assert.AreEqual(2U, val.Extract(2, 3), "Result differs : val=0xAAAAAAAA<32>[2:3]");
            Assert.AreEqual(1U, val.Extract(3, 2), "Result differs : val=0xAAAAAAAA<32>[3:2]");
            Assert.AreEqual(2U, val.Extract(18, 3), "Result differs : val=0xAAAAAAAA<32>[18:3]");
            Assert.AreEqual(1U, val.Extract(19, 2), "Result differs : val=0xAAAAAAAA<32>[19:2]");
        }

        [Test]
        public void Bits_ExtractInt()
        {
            int val;

            val = 0;
            Assert.AreEqual(0U, val.Extract(0, 1), "Result differs : val=0[0:1]");
            val = 1;
            Assert.AreEqual(1U, val.Extract(0, 1), "Result differs : val=1[0:1]");
            val = 0xFFFF;
            Assert.AreEqual(7U, val.Extract(2, 3), "Result differs : val=0xFFFF<16>[2:3]");
            Assert.AreEqual(15U, val.Extract(5, 4), "Result differs : val=0xFFFF<16>[5:4]");
            val = -134744073;
            Assert.AreEqual(5U, val.Extract(2, 3), "Result differs : val=0xF7F7F7F7<32>[2:3]");
            Assert.AreEqual(5U, val.Extract(10, 3), "Result differs : val=0xF7F7F7F7<32>[10:3]");
            Assert.AreEqual(5U, val.Extract(18, 3), "Result differs : val=0xF7F7F7F7<32>[18:3]");
            val = -1431655766;
            Assert.AreEqual(2U, val.Extract(2, 3), "Result differs : val=0xAAAAAAAA<32>[2:3]");
            Assert.AreEqual(1U, val.Extract(3, 2), "Result differs : val=0xAAAAAAAA<32>[3:2]");
            Assert.AreEqual(2U, val.Extract(18, 3), "Result differs : val=0xAAAAAAAA<32>[18:3]");
            Assert.AreEqual(1U, val.Extract(19, 2), "Result differs : val=0xAAAAAAAA<32>[19:2]");
        }

        [Test]
        public void Bits_BitCount()
        {
            Assert.AreEqual(1, (uint)0x010.CountBits());
            Assert.AreEqual(2, (uint)0x011.CountBits());
            Assert.AreEqual(31, (uint)0xFFFFFFFE.CountBits());
            Assert.AreEqual(30, (uint)0x7FFFFFFE.CountBits());
            Assert.AreEqual(32, (uint)0xFFFFFFFF.CountBits());
        }

        [Test]
        public void Bits_Align()
        {
            Assert.AreEqual(0x11, (uint)0x011.Align(1));
            Assert.AreEqual(0x12, (uint)0x011.Align(2));
            Assert.AreEqual(0x14, (uint)0x011.Align(4));
            Assert.AreEqual(0x18, (uint)0x011.Align(8));

            Assert.AreEqual(0x11, (int)0x011.Align(1));
            Assert.AreEqual(0x12, (int)0x011.Align(2));
            Assert.AreEqual(0x14, (int)0x011.Align(4));
            Assert.AreEqual(0x18, (int)0x011.Align(8));
        }

        [Test]
        public void Bits_Rotate()
        {
            Assert.AreEqual(0, 0.Signum());
            Assert.AreEqual(1, 236570.Signum());
            Assert.AreEqual(-1, -75632.Signum());
            Assert.AreEqual(0, 0L.Signum());
            Assert.AreEqual(1, 2365708835L.Signum());
            Assert.AreEqual(-1, -7563200452L.Signum());

            Assert.AreEqual(0x22, (uint)0x011.RotateLeft(1));
            Assert.AreEqual(0x11, (uint)0x022.RotateRight(1));
            Assert.AreEqual(0x22, (int)0x011.RotateLeft(1));
            Assert.AreEqual(0x11, (int)0x022.RotateRight(1));

            Assert.AreEqual(0x2244, (ulong)0x01122.RotateLeft(1));
            Assert.AreEqual(0x1122, (ulong)0x02244.RotateRight(1));
            Assert.AreEqual(0x2244, (long)0x01122.RotateLeft(1));
            Assert.AreEqual(0x1133, (long)0x02266.RotateRight(1));

            Assert.AreEqual(0x88000000, (uint)0x011.Reverse());
            Assert.AreEqual(0xE6A2C480, (uint)0x01234567.Reverse());
            Assert.AreEqual(-69604664, 0x13579BDF.Reverse());
            Assert.AreEqual(-131721928, -324508640.Reverse());

            Assert.AreEqual(0x8800000000000000UL, 0x011UL.Reverse());
            Assert.AreEqual(0xE6A2C48000000000UL, 0x01234567UL.Reverse());
            Assert.AreEqual(0x0F7B3D591E6A2C48L, 0x123456789ABCDEF0L.Reverse());
            Assert.AreEqual(-1131482754016527488, -81985529216486896L.Reverse());

        }

        [Test]
        public void Bits_Strings()
        {
            Assert.AreEqual("-1-1-11-1-1-1-", 0x55AA.ToStringField(14, '1'));
            Assert.AreEqual("-r-r-rr-r-r-r-", 0x55AA.ToStringField(14, 'r'));
            Assert.AreEqual("oioiioioioio", 0x55AA.ToStringField(12, 'i', 'o'));

            Assert.AreEqual(0, "".ToBitField(14, '1'));
            Assert.AreEqual(0x15AA, "-1-1-11-1-1-1-".ToBitField(14, '1'));
            Assert.AreEqual(0, "-1-1-11-1-1-1-".ToBitField(14, 'r'));
            Assert.AreEqual(0x2A55, "-1-1-11-1-1-1-".ToBitField(14, '-'));
        }

    }
}
