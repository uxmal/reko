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
        public void AeonRw_bt_add__()
        {
            Given_HexString("8CE5");
            AssertCode(     // bt.add?	r7,r5
                "0|L--|00100000(2): 3 instructions",
                "1|L--|r7 = r7 + r5",
                "2|L--|cy = cond(r7)",
                "3|L--|ov = cond(r7)");
        }

        [Test]
        public void AeonRw_bn_add()
        {
            // confirmed with source
            Given_HexString("408324");
            AssertCode(     // bn.add	r4,r3,r4
                "0|L--|00100000(3): 3 instructions",
                "1|L--|r4 = r3 + r4",
                "2|L--|cy = cond(r4)",
                "3|L--|ov = cond(r4)");
        }

        [Test]
        public void AeonRw_bn_addc__()
        {
            Given_HexString("40 84 47");
            AssertCode(     // bn.addc?\tr4,r4,r8
                "0|L--|00100000(3): 3 instructions",
                "1|L--|r4 = r4 + r8 + cy",
                "2|L--|cy = cond(r4)",
                "3|L--|ov = cond(r4)");
        }

        [Test]
        public void AeonRw_bg_addci__()
        {
            Given_HexString("DD 08 FF FF");
            AssertCode(     // bg.addci?\tr8,r8,-0x1
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r8 = r8 - 1<i32> + cy",
                "2|L--|cy = cond(r8)",
                "3|L--|ov = cond(r8)");
        }

        [Test]
        public void AeonRw_bt_addi__()
        {
            Given_HexString("9C3C");
            AssertCode(     // bt.addi?	r1,-0x4
                "0|L--|00100000(2): 3 instructions",
                "1|L--|r1 = r1 - 4<i32>",
                "2|L--|cy = cond(r1)",
                "3|L--|ov = cond(r1)");
        }

        [Test]
        public void AeonRw_bn_addi_negative_immediate()
        {
            Given_HexString("1C21EC");
            AssertCode(     // bn.addi	r1,r1,-0x14
                "0|L--|00100000(3): 3 instructions",
                "1|L--|r1 = r1 - 20<i32>",
                "2|L--|cy = cond(r1)",
                "3|L--|ov = cond(r1)");
        }

        [Test]
        public void AeonRw_bg_addi()
        {
            Given_HexString("FC8A026C");
            AssertCode(     // bg.addi	r4,r10,0x26C
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r4 = r10 + 620<i32>",
                "2|L--|cy = cond(r4)",
                "3|L--|ov = cond(r4)");
        }

        [Test]
        public void AeonRw_bg_addi_0()
        {
            Given_HexString("FCC0829F");
            AssertCode(     // bg.addi	r6,r0,-0x0x7D61
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r6 = -32097<i32>",
                "2|L--|cy = cond(r6)",
                "3|L--|ov = cond(r6)");
        }

        [Test]
        public void AeonRw_bn_and()
        {
            Given_HexString("44E734");
            AssertCode(     // bn.and	r7,r7,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r7 & r6");
        }

        [Test]
        public void AeonRw_bn_andi()
        {
            Given_HexString("54E301");
            AssertCode(     // bn.andi	r7,r3,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r3 & 1<32>");
        }

        [Test]
        public void AeonRw_bg_andi()
        {
            Given_HexString("C54D8000");
            AssertCode(     // bg.andi	r10,r13,0x8000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = r13 & 0x8000<32>");
        }

        [Test]
        public void AeonRw_bg_b__bitset__()
        {
            Given_HexString("D0 7F FB 7C");
            AssertCode(     // bg.b?bitset?\tr3,0x1F,000FFF6F
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (__bit<word32,word32>(r3, 0x1F<32>)) branch 000FFF6F");
        }

        [Test]
        public void AeonRw_bg_beq()
        {
            Given_HexString("D4E3FF8A");
            AssertCode(     // b.beq?	r7,r3,00384D75
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r7 == r3) branch 000FFFF1");
        }

        [Test]
        public void AeonRw_bn_beqi__()
        {
            Given_HexString("2061E0");
            AssertCode(     // bn.beqi?	r3,0x0,00100078
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r3 == 0<32>) branch 00100078");
        }

        [Test]
        public void AeonRw_bg_beqi__()
        {
            Given_HexString("D0B700CA");
            AssertCode(     // bg.beqi?	r3,0x17,00100019
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r5 == 0x17<32>) branch 00100019");
        }

        [Test]
        public void AeonRw_bn_bf()
        {
            Given_HexString("2003E9");
            AssertCode(     // bn.bf	001000FA
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (f) branch 001000FA");
        }

        [Test]
        public void AeonRw_bg_bf()
        {
            Given_HexString("D4 00 02 1B");
            AssertCode(     // bg.bf	00100043
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (f) branch 00100043");
        }

        [Test]
        public void AeonRw_bg_bges()
        {
            Given_HexString("D4 6B 04 B9");
            AssertCode(     // bg.bgeu?\r3,r11,00100097
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r3 >= r11) branch 00100097");
        }

        [Test]
        public void AeonRw_bg_bgeu()
        {
            Given_HexString("D4 E5 FF 1D");
            AssertCode(     // bg.bgeu?\tr7,r5,000FFFE3
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r7 >=u r5) branch 000FFFE3");
        }

        [Test]
        public void AeonRw_bn_bleui__()
        {
            Given_HexString("248415");
            AssertCode(     // bn.bleui?	r4,0x1,00354F36
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r4 <=u 1<32>) branch 00100005");
        }

        [Test]
        public void AeonRw_bg_bleu__()
        {
            Given_HexString("D4 E5 FF 30");
            AssertCode(     // bg.bleu?\tr7,r5,xxx
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r7 <=u r5) branch 000FFFE6");
        }

        [Test]
        public void AeonRw_bg_bltui__()
        {
            Given_HexString("D0 88 FF 3E");
            AssertCode(     // bg.bltui? r4,0x8,002F4942
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r4 <u 8<32>) branch 000FFFE7");
        }

        [Test]
        public void AeonRw_bn_bgtui__()
        {
            Given_HexString("24AFC3");
            AssertCode(     // bn.bgtui?	r5,0x3,000FFFF0
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r5 >u 3<32>) branch 000FFFF0");
        }

        [Test]
        public void AeonRw_bg_bgts()
        {
            Given_HexString("D6 E5 FF 94");
            AssertCode(     // bg.bgts?\tr23,r5,000FFFF2
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 > r5) branch 000FFFF2");
        }

        [Test]
        public void AeonRw_bg_bgtui__()
        {
            // This is being used in unsigned comparisons
            // swtich (...) statements.
            Given_HexString("D0 6D 00 B5");
            AssertCode(     // bg.bgtui?\tr3,0x0D,00100016
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r3 >u 0xD<32>) branch 00100016");
        }

        [Test]
        public void AeonRw_bg_blesi__()
        {
            Given_HexString("D1 5F 05 88");
            AssertCode(     // bg.blesi? r10,-0x1,001000B1
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r10 <= -1<i32>) branch 001000B1");
        }

        [Test]
        public void AeonRw_bn_blesi__()
        {
            Given_HexString("26 E0 5C");
            AssertCode(     // bn.beqi??\tr23,0x0,00100011
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r23 <= 0<32>) branch 00100017");
        }

        [Test]
        public void AeonRw_bn_blesi__2()
        {
            Given_HexString("24 63 36");
            AssertCode(     // bn.blesi? r3,0x0,000FFFCD
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r3 <= 0<i32>) branch 000FFFCD");
        }

        [Test]
        public void AeonRw_bg_bne__()
        {
            Given_HexString("D4 C7 00 6E");
            AssertCode(     // bg.bne?	r6,r7,42
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r6 != r7) branch 0010000D");
        }

        [Test]
        public void AeonRw_bn_bnei__()
        {
            Given_HexString("21 4D B6");
            AssertCode(     // bn.bnei?	r10,0x3,0010006D
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r10 != 3<32>) branch 0010006D");
        }

        [Test]
        public void AeonRw_bn_bnf__()
        {
            Given_HexString("222F6B");
            AssertCode(     // bn.bnf?	000F8BDA
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (!f) branch 000F8BDA");
        }

        [Test]
        public void AeonRw_bt_trap()
        {
            Given_HexString("8002");
            AssertCode(     // bt.trap
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__trap(1<32>)");
        }

        [Test]
        public void AeonRw_bn_cmov____()
        {
            Given_HexString("48 E7 00");
            AssertCode(     // bn.cmov??	r3,r3,r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = f ? r7 : 0<32>");
        }

        [Test]
        public void AeonRw_bn_cmovii__()
        {
            Given_HexString("48 E1 FB");
            AssertCode(     // bn.cmovi??\tr7,0x1,-0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = f ? 1<i32> : -1<i32>");
        }

        [Test]
        public void AeonRw_bn_cmovis__positive()
        {
            Given_HexString("49 8C 0A");
            AssertCode(     // bn.cmovis?	r12,r12,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r12 = f ? r12 : 1<i32>");
        }

        [Test]
        public void AeonRw_bn_cmovis__negative()
        {
            Given_HexString("48 C0 FA");
            // speculative guess.
            AssertCode(    // bn.cmovis?\tr6,r0,-0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r6 = f ? 0<32> : -1<i32>");
        }

        [Test]
        public void AeonRw_bn_divs()
        {
            //$REVIEW: this might be bn.divu 
            Given_HexString("40 E7 30");
            AssertCode(     // bn.divs\tr7,r7,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r7 / r6");
        }

        [Test]
        public void AeonRw_bn_divu()
        {
            Given_HexString("408339");
            AssertCode(     // bn.divu	r4,r3,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = r3 /u r7");
        }

        [Test]
        public void AeonRw_bn_entri__()
        {
            Given_HexString("5C8058");
            AssertCode(     // bn.entri?	0x2,0x2
                "0|L--|00100000(3): 3 instructions",
                "1|L--|Mem0[r1:word32] = r9",
	            "2|L--|Mem0[r1 - 4<i32>:word32] = r10",
	            "3|L--|r1 = r1 - 0x10<32>");
        }

        [Test]
        public void AeonRw_bn_extbs__()
        {
            Given_HexString("5C A5 02");
            AssertCode(     // bn.extbs?\tr5,r5
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v2 = SLICE(r5, int8, 0)",
                "2|L--|r5 = CONVERT(v2, int8, int32)");
        }

        [Test]
        public void AeonRw_bn_extbz__()
        {
            Given_HexString("5C 85 00");
            AssertCode(     // bn.extbz  r4,r5
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v2 = SLICE(r5, byte, 0)",
                "2|L--|r4 = CONVERT(v2, byte, uint32)");
        }

        [Test]
        public void AeonRw_bn_exths__()
        {
            Given_HexString("5F 67 06");
            AssertCode(     // bn.exths?\r27,r7
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v2 = SLICE(r7, int16, 0)",
                "2|L--|r27 = CONVERT(v2, int16, int32)");
        }

        [Test]
        public void AeonRw_bn_exthz__()
        {
            Given_HexString("5D 47 04");
            AssertCode(     // bn.exthz  r10,r7
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v2 = SLICE(r7, uint16, 0)",
                "2|L--|r10 = CONVERT(v2, uint16, uint32)");
        }

        [Test]
        public void AeonRw_bn_ff1__()
        {
            Given_HexString("5C6B08");
            AssertCode(     // bn.ff1?	r3,r11
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = __find_first_one<word32>(r11)");
        }

        [Test]
        public void AeonRw_bg_flush_invalidate()
        {
            Given_HexString("F4033C04");
            AssertCode(     // bg.flush.invalidate	0xF0(r3)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__flush_invalidate(&Mem0[r3 + 240<i32>:word32])");
        }

        [Test]
        public void AeonRw_bg_flush_line()
        {
            // confirmed with source
            Given_HexString("F4030016");
            AssertCode(     // bg.flush.line	(r3),0x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__flush_line(&Mem0[r3:word32], 1<32>)");
        }

        [Test]
        public void AeonRw_bg_invalidate_line()
        {
            // confirmed with source
            Given_HexString("F4030007");
            AssertCode(     // bg.invalidate.line	(r3),0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__invalidate_line(&Mem0[r3:word32], 0<32>)");
        }

        [Test]
        public void AeonRw_bt_j()
        {
            Given_HexString("93C5");
            AssertCode(     // bt.j	0x3C5
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 000FFFC5");
        }

        [Test]
        public void AeonRw_bn_j____()
        {
            // Found in switch statements
            Given_HexString("2F F1 7E");
            AssertCode(     // bn.j 
                "0|T--|00100000(3): 1 instructions",
                "1|T--|goto 000FF17E");
        }

        [Test]
        public void AeonRw_bg_j()
        {
            Given_HexString("E5555555");
            AssertCode(     // bg.j	0x00BAAAAA
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 00BAAAAA");
        }

        [Test]
        public void AeonRw_bg_jal()
        {
            Given_HexString("E4000A70");
            AssertCode(     // bg.jal	00100A70
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100538 (0)");
        }

        [Test]
        public void AeonRw_bn_jal__()
        {
            Given_HexString("2B FF 2A");
            AssertCode(     // bn.jal?\t000FFF2A
                "0|T--|00100000(3): 1 instructions",
                "1|T--|call 000FFF2A (0)");
        }

        [Test]
        public void AeonRw_bg_jalr__()
        {
            Given_HexString("8628");
            AssertCode(     // bg.jalr?	r17
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call r17 (0)");
        }

        [Test]
        public void AeonRw_bt_jr()
        {
            Given_HexString("84E9");
            AssertCode(     // bt.jr	r7
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto r7");
        }

        [Test]
        public void AeonRw_bt_jr_ret()
        {
            Given_HexString("8529");
            AssertCode(     // bt.jr	r9
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void AeonRw_bg_lbs__()
        {
            Given_HexString("F4 E5 38 B2");
            AssertCode(     // bg.lbs?	r7,0x38B2(r5)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r5 + 14514<i32>:byte]",
                "2|L--|r7 = CONVERT(v4, int8, word32)");
        }

        [Test]
        public void AeonRw_bn_lbz()
        {
            Given_HexString("10 64 00");
            AssertCode(     // bn.lbz?	r3,(r4)
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = Mem0[r4:byte]",
                "2|L--|r3 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void AeonRw_bg_lbz__()
        {
            Given_HexString("F0 C7 1E EC");
            AssertCode(     // bg.lbz?	r6,0x1EEC(r7)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r7 + 7916<i32>:byte]",
                "2|L--|r6 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void AeonRw_bg_lhs()
        {
            Given_HexString("E8 E6 A5 82");
            AssertCode(     // bg.lhs?\tr3,(re)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r6 - 23166<i32>:int16]",
                "2|L--|r7 = CONVERT(v4, int16, word32)");
        }

        [Test]
        public void AeonRw_bn_lhz()
        {
            Given_HexString("08C301");
            AssertCode(     // bn.lhz	r6,(r3)
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = Mem0[r3:uint16]",
                "2|L--|r6 = CONVERT(v4, uint16, word32)");
        }

        [Test]
        public void AeonRw_bg_lhz__()
        {
            Given_HexString("E8C75CA5");
            AssertCode(     // bg.lhz?	r6,0x5CA4(r7)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r7 + 23716<i32>:word16]",
                "2|L--|r6 = CONVERT(v4, uint16, word32)");
        }

        [Test]
        public void AeonRw_bn_lwz()
        {
            Given_HexString("0CE302");
            AssertCode(     // bn.lwz	r7,(r3)
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = Mem0[r3:word32]");
        }

        [Test]
        public void AeonRw_bg_lwz()
        {
            Given_HexString("EEF2F312");
            AssertCode(     // bg.lwz	r23,-0xCF0(r18)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = Mem0[r18 - 3312<i32>:word32]");
        }

        [Test]
        public void AeonRw_bg_lwz_unlikely()
        {
            Given_HexString("EC12F312");
            AssertCode(     // bg.lwz	r23,-0xCF0(r18)
                "0|U--|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[r18 - 3312<i32>:word32]");
        }

        [Test]
        public void AeonRw_bg_mfspr()
        {
            Given_HexString("C030011F");
            AssertCode(     // bg.mfspr	r1,r16,0x11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = __move_from_spr(r16 | 0x11<32>)");
        }

        [Test]
       public void AeonRw_bg_mfspr_only_reg()
        {
            Given_HexString("C0A4000F");
            AssertCode(     // bg.mfspr	r5,r4,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __move_from_spr(r4)");
        }

        [Test]
        public void AeonRw_bg_mfspr_only_imm()
        {
            Given_HexString("C220F01F");
            AssertCode(     // bg.mfspr	r17,r0,0xF01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = __move_from_spr(0xF01<32>)");
        }

        [Test]
        public void AeonRw_bg_mfspr1__()
        {
            Given_HexString("C2700B47");
            AssertCode(     // bg.mfspr1?	r19,0x805A
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = __move_from_spr(0x805A<32>)");
        }

        [Test]
        public void AeonRw_bn_movhi__()
        {
            Given_HexString("046989");
            AssertCode(     // bn.movhi?	r3,0x989
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = 0x9890000<32>");
        }

        [Test]
        public void AeonRw_bg_movhi()
        {
            Given_HexString("C0EFFFE1");
            AssertCode(     // bg.movhi	r7,0x7FFF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = 0x7FFF0000<32>");
        }

        [Test]
        public void AeonRw_bt_movhi()
        {
            Given_HexString("95 50");
            AssertCode(     // bt.movhi?	r10,0x10
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r10 = 0x100000<32>");
        }

        [Test]
        public void AeonRw_bg_movhi_fuse_with_load()
        {
            Given_HexString(
                "C0 C0 0A 41" +
                "EC E6 3A 3E");
            AssertCode(
                // bg.movhi	r6,0x523A3C@hi
                // bg.lwz	r7,0x523A3C@lo(r6)
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r6 = 0x520000<32>",
                "2|L--|r7 = Mem0[0x00523A3C<p32>:word32]");
        }

        [Test]
        public void AeonRw_bg_movhi_fuse_with_bg_lbs__()
        {
            Given_HexString(
                "C0 E0 0A 61" +
                "F5 67 A1 B0");
            AssertCode(
                // bg.movhi	r7,0x0052A1B0@hi
                // bg.lbs?	r11,0x0052A1B0@lo(r7)
                "0|L--|00100000(8): 3 instructions",
                "1|L--|r7 = 0x530000<32>",
                "2|L--|v4 = Mem0[0x0052A1B0<p32>:byte]",
                "3|L--|r11 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void AeonRw_bg_movhi_fuse_with_store()
        {
            Given_HexString(
                "C0 E0 0A 41" +
                "F8 C7 3A 05");
            AssertCode(
                // bg.movhi	r7,0x523A05@hi
                // bg.sb?	0x523A05@lo(r7),r6
                "0|L--|00100000(8): 3 instructions",
                "1|L--|r7 = 0x520000<32>",
                "2|L--|v4 = SLICE(r6, byte, 0)",
                "3|L--|Mem0[0x00523A05<p32>:byte] = v4");
        }

        [Test]
        public void AeonRw_bg_movhi_fuse_with_bt_addi()
        {
            // haven't seen examples of this
            Given_HexString(
                "C0 E2 08 41" +
                "9C F0");
            AssertCode(
                // bg.movhi	r7,0x1041FFF0@hi
                // bt.addi?	r7,0x1041FFF0@lo
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r7 = 0x10420000<32>",
                "2|L--|r7 = 0x1041FFF0<32>");
        }

        [Test]
        public void AeonRw_bg_movhi_fuse_with_bg_addi()
        {
            Given_HexString(
                "C1 80 07 41" +
                "FD 4C 88 F0");
            AssertCode(
                // bg.movhi\tr12,0x3988F0@hi
                // bg.addi\tr10,r12,0x3988F0@lo
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r12 = 0x3A0000<32>",
                "2|L--|r10 = 0x3988F0<32>");
        }

        [Test]
        public void AeonRw_bn_movhi_fuse_with_bg_ori()
        {
            Given_HexString(
                "07 00 7F" +
                "CB 18 FF FF");
            AssertCode(
                // bn.movhi?	r24,0x7FFFFF@hi
	            // bg.ori	r24,r24,0x7FFFFF@lo
                "0|L--|00100000(7): 2 instructions",
                "1|L--|r24 = 0x7F0000<32>",
                "2|L--|r24 = 0x7FFFFF<32>");
        }

        [Test]
        public void AeonRw_bg_movhi_fuse_with_bn_ori()
        {
            Given_HexString(
                "C1 50 00 E1" +
                "51 4A 57");
            AssertCode(
                // bg.movhi	r10,0x80070057@hi
	            // bn.ori	r10,r10,0x80070057@lo
                "0|L--|00100000(7): 2 instructions",
                "1|L--|r10 = 0x80070000<32>",
                "2|L--|r10 = 0x80070057<32>");
        }

        [Test]
        public void AeonRw_bg_movhi_fuse_with_bg_ori()
        {
            Given_HexString(
                "C0 CF FF E1" +
                "C8 C6 FF FF");
            AssertCode(
                // bg.movhi\tr6,0x7FFFFFFF@hi
                // bg.ori\tr6,r6,0x7FFFFFFF@lo
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r6 = 0x7FFF0000<32>",
                "2|L--|r6 = 0x7FFFFFFF<32>");
        }

        [Test]
        public void AeonRw_bt_movhi_fuse_with_ori()
        {
            Given_HexString(
                "95 50" +
                "C8 6A 31 26");
            AssertCode(
                //"bt.movhi?\rr10,0x103126@hi",
                //"bg.ori\tr3,r10,0x103126@lo",
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r10 = 0x100000<32>",
                "2|L--|r3 = 0x103126<32>");
        }

        [Test]
        public void AeonRw_bg_mtspr()
        {
            Given_HexString("C218821D");
            AssertCode(     // bg.mtspr	r24,r16,0x821
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__move_to_spr(r24 | 0x821<32>, r16)");
        }

        [Test]
        public void AeonRw_bg_mtspr_only_reg()
        {
            Given_HexString("C268000D");
            AssertCode(     // bg.mtspr	r8,r19,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__move_to_spr(r8, r19)");
        }

        [Test]
        public void AeonRw_bg_mtspr_only_imm()
        {
            Given_HexString("C060011D");
            AssertCode(     // bg.mtspr	r0,r3,0x11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__move_to_spr(0x11<32>, r3)");
        }

        [Test]
        public void AeonRw_bg_mtspr1__()
        {
            Given_HexString("C2500425");
            AssertCode(     // bg.mtspr1?	r18,0x8021
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__move_to_spr(0x8021<32>, r18)");
        }

        [Test]
        public void AeonRw_bt_mov__()
        {
            Given_HexString("8AE5");
            AssertCode(     // bt.mov?	r23,r5
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r23 = r5");
        }

        [Test]
        public void AeonRw_bt_movi__()
        {
            Given_HexString("98 DF");
            AssertCode(     // bt.movi? r6,0x1F
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r6 = 0xFFFFFFFF<32>");
        }

        [Test]
        public void AeonRw_bn_mul()
        {
            Given_HexString("408323");
            AssertCode(     // bn.mul	r4,r3,r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = r3 *s64 r4",
                "2|L--|MULHI_r4 = v4");
        }

        [Test]
        public void AeonRw_bg_muli__()
        {
            Given_HexString("CCECFFE0");
            AssertCode(     // bg.muli?	r7,r12,-0x20
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = r12 *64 -32<i32>",
                "2|L--|MULHI_r7 = v3");
        }

        [Test]
        public void AeonRw_bn_mulu____()
        {
            Given_HexString("42FFA2");
            AssertCode(     // bn.mulu??	r23,r31,r20
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = r31 *u64 r20",
                "2|L--|MULHI_r23 = v4");
        }

        [Test]
        public void AeonRw_bn_nand__()
        {
            Given_HexString("44EA57");
            AssertCode(     // bn.nand?	r7,r10,r10
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = ~r10");
        }

        [Test]
        public void AeonRw_bn_nand__different()
        {
            // not from actual binary
            Given_HexString("44221F");
            AssertCode(     // bn.nand?	r1,r2,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r1 = ~(r2 & r3)");
        }

        [Test]
        public void AeonRw_bt_nop()
        {
            // confirmed with source
            Given_HexString("8001");
            AssertCode(     // bt.nop
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void AeonRw_bn_or()
        {
            Given_HexString("44E325");
            AssertCode(     // bn.or	r7,r3,r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r3 | r4");
        }

        [Test]
        public void AeonRw_bn_ori()
        {
            Given_HexString("506401");
            AssertCode(     // bn.ori	r3,r4,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = r4 | 1<32>");
        }

        [Test]
        public void AeonRw_bn_ori_zero() 
        {
            Given_HexString("506001");
            AssertCode(     // bn.ori	r3,r0,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = 1<32>");
        }

        [Test]
        public void AeonRw_bg_ori()
        {
            Given_HexString("C9BCF00F");
            AssertCode(     // bg.ori	r13,r28,0xF00F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = r28 | 0xF00F<32>");
        }

        [Test]
        public void AeonRw_bg_ori_zero()
        {
            Given_HexString("CAA00AAC");
            AssertCode(     // bg.ori	r21,r0,0xAAC
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = 0xAAC<32>");
        }

        [Test]
        public void AeonRw_bt_rfe()
        {
            // confirmed with source
            Given_HexString("8400");
            AssertCode(     // bt.rfe
                "0|R--|00100000(2): 2 instructions",
                "1|L--|__restore_exception_state(EPCR_0, ESR_0)",
                "2|R--|return (0,0)");
        }

        [Test]
        public void AeonRw_bn_ror__()
        {
            Given_HexString("4D B0 FF");
            AssertCode(     // bn.ror?	r13,r16,r31
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r13 = __ror<word32,word32>(r16, r31)");
        }

        [Test]
        public void AeonRw_bn_rori__()
        {
            Given_HexString("4D0433");
            AssertCode(     // bn.rori?	r8,r4,0x6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r8 = __ror<word32,word32>(r4, 6<32>)");
        }

        [Test]
        public void AeonRw_bn_rtnei__()
        {
            Given_HexString("5C805C");
            AssertCode(     // bn.rtnei?	0x2,0x2
                "0|L--|00100000(3): 3 instructions",
                "1|L--|r9 = Mem0[r1 + 12<i32>:word32]",
	            "2|L--|r10 = Mem0[r1 + 8<i32>:word32]",
	            "3|L--|r1 = r1 + 0x10<32>");
        }

        [Test]
        public void AeonRw_bn_sb__()
        {
            Given_HexString("18 EB 05"); 
            AssertCode(     // bn.sb?    0x5(r11),r7
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = SLICE(r7, byte, 0)",
                "2|L--|Mem0[r11 + 5<i32>:byte] = v4");
        }

        [Test]
        public void AeonRw_bg_sb__()
        {
            Given_HexString("F8 EA 36 D8");
            AssertCode(     // bg.sb? 0x36D8(r10),r7
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r7, byte, 0)",
                "2|L--|Mem0[r10 + 14040<i32>:byte] = v4");
        }

        [Test]
        public void AeonRw_bn_sfeq__()
        {
            Given_HexString("5DC605");
            AssertCode(     // bn.sfeq?	r14,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r14 == r6");
        }

        public void AeonRw_bn_sfeqi()
        {
            Given_HexString("5C8A21");
            AssertCode(     // bn.sfeqi	r4,0xA
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r4 == 0xA<32>");
        }

        [Test]
        public void AeonRw_bg_sfgesi__()
        {
            Given_HexString("C1 40 10 AC");
            AssertCode(     // bg.sfgesi\tr10, 0x85
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f = r10 >= 133<i32>");
        }

        [Test]
        public void AeonRw_bn_sfgesi__()
        {
            Given_HexString("5D 5F F9");
            AssertCode(     // bn.sfgesi?\tr10,-0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r10 >= -1<i32>");
        }

        [Test]
        public void AeonRw_bn_sfgeu()
        {
            Given_HexString("5E2717");
            AssertCode(     // bn.sfgeu	r17,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r7 >=u r17");
        }

        [Test]
        public void AeonRw_bg_sfgeui__()
        {
            Given_HexString("C0 80 1F E0");
            AssertCode(     // bg.sfgeui?	r4,0xFF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f = r4 >=u 0xFF<32>");
        }

        [Test]
        public void AeonRw_bn_sfgtu()
        {
            Given_HexString("5F 47 1F");
            AssertCode(     // bn.sfgtu	r26,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r26 >u r7");
        }

        [Test]
        public void AeonRw_bg_sfgtui__()
        {
            Given_HexString("C1603FCE");
            AssertCode(     // bg.sfgtui?	r11,0x1FE
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f = r11 >u 0x1FE<32>");
        }

        [Test]
        public void AeonRw_bn_sfgtui()
        {
            Given_HexString("5C641B");
            AssertCode(     // bn.sfgtui	r3,0x20
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r3 >u 0x20<32>");
        }

        [Test]
        public void AeonRw_bg_sflesi__()
        {
            Given_HexString("C0 60 E0 08");
            AssertCode(     // bg.sflesi?\tr3,0x700
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f = r3 <= 1792<i32>");
        }

        [Test]
        public void AeonRw_bn_sflesi__()
        {
            Given_HexString("5CE4B1");
            AssertCode(     // bn.sflesi?	r7,0x25
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r7 <= 37<i32>");
        }

        [Test]
        public void AeonRw_bn_sfleui__()
        {
            Given_HexString("5C 6E F3");
            AssertCode(     // bn.sfleui?	r3,0x77
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r3 <=u 0x77<32>");
        }

        [Test]
        public void AeonRw_bn_sflts__()
        {
            Given_HexString("5C C7 1D");
            AssertCode(     // bn.sflts?\tr6,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r6 < r7");
        }

        [Test]
        public void AeonRw_bn_sfne()
        {
            Given_HexString("5D8E0D");
            AssertCode(     // bn.sfne	r12,r14
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r12 != r14");
        }

        [Test]
        public void AeonRw_bn_sfnei__()
        {
            Given_HexString("5CE749");
            AssertCode(     // bn.sfnei?	r7,0x7,
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r7 != 0x3A<32>");
        }

        [Test]
        public void AeonRw_bg_sfnei__()
        {
            Given_HexString("C0C07D04");
            AssertCode(     // bg.sfnei? r6,0x3E8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f = r6 != 0x3E8<32>");
        }

        [Test]
        public void AeonRw_bn_sh__()
        {
            Given_HexString("0CBCC9");
            AssertCode(     // bn.sh? -0x38(r28),r5
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = SLICE(r5, word16, 0)",
                "2|L--|Mem0[r28 - 56<i32>:word16] = v4");
        }

        [Test]
        public void AeonRw_bn_lbz____()
        {
            Given_HexString("15 E3 03");
            AssertCode(     // bn.lbz??\t  0x3(r3)",
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v4 = Mem0[r3 + 3<i32>:byte]",
                "2|L--|r15 = CONVERT(v4, int8, word32)");
        }

        [Test]
        public void AeonRw_bg_sh__()
        {
            Given_HexString("EC67345B");
            AssertCode(     // bg.sh? 0x345A(r7),r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r3, word16, 0)",
                "2|L--|Mem0[r7 + 13402<i32>:word16] = v4");
        }

        [Test]
        public void AeonRw_bn_sll__()
        {
            Given_HexString("4CAC24");
            AssertCode(     // bn.sll?	r5,r12,r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r5 = r12 << r4");
        }

        [Test]
        public void AeonRw_bn_slli__()
        {
            Given_HexString("4C6320");
            AssertCode(     // bn.slli?	r3,r3,0x4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = r3 << 4<i32>");
        }

        [Test]
        public void AeonRw_bn_sra__()
        {
            Given_HexString("4C EB DE");
            AssertCode(     // bn.sra?\tr7,r11,r27
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r11 >> r27");
        }

        [Test]
        public void AeonRw_bn_srai()
        {
            Given_HexString("4CA70A");
            AssertCode(     // bn.srai?	r5,r7,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r5 = r7 >> 1<i32>");
        }

        [Test]
        public void AeonRw_bn_srl__()
        {
            Given_HexString("4D6B3D");
            AssertCode(     // bn.srl?	r11,r11,r7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r11 = r11 >>u r7");
        }

        [Test]
        public void AeonRw_bn_srli__()
        {
            Given_HexString("4C6319");
            AssertCode(     // bn.srli?	r3,r3,0x3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r3 = r3 >>u 3<i32>");
        }

        [Test]
        public void AeonRw_bn_sub()
        {
            Given_HexString("40EA1D");
            AssertCode(     // bn.sub	r7,r10,r3
                "0|L--|00100000(3): 3 instructions",
                "1|L--|r7 = r10 - r3",
                "2|L--|cy = cond(r7)",
                "3|L--|ov = cond(r7)");
        }

        [Test]
        public void AeonRw_bn_subb__()
        {
            Given_HexString("41 AD 36");
            // Always found right after a sub
            AssertCode(     // bn.subb?\tr13,r13,r6
                "0|L--|00100000(3): 3 instructions",
                "1|L--|r13 = r13 - r6 + cy",
                "2|L--|cy = cond(r13)",
                "3|L--|ov = cond(r13)");
        }

        [Test]
        public void AeonRw_bn_sw()
        {
            Given_HexString("0D41FC");
            AssertCode(     // bn.sw	-0x4(r1),r10
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Mem0[r1 - 4<i32>:word32] = r10");
        }

        [Test]
        public void AeonRw_bn_sw__0()
        {
            Given_HexString("0C0108");
            AssertCode(     // bn.sw?	0x8(r1),r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|Mem0[r1 + 8<i32>:word32] = 0<32>");
        }

        [Test]
        public void AeonRw_bg_sw()
        {
            // confirmed with source
            Given_HexString("ECA10084");
            AssertCode(     // bg.sw	0x84(r1),r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 + 132<i32>:word32] = r5");
        }

        [Test]
        public void AeonRw_bt_swst____()
        {
            Given_HexString("80FE");
            AssertCode(     // bt.swst??	0x3C(r1),r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r1 + 60<i32>:word32] = r7");
        }

        [Test]
        public void AeonRw_bg_syncwritebuffer()
        {
            Given_HexString("F4000005");
            AssertCode(     // bg.syncwritebuffer
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__syncwritebuffer()");
        }

        [Test]
        public void AeonRw_bn_xor__()
        {
            Given_HexString("44E736");
            AssertCode(     // bn.xor? r7,r7,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r7 ^ r6");
        } 
    }
}
