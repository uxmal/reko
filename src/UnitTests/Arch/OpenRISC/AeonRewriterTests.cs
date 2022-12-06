using NUnit.Framework;
using Reko.Arch.OpenRISC;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class AeonRewriterTests : RewriterTestBase
    {
        private readonly AeonArchitecture arch;
        private readonly Address addrLoad;

        public AeonRewriterTests()
        {
            this.arch = new AeonArchitecture(CreateServiceContainer(), "aeon", new());
            this.addrLoad = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addrLoad;

        [Test]
        public void AeonRw_l_addi__()
        {
            Given_HexString("9C3C");
            AssertCode(     // l.addi?	r1,0x1C
                "0|L--|00100000(2): 1 instructions",
                "1|L--|l_addi__(r1, 0x1C<32>)");
        }

        [Test]
        public void AeonRw_l_addi()
        {
            Given_HexString("1C21EC");
            AssertCode(     // l.addi	r1,r1,0xEC
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r1 = r1 + 0xEC<32>");
        }

        [Test]
        public void AeonRw_l_andi__()
        {
            Given_HexString("9869");
            AssertCode(     // l.andi?	r3,0x9
                "0|L--|00100000(2): 1 instructions",
                "1|L--|l_andi__(r3, 9<32>)");
        }

        [Test]
        public void AeonRw_ble__i__()
        {
            Given_HexString("248415");
            AssertCode(     // ble?i?	r4,0x1,00354F36
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r4 <= 1<32>) branch 00100005");
        }


        [Test]
        public void AeonRw_l_jr()
        {
            Given_HexString("8529");
            AssertCode(     // l.jr	r9,r9
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void AeonRw_l_ori()
        {
            Given_HexString("506401");
            AssertCode(     // l.ori	r3,r4,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = r4 | 1<32>");
        }

        [Test]
        public void AeonRw_l_ori_zero()
        {
            Given_HexString("506001");
            AssertCode(     // l.ori	r3,r0,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = 1<32>");
        }

        [Test]
        public void AeonRw_l_slli__()
        {
            Given_HexString("4C6320");
            AssertCode(     // l.slli?	r3,r3,0x4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = r3 << 4<i32>");
        }

        [Test]
        public void AeonRw_l_lwz__()
        {
            Given_HexString("0CE302");
            AssertCode(     // l.lwz?	r7,(r3)
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = Mem0[r3:word32]");
        }

        [Test]
        public void AeonRw_l_movhi()
        {
            Given_HexString("C0EFFFE1");
            AssertCode(     // l.movhi	r7,0x7FFF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = 0x7FFF0000<32>");
        }

        [Test]
        public void AeonRw_Nyi()
        {
            Given_HexString("40E01D");
            AssertCode(     // Nyi
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Nyi()");
        }


        [Test]
        public void AeonRw_l_sw()
        {
            Given_HexString("0D41FC");
            AssertCode(     // l.sw	-0x4(r1),r10
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Mem0[r1 - 4<i32>:word32] = r10");
        }

        [Test]
        public void AeonRw_mov__()
        {
            Given_HexString("8AE5");
            AssertCode(     // mov?	r23,r5
                "0|L--|00100000(2): 1 instructions",
                "1|L--|mov__(r23, r5)");
        }

        [Test]
        public void AeonRw_l_or__()
        {
            Given_HexString("44E325");
            AssertCode(     // l.or?	r7,r3,r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r3 | r4");
        }

        [Test]
        public void AeonRw_l_andi()
        {
            Given_HexString("54E301");
            AssertCode(     // l.andi	r7,r3,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r3 & 1<32>");
        }

        [Test]
        public void AeonRw_beqi__()
        {
            Given_HexString("2061E0");
            AssertCode(     // beqi?	r3,0x0,00100078
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100078");
        }

        [Test]
        public void AeonRw_l_sfgtui()
        {
            Given_HexString("5C641B");
            AssertCode(     // l.sfgtui	r3,0x20
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r3 >u 0x20<32>");
        }

        [Test]
        public void AeonRw_l_jal__()
        {
            Given_HexString("E4000A71");
            AssertCode(     // l.jal?	0037E855
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 0010029C (0)");
        }

        [Test]
        public void AeonRw_l_sw__0()
        {
            Given_HexString("180308");
            AssertCode(     // l.sw?	0x8(r3),r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Mem0[r3 + 8<i32>:word32] = 0<32>");
        }

        [Test]
        public void AeonRw_l_srli__()
        {
            Given_HexString("4C6319");
            AssertCode(     // l.srli?	r3,r3,0x3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = r3 >>u 3<i32>");
        }

        [Test]
        public void AeonRw_l_lhz()
        {
            Given_HexString("08C301");
            AssertCode(     // l.lhz	r6,(r3)
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v3 = Mem0[r3:uint16]",
                "2|L--|r6 = CONVERT(v3, uint16, word32)");
        }

        [Test]
        public void AeonRw_l_movhi__()
        {
            Given_HexString("046989");
            AssertCode(     // l.movhi?	r3,0x989
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = 0x9890000<32>");
        }

        [Test]
        public void AeonRw_l_mul()
        {
            Given_HexString("408323");
            AssertCode(     // l.mul	r4,r3,r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = r3 * r4");
        }

        [Test]
        public void AeonRw_l_nop()
        {
            Given_HexString("000000");
            AssertCode(     // l.nop
                "0|L--|00100000(3): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void AeonRw_l_sfeqi()
        {
            Given_HexString("5C8A21");
            AssertCode(     // l.sfeqi	r4,0xA
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r4 == 0xA<32>");
        }

        [Test]
        public void AeonRw_l_syncwritebuffer()
        {
            Given_HexString("F4000005");
            AssertCode(     // l.syncwritebuffer
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__syncwritebuffer()");
        }

    }
}
