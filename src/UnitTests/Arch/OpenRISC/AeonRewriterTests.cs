using NUnit.Framework;
using Reko.Arch.OpenRISC;
using Reko.Core;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class AeonRewriterTests : RewriterTestBase
    {
        private readonly AeonArchitecture arch;
        private readonly Address addrLoad;

        public AeonRewriterTests()
        {
            // Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
            this.arch = new AeonArchitecture(CreateServiceContainer(), "aeon", new());
            this.addrLoad = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addrLoad;

        [Test]
        public void AeonRw_l_add__()
        {
            Given_HexString("8CE5");
            AssertCode(     // l.add?	r7,r5
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = r7 + r5");
        }

        [Test]
        public void AeonRw_l_add_two_operand()
        {
            Given_HexString("5CE704");
            AssertCode(     // l.add??	r7,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r7 + r7");
        }

        [Test]
        public void AeonRw_l_add_three_operand()
        {
            // confirmed with source
            Given_HexString("408324");
            AssertCode(     // l.add	r4,r3,r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = r3 + r4");
        }

        [Test]
        public void AeonRw_l_addi__()
        {
            Given_HexString("9C3C");
            AssertCode(     // l.addi?	r1,-0x4
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = r1 - 4<i32>");
        }

        [Test]
        public void AeonRw_l_addi()
        {
            Given_HexString("1C21EC");
            AssertCode(     // l.addi	r1,r1,0xEC
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r1 = r1 - 20<i32>");
        }

        [Test]
        public void AeonRw_l_addi_32bit()
        {
            Given_HexString("FC8A026C");
            AssertCode(     // l.addi	r4,r10,0x26C
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r10 + 620<i32>");
        }

        [Test]
        public void AeonRw_l_addi_32bit_0()
        {
            Given_HexString("FCC0829F");
            AssertCode(     // l.addi	r6,r0,-0x0x7D61
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = -32097<i32>");
        }

        [Test]
        public void AeonRw_l_and()
        {
            Given_HexString("44E734");
            AssertCode(     // l.and	r7,r7,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r7 & r6");
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
        public void AeonRw_l_bf()
        {
            Given_HexString("2003E9");
            AssertCode(     // l.bf	00333DEC
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (f) branch 001000FA");
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
        public void AeonRw_l_blti__()
        {
            Given_HexString("D0 88 FF 3E");
            AssertCode(     // l.blti? r4,0x8,002F4942
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r4 < 8<32>) branch 000FFFE7");
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
        public void AeonRw_bt_trap()
        {
            Given_HexString("8002");
            AssertCode(     // bt.trap
                "0|L--|00100000(2): 1 instructions",
                "1|L--|bt_trap()");
        }

        [Test]
        public void AeonDis_l_cmov()
        {
            Given_HexString("48 E7 00");
            AssertCode(     // l.cmov\tr3,r3,r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = f ? r7 : 0<32>");
        }

        [Test]
        public void AeonDis_l_cmovi()
        {
            Given_HexString("49 8C 0A");
            AssertCode(     // l.cmovi\tr12,r12,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r12 = f ? r12 : 1<32>");
        }

        [Test]
        public void AeonRw_l_divu()
        {
            Given_HexString("408339");
            AssertCode(     // l.divu	r4,r3,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = r3 /u r7");
        }

        [Test]
        public void AeonRw_entri__()
        {
            // XXX: no examples known
            Given_HexString("5E9338");
            AssertCode(     // entri?	0xA,0x99
                "0|L--|00100000(3): 1 instructions",
                "1|L--|entri__(0xA<32>, 0x99<32>)");
        }

        [Test]
        public void AeonRw_l_flush_line()
        {
            // confirmed with source
            Given_HexString("F4030016");
            AssertCode(     // l.flush.line	(r3),0x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__flush_line(&Mem0[r3:word32], 1<32>)");
        }

        [Test]
        public void AeonRw_l_invalidate_line()
        {
            // confirmed with source
            Given_HexString("F4030027");
            AssertCode(     // l.invalidate.line	(r3),0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__invalidate_line(&Mem0[r3:word32], 0<32>)");
        }

        [Test]
        public void AeonRw_l_j()
        {
            Given_HexString("93C5");
            AssertCode(     // l.j	0x3C5
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 000FFFC5");
        }

        [Test]
        public void AeonRw_l_jr()
        {
            Given_HexString("84E9");
            AssertCode(     // l.jr	r7
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto r7");
        }

        [Test]
        public void AeonRw_l_jr_ret()
        {
            Given_HexString("8529");
            AssertCode(     // l.jr	r9
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void AeonRw_l_lbz()
        {
            Given_HexString("F0 C7 1E EC");
            AssertCode(     // l.lbz?\tr6,0x1EEC(r7)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r7 + 7916<i32>:byte]",
                "2|L--|r6 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void AeonRw_l_lhz()
        {
            Given_HexString("08C301");
            AssertCode(     // l.lhz	r6,(r3)
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = Mem0[r3:uint16]",
                "2|L--|r6 = CONVERT(v4, uint16, word32)");
        }

        [Test]
        public void AeonRw_l_lhz___32bit()
        {
            Given_HexString("E8C75CA5");
            AssertCode(     // l.lhz?	r6,0x5CA4(r7)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r7 + 23716<i32>:word16]",
                "2|L--|r6 = CONVERT(v4, uint16, word32)");
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
        public void AeonRw_l_mfspr()
        {
            Given_HexString("C0A4000F");
            AssertCode(     // l.mfspr	r5,r4,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __move_from_spr(r4, 0<32>)");
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
        public void AeonRw_l_movhi__()
        {
            Given_HexString("046989");
            AssertCode(     // l.movhi?	r3,0x989
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = 0x9890000<32>");
        }

        [Test]
        public void AeonRw_l_movhi_fuse_with_load()
        {
            Given_HexString(
                "C0 C0 0A 41" +
                "EC E6 3A 3E");
            AssertCode(
                // l.movhi\tr6,0x523A3C@hi
                // l.lwz?\tr7,0x523A3C@lo(r6)
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r6 = 0x520000<32>",
                "2|L--|r7 = Mem0[0x00523A3C<p32>:word32]");
        }

        [Test]
        public void AeonRw_l_movhi_fuse_with_store()
        {
            Given_HexString(
                "C0 E0 0A 41" +
                "F8 C7 3A 05");
            AssertCode(
                // l.movhi\tr7,0x523A05@hi
                // l.sb?\t0x523A05@lo(r7),r6
                "0|L--|00100000(8): 3 instructions",
                "1|L--|r7 = 0x520000<32>",
                "2|L--|v4 = SLICE(r6, byte, 0)",
                "3|L--|Mem0[0x00523A05<p32>:byte] = v4");
        }

        [Test]
        public void AeonRw_l_movhi_fuse_with_addi()
        {
            Given_HexString(
                "C1 80 07 41" +
                "FD 4C 88 F0");
            AssertCode(
                // l.movhi\tr12,0x3988F0@hi
                // l.addi\tr10,r12,0x3988F0@lo
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r12 = 0x3A0000<32>",
                "2|L--|r10 = 0x3988F0<32>");
        }

        [Test]
        public void AeonRw_l_movhi_fuse_with_ori()
        {
            Given_HexString(
                "C0 CF FF E1" +
                "C8 C6 FF FF");
            AssertCode(
                // l.movhi\tr6,0x7FFFFFFF@hi
                // l.ori\tr6,r6,0x7FFFFFFF@lo
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r6 = 0x7FFF0000<32>",
                "2|L--|r6 = 0x7FFFFFFF<32>");
        }


        [Test]
        public void AeonRw_l_mtspr()
        {
            Given_HexString("C060011D");
            AssertCode(     // l.mtspr	r0,r3,0x11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__move_to_spr(0<32>, r3, 0x11<32>)");
        }

        [Test]
        public void AeonRw_mov__()
        {
            Given_HexString("8AE5");
            AssertCode(     // mov?	r23,r5
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r23 = r5");
        }

        [Test]
        public void AeonRw_movi__()
        {
            Given_HexString("98 DF");
            AssertCode(     // l.movi? r6,0x1F
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r6 = 0xFFFFFFFF<32>");
        }

        [Test]
        public void AeonRw_l_or()
        {
            Given_HexString("44E325");
            AssertCode(     // l.or	r7,r3,r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r3 | r4");
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
        public void AeonRw_l_sll__()
        {
            Given_HexString("4CAC24");
            AssertCode(     // l.sll?	r5,r12,r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r5 = r12 << r4");
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
        public void AeonRw_l_sfgtui()
        {
            Given_HexString("5C641B");
            AssertCode(     // l.sfgtui	r3,0x20
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r3 >u 0x20<32>");
        }

        [Test]
        public void AeonDis_l_sfnei__()
        {
            Given_HexString("C0 C0 7D 04");
            AssertCode(     // l.sfnei? r6,0x3E8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f = r6 != 0x3E8<32>");
        }

        [Test]
        public void AeonRw_l_jal()
        {
            Given_HexString("E4000A70");
            AssertCode(     // l.jal?	00100A70
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100538 (0)");
        }

        [Test]
        public void AeonRw_l_sb()
        {
            Given_HexString("F8 EA 36 D8");
            AssertCode(     // l.sb 0x36D8(r10),r7
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r7, byte, 0)",
                "2|L--|Mem0[r10 + 14040<i32>:byte] = v4");
        }

        [Test]
        public void AeonRw_l_sb___24bit()
        {
            Given_HexString("18 EB 05"); 
            AssertCode(     // l.sb?    0x5(r11),r7
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = SLICE(r7, byte, 0)",
                "2|L--|Mem0[r11 + 5<i32>:byte] = v4");
        }

        [Test]
        public void AeonRw_l_sh()
        {
            Given_HexString("EC 67 34 5B");
            AssertCode(     // l.sh? 0x345A(r7),r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r3, word16, 0)",
                "2|L--|Mem0[r7 + 13402<i32>:word16] = v4");
        }

        [Test]
        public void AeonRw_l_srai()
        {
            Given_HexString("4C A7 0A");
            AssertCode(     // l.srai?\tr5,r7,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r5 = r7 >> 1<i32>");
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
        public void AeonRw_l_sw__0()
        {
            Given_HexString("0C0108");
            AssertCode(     // l.sw?	0x8(r1),r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Mem0[r1 + 8<i32>:word32] = 0<32>");
        }

        [Test]
        public void AeonRw_l_sw_32bit()
        {
            // confirmed with source
            Given_HexString("ECA10084");
            AssertCode(     // l.sw	0x84(r1),r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 132<i32>:word32] = r5");
        }

        [Test]
        public void AeonRw_l_srl__()
        {
            Given_HexString("4D6B3D");
            AssertCode(     // l.srl?	r11,r11,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r11 = r11 >>u r7");
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
        public void AeonRw_l_mul()
        {
            Given_HexString("408323");
            AssertCode(     // l.mul	r4,r3,r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = r3 * r4");
        }

        [Test]
        public void AeonRw_l_nand__()
        {
            Given_HexString("44EA57");
            AssertCode(     // l.nand?	r7,r10,r10
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = ~r10");
        }

        [Test]
        public void AeonRw_l_nand__different()
        {
            // not from actual binary
            Given_HexString("44221F");
            AssertCode(     // l.nand?	r1,r2,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r1 = ~(r2 & r3)");
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
        public void AeonRw_l_nop_16bit()
        {
            // confirmed with source
            Given_HexString("8001");
            AssertCode(     // l.nop
                "0|L--|00100000(2): 1 instructions",
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
        public void AeonRw_l_sfgeu()
        {
            Given_HexString("5E2717");
            AssertCode(     // l.sfgeu	r17,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r7 >=u r17");
        }

        [Test]
        public void AeonDis_l_sfleui__()
        {
            Given_HexString("5C 6E F3");
            AssertCode(     // l.sfleui?\tr3,0x77
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r3 <=u 0x77<32>");
        }

        [Test]
        public void AeonDis_l_sfltu()
        {
            Given_HexString("5F 47 1F");
            AssertCode(     // l.sfltu	r7,r26
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r7 <u r26");
        }

        [Test]
        public void AeonRw_l_sfne()
        {
            Given_HexString("5D8E0D");
            AssertCode(     // l.sfne	r12,r14
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r12 != r14");
        }

        [Test]
        public void AeonRw_l_sub()
        {
            Given_HexString("40EA1D");
            AssertCode(     // l.sub	r7,r10,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r10 - r3");
        }

        [Test]
        public void AeonRw_l_syncwritebuffer()
        {
            Given_HexString("F4000005");
            AssertCode(     // l.syncwritebuffer
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__syncwritebuffer()");
        }

        [Test]
        public void AeonRw_l_xor__()
        {
            Given_HexString("44E736");
            AssertCode(     // l.xor? r7,r7,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r7 ^ r6");
        } 
    }
}
