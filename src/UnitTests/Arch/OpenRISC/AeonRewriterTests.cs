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
            this.arch = new AeonArchitecture(CreateServiceContainer(), "aeon", new());
            this.addrLoad = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addrLoad;

        [Test]
        public void AeonRw_bg_abs__()
        {
            Given_HexString("B8770014");
            AssertCode(     // bg.abs?	r3,r23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = abs<int32>(r23)");
        }

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
        public void AeonRw_bn_add_s()
        {
            Given_HexString("4737CB");
            AssertCode(     // bn.add.s	r25,r23,r25
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r25 = __add.s?(r23, r25)");
        }

        [Test]
        public void AeonRw_bt_add16()
        {
            Given_HexString("84 34");
            AssertCode(     // bt.add16\tr1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = r1 + 0x10<32>");
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
        public void AeonRw_bn_addc_s_lwq__()
        {
            Given_HexString("7CE430");
            AssertCode(     // bn.addc.s.lwq?	r7,r4,0x3,0x0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = __addc_s_lwq?(r4, 3<32>, 0<32>)");
        }

        [Test]
        public void AeonRw_bn_addc_s_lwqq__()
        {
            Given_HexString("7CF0B4");
            AssertCode(     // bn.addc.s.lwqq?	r7,r16,0x0,0x0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = __addc_s_lwqq?(r16, 0<32>, 0<32>)");
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
        public void AeonRw_bn_addcn_s__()
        {
            Given_HexString("6DE7FE");
            AssertCode(     // bn.addcn.s?	r15,r7,r31
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r15 = __addcn_s(r7, r31)");
        }

        [Test]
        public void AeonRw_bn_addcnqq_s__()
        {
            Given_HexString("60800E");
            AssertCode(     // bn.addcnqq.s?	r4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|__addcnqq_s(r4)");
        }

        [Test]
        public void AeonRw_bn_addcqq_s__()
        {
            Given_HexString("61FC84");
            AssertCode(     // bn.addcqq.s?	r15
                "0|L--|00100000(3): 1 instructions",
                "1|L--|__addcqq_s(r15)");
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
        public void AeonRw_bn_addp_s__()
        {
            Given_HexString("6D9884");
            AssertCode(     // bn.addp.s?	r12,r24,r16
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r12 = __addp_s(r24, r16)");
        }


        [Test]
        public void AeonRw_bg_amsb_lh__()
        {
            Given_HexString("BC12206F");
            AssertCode(     // bg.amsb.lh?	ac0,r18,r4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amsb_lh?(r18, r4)");
        }

        [Test]
        public void AeonRw_bg_amsb_wh__()
        {
            Given_HexString("BC08F06D");
            AssertCode(     // bg.amsb.wh?	ac0,r8,r30
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amsb_wh?(r8, r30)");
        }

        [Test]
        public void AeonRw_bg_amsb_wl__()
        {
            Given_HexString("BC1E506C");
            AssertCode(     // bg.amsb.wl?	ac0,r30,r10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amsb_wl?(r30, r10)");
        }

        [Test]
        public void AeonRw_bg_amul()
        {
            Given_HexString("BC08E848");
            AssertCode(     // bg.amul\tac0,r8,r29
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amul?(r8, r29)");
        }

        [Test]
        public void AeonRw_bn_addsf__()
        {
            Given_HexString("771AC0");
            AssertCode(     // bn.addsf?	r24,r26,r24
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r24 = r26 + r24");
        }

        [Test]
        public void AeonRw_bg_amac__()
        {
            Given_HexString("BC18C058");
            AssertCode(     // bg.amac?	ac0,r24,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amac?(r24, r24)");
        }

        [Test]
        public void AeonRw_bg_amac_hh__()
        {
            Given_HexString("BC1B785E");
            AssertCode(     // bg.amac.hh?	ac0,r27,r15
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amac_hh?(r27, r15)");
        }

        [Test]
        public void AeonRw_bg_amac_lh__()
        {
            Given_HexString("BC10405F");
            AssertCode(     // bg.amac.lh?	ac0,r16,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amac_lh?(r16, r8)");
        }

        [Test]
        public void AeonRw_bg_amac_ll__()
        {
            Given_HexString("BE06E0DA");
            AssertCode(     // bg.amac.ll?	ac1,r6,r28
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac1 = __amac_ll?(r6, r28)");
        }

        [Test]
        public void AeonRw_bg_amac_wh__()
        {
            Given_HexString("BC1AD05D");
            AssertCode(     // bg.amac.wh?	ac0,r26,r26
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amac_wh?(r26, r26)");
        }

        [Test]
        public void AeonRw_bg_amac_wl__()
        {
            Given_HexString("BC04D05C");
            AssertCode(     // bg.amac.wl?	ac0,r4,r26
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amac_wl?(r4, r26)");
        }

        [Test]
        public void AeonRw_bg_amacq_hl__()
        {
            Given_HexString("BE9AE09B");
            AssertCode(     // bg.amacq.hl?	ac1,r26,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac1 = __amacq_hl?(r26, 0<32>)");
        }

        [Test]
        public void AeonRw_bg_amacr__()
        {
            Given_HexString("AB800840");
            AssertCode(     // bg.amacr?	r28,ac0,0x1,0x20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = __amacr?(ac0, 1<32>, 0x20<32>)");
        }

        [Test]
        public void AeonRw_bg_amacr_ex__()
        {
            Given_HexString("AAE39CB0");
            AssertCode(     // bg.amacr.ex?	r23,ac1,0x13,0x18,0x0,0x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = __amacr_ex?(ac1, 0x13<32>, 0x18<32>, 0<32>, 3<32>)");
        }

        [Test]
        public void AeonRw_bg_amacrq__()
        {
            Given_HexString("BCE43000");
            AssertCode(     // bg.amacrq?	r7,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = __amacrq?(0<32>)");
        }

        [Test]
        public void AeonRw_bg_amacw__()
        {
            Given_HexString("BC1D5878");
            AssertCode(     // bg.amacw?	ac0,r29,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amacw?(r29, r11)");
        }

        [Test]
        public void AeonRw_bg_amadd__()
        {
            Given_HexString("BC000879");
            AssertCode(     // bg.amadd?	ac0,ac0,ac1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amadd?(ac0, ac1)");
        }

        [Test]
        public void AeonRw_bg_amsb__()
        {
            Given_HexString("BC19B868");
            AssertCode(     // bg.amsb?	ac0,r25,r23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amsb?(r25, r23)");
        }

        [Test]
        public void AeonRw_bg_amsub_hl__()
        {
            Given_HexString("BCE42FFB");
            AssertCode(     // bg.amsub.hl?	ac3,r4,r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac3 = __amsub_hl?(r4, r5)");
        }

        [Test]
        public void AeonRw_bg_amsub_wl__()
        {
            Given_HexString("BCE42FFC");
            AssertCode(     // bg.amsub.wl?	ac3,r4,r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac3 = __amsub_wl?(r4, r5)");
        }

        [Test]
        public void AeonRw_bg_amul_lh__()
        {
            Given_HexString("BC0FD84F");
            AssertCode(     // bg.amul.lh?	ac0,r15,r27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amul_lh?(r15, r27)");
        }

        [Test]
        public void AeonRw_bg_amul_ll__()
        {
            Given_HexString("BC1BD9CA");
            AssertCode(     // bg.amul.ll?	ac3,r27,r27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac3 = __amul_lh?(r27, r27)");
        }

        [Test]
        public void AeonRw_bg_amul_wh__()
        {
            Given_HexString("BC03C84D");
            AssertCode(     // bg.amul.wh?	ac0,r3,r25
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amul_wh?(r3, r25)");
        }

        [Test]
        public void AeonRw_bg_amul_wl__()
        {
            Given_HexString("BC1BE04C");
            AssertCode(     // bg.amul.wl?	ac0,r27,r28
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amul_wl?(r27, r28)");
        }

        [Test]
        public void AeonRw_bg_amulqu__()
        {
            Given_HexString("BDC2E009");
            AssertCode(     // bg.amulqu?	ac0,r2,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ac0 = __amulqu?(r2, 0<32>)");
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
        public void AeonRw_bn_andn()
        {
            Given_HexString("4719C1");
            AssertCode(     // bn.andn	r24,r25,r24
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r25 = r25 & ~r24");
        }

        [Test]
        public void AeonRw_bg_bgts__()
        {
            Given_HexString("D0 7F FB 7C");
            AssertCode(     // bg.bgts?\tr3,0x1F,000FFF6F
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r3 > 0x1F<32>) branch 000FFF6F"
);
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
                "1|T--|if (r3 == 0<i32>) branch 00100078");
        }

        [Test]
        public void AeonRw_bn_beqi___negative()
        {
            Given_HexString("207C68");
            AssertCode(     // bn.beqi?	r3,-0x1,0010001A
                "0|T--|00100000(3): 1 instructions",
                "1|T--|if (r3 == -1<i32>) branch 0010001A");
        }

        [Test]
        public void AeonRw_bg_beqi__()
        {
            Given_HexString("D0A700CA");
            AssertCode(     // bg.beqi?	r5,0x7,00100019
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r5 == 7<i32>) branch 00100019");
        }

        [Test]
        public void AeonRw_bg_beqi___negative()
        {
            Given_HexString("D29F24BA");
            AssertCode(     // bg.beqi?	r20,-0x1,00100497
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r20 == -1<i32>) branch 00100497");
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
        public void AeonRw_bg_bgtsi__()
        {
            Given_HexString("D3272F64");
            AssertCode(     // bg.bgtsi?	r25,0x7,001A3F92
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r25 > 7<32>) branch 001005EC");
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
        public void AeonRw_bg_bleui__()
        {
            Given_HexString("D0A70749");
            AssertCode(     // bg.bleui?	r5,0x7,0019C12D
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r5 <=u 7<i32>) branch 001000E9");
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
                "1|T--|if (r10 != 3<i32>) branch 0010006D");
        }

        [Test]
        public void AeonRw_bg_bnf()
        {
            Given_HexString("D6E4FFFF");
            AssertCode(     // bg.b111?	r23,r4,00205793
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (!f) branch 000FFFFF");
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
        public void AeonRw_bn_bsa__()
        {
            Given_HexString("64E7C3");
            AssertCode(     // bn.bsa?	r7,r7,r24
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = __bsa?(r7, r24)");
        }

        [Test]
        public void AeonRw_bn_bsl__()
        {
            Given_HexString("660E41");
            AssertCode(     // bn.bsl?	r16,r14,r8
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r16 = __bsl?(r14, r8)");
        }

        [Test]
        public void AeonRw_bn_bsa_s__()
        {
            Given_HexString("6739D7");
            AssertCode(     // bn.bsa.s?	r25,r25,r26
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r25 = __bsa_s?(r25, r26)");
        }
        [Test]
        public void AeonRw_bg_btb__()
        {
            Given_HexString("AE000000");
            AssertCode(     // bg.btb?	0x0,0x0,0x0,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__btb?(0<32>, 0<32>, 0<32>, 0<32>)");
        }


        [Test]
        public void AeonRw_bg_chk_ll__()
        {
            Given_HexString("D1886AE7");
            AssertCode(     // bg.b111i?	r12,0x8,00013740
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__chk_ll?(r12, Mem0[r8 + 184<i32>:byte], r13)");
        }

        [Test]
        public void AeonRw_bg_chk_lu__()
        {
            Given_HexString("D2C06003");
            AssertCode(     // bg.b011i?	r22,0x0,0001BB19
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__chk_lu?(r22, Mem0[r0:byte], r12)");
        }

        [Test]
        public void AeonRw_bn_clz__()
        {
            Given_HexString("5EE30E");
            AssertCode(     // bn.clz?	r23,r3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r23 = __count_leading_zeros<word32>(r3)");
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
        public void AeonRw_bn_cmovir__()
        {
            Given_HexString("4B4101");
            AssertCode(     // bn.cmovir?	r26,0x1,r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r26 = f ? 1<i32> : 0<32>");
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
        public void AeonRw_bn_conjc__()
        {
            Given_HexString("65000D");
            AssertCode(     // bn.conjc?	r8,r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r8 = __conjc?(0<32>)");
        }

        [Test]
        public void AeonRw_bn_crc_le__()
        {
            Given_HexString("64171C");
            AssertCode(     // bn.crc.le?	r0,r23,r3
                "0|U--|00100000(3): 1 instructions",
                "1|L--|r0 = __crc_le?(r23, r3)");
        }

        [Test]
        public void AeonRw_bg_dep__()
        {
            Given_HexString("BB89A442");
            AssertCode(     // bg.dep?	r28,r9,0x11,r20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = __dep(r9, 0x11<32>, r20)");
        }

        [Test]
        public void AeonRw_bg_depi__()
        {
            Given_HexString("BB590212");
            AssertCode(     // bg.depi?	r26,r25,0x8,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r26 = __dep(r25, 8<32>, 0<32>)");
        }

        [Test]
        public void AeonRw_bn_df2sf__()
        {
            Given_HexString("70000D");
            AssertCode(     // bn.df2sf?	r0,r0
                "0|U--|00100000(3): 1 instructions",
                "1|L--|r0 = __df2sf?(0<32>)");
        }

        [Test]
        public void AeonRw_bg_divl__()
        {
            Given_HexString("A02100C0");
            AssertCode(     // bg.divl?	r1,r1,r0,0x6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = __divl?(r1, 0<32>, 6<32>)");
        }

        [Test]
        public void AeonRw_bg_divlr__()
        {
            Given_HexString("A05A8899");
            AssertCode(     // bg.divlr?	r2,r26,r17,0x4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __divl?(r26, r17, 4<32>)");
        }

        [Test]
        public void AeonRw_bg_divlu__()
        {
            Given_HexString("A1FEF76E");
            AssertCode(     // bg.divlu?	r15,r30,r30,0x3B
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = __divlu?(r30, r30, 0x3B<32>)");
        }

        [Test]
        public void AeonRw_bg_divlru__()
        {
            Given_HexString("A1CAF78B");
            AssertCode(     // bg.divlru?	r14,r10,r30,0x3C
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r14 = __divlu?(r10, r30, 0x3C<32>)");
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
        public void AeonRw_bn_divsf__()
        {
            Given_HexString("772E0E");
            AssertCode(     // bn.divsf?	r25,r14,r1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r25 = r14 / r1");
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
        public void AeonRw_bg_dma_op__()
        {
            Given_HexString("F41AEC0A");
            AssertCode(     // bg.dma.op?	r26,r27
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dma_op(r26, r27)");
        }

        [Test]
        public void AeonRw_bn_dsl__()
        {
            Given_HexString("67EEF0");
            AssertCode(     // bn.dsl?	r31,r14,r30
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r31 = __dsl?(r14, r30)");
        }

        [Test]
        public void AeonRw_bn_dsli__()
        {
            Given_HexString("69FF38");
            AssertCode(     // bn.dsli?	r15,r31,0xE
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r15 = __dsli(r31, 0xE<32>)");
        }

        [Test]
        public void AeonRw_bn_dsr__()
        {
            Given_HexString("65CBA2");
            AssertCode(     // bn.dsr?	r14,r11,r20
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r14 = __dsr?(r11, r20)");
        }


        [Test]
        public void AeonRw_bn_dsri__()
        {
            Given_HexString("6B9705");
            AssertCode(     // bn.dsri?	r28,r23,0x1
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r28 = __dsri(r23, 1<32>)");
        }

        [Test]
        public void AeonRw_bt_ei__()
        {
            Given_HexString("8501");
            AssertCode(     // bt.ei?
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__enable_interrupts()");
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
        public void AeonRw_bn_exp__()
        {
            Given_HexString("5CEF0A");
            AssertCode(     // bn.exp?	r7,r15
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = __exp(r15)");
        }

        [Test]
        public void AeonRw_bn_exp16__()
        {
            Given_HexString("5C0CAA");
            AssertCode(     // bn.exp16?	r0,r12
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r0 = __exp16(r12)");
        }

        [Test]
        public void AeonRw_bg_ext__()
        {
            Given_HexString("BB80E400");
            AssertCode(     // bg.ext?	r28,r0,0x10,r28
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = __extract_bits(0<32>, 0x10<32>, r28)");
        }

        [Test]
        public void AeonRw_bn_extbs__()
        {
            Given_HexString("5C A5 02");
            AssertCode(     // bn.extbs?\tr5,r5
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v3 = SLICE(r5, int8, 0)",
                "2|L--|r5 = CONVERT(v3, int8, int32)");
        }

        [Test]
        public void AeonRw_bg_exti__()
        {
            Given_HexString("B9D74220");
            AssertCode(     // bg.exti?	r14,r23,0x8,0x8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r14 = __extract_bits(r23, 8<32>, 8<32>)");
        }

        [Test]
        public void AeonRw_bg_extui__()
        {
            Given_HexString("BAE64130");
            AssertCode(     // bg.extui?	r23,r6,0x4,0x8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = __extract_unsigned_bits(r6, 4<32>, 8<32>)");
        }

        [Test]
        public void AeonRw_bn_extbz__()
        {
            Given_HexString("5C 85 00");
            AssertCode(     // bn.extbz  r4,r5
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v3 = SLICE(r5, byte, 0)",
                "2|L--|r4 = CONVERT(v3, byte, uint32)");
        }

        [Test]
        public void AeonRw_bn_exths__()
        {
            Given_HexString("5F 67 06");
            AssertCode(     // bn.exths?\r27,r7
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v3 = SLICE(r7, int16, 0)",
                "2|L--|r27 = CONVERT(v3, int16, int32)");
        }

        [Test]
        public void AeonRw_bn_exthz__()
        {
            Given_HexString("5D 47 04");
            AssertCode(     // bn.exthz  r10,r7
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v3 = SLICE(r7, uint16, 0)",
                "2|L--|r10 = CONVERT(v3, uint16, uint32)");
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
        public void AeonRw_bg_fft__()
        {
            Given_HexString("BACAEB08");
            AssertCode(     // bg.fft?	0x0,r10,r29,0x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|0<32> = __fft(r10, r29, 3<32>)");
        }

        [Test]
        public void AeonRw_bn_fl1()
        {
            Given_HexString("5D4A0C");
            AssertCode(     // bn.fl1	r10,r10
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r10 = __fl1(r10)");
        }

        [Test]
        public void AeonRw_bn_flb()
        {
            Given_HexString("44D060");
            AssertCode(     // bn.flb	r6,r16,r12
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r6 = __flb(r16, r12)");
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
        public void AeonRw_bn_i2sf__()
        {
            Given_HexString("758892");
            AssertCode(     // bn.i2sf?	r12,r8
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r12 = CONVERT(r8, int32, real32)");
        }

        [Test]
        public void AeonRw_bg_invalidate()
        {
            Given_HexString("F4030003");
            AssertCode(     // bg.invalidate	(r3)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__invalidate_line(&Mem0[r3:word32])");
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
        public void AeonRw_bg_lbs__()
        {
            Given_HexString("F4 E5 38 B2");
            AssertCode(     // bg.lbs?	r7,0x38B2(r5)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r5 + 14514<i32>:byte]",
                "2|L--|r7 = CONVERT(v5, int8, word32)");
        }

        [Test]
        public void AeonRw_bn_lbz()
        {
            Given_HexString("10 64 00");
            AssertCode(     // bn.lbz?	r3,(r4)
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = Mem0[r4:byte]",
                "2|L--|r3 = CONVERT(v5, byte, word32)");
        }

        [Test]
        public void AeonRw_bg_lbz__()
        {
            Given_HexString("F0 C7 1E EC");
            AssertCode(     // bg.lbz?	r6,0x1EEC(r7)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r7 + 7916<i32>:byte]",
                "2|L--|r6 = CONVERT(v5, byte, word32)");
        }

        [Test]
        public void AeonRw_bg_ldww__()
        {
            Given_HexString("A5852900");
            AssertCode(     // bg.ldww?	r12,0x148(r5),0x0,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r12 = __ldww?(Mem0[r5 + 328<i32>:word32], 0<32>, 0<32>)");
        }

        [Test]
        public void AeonRw_bg_ldww2__()
        {
            Given_HexString("A40F2A66");
            AssertCode(     // bg.ldww2?	r0,0x153(r15),0x1,0x0
                "0|U--|00100000(4): 1 instructions",
                "1|L--|r0 = __ldww2?(Mem0[r15 + 339<i32>:word32], 1<32>, 0<32>)");
        }

        [Test]
        public void AeonRw_bg_ldww2x__()
        {
            Given_HexString("A7678893");
            AssertCode(     // bg.ldww2x?	r27,-0x3BC(r7),0x0,0x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r27 = __ldww2x?(Mem0[r7 - 956<i32>:word32], 0<32>, 1<32>)");
        }

        [Test]
        public void AeonRw_bg_ldwwx__()
        {
            Given_HexString("A4EE8891");
            AssertCode(     // bg.ldwwx?	r7,-0x3BC(r14),0x0,0x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = __ldwwx?(Mem0[r14 - 956<i32>:word32], 0<32>, 1<32>)");
        }

        [Test]
        public void AeonRw_bg_lhs()
        {
            Given_HexString("E8 E6 A5 82");
            AssertCode(     // bg.lhs?\tr3,(re)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r6 - 23166<i32>:int16]",
                "2|L--|r7 = CONVERT(v5, int16, word32)");
        }

        [Test]
        public void AeonRw_bn_lhz()
        {
            Given_HexString("08C301");
            AssertCode(     // bn.lhz	r6,(r3)
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = Mem0[r3:uint16]",
                "2|L--|r6 = CONVERT(v5, uint16, word32)");
        }

        [Test]
        public void AeonRw_bg_lhz__()
        {
            Given_HexString("E8C75CA5");
            AssertCode(     // bg.lhz?	r6,0x5CA4(r7)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r7 + 23716<i32>:word16]",
                "2|L--|r6 = CONVERT(v5, uint16, word32)");
        }

        [Test]
        public void AeonRw_bg_loop__()
        {
            Given_HexString("AF000039");
            AssertCode(     // bg.loop?	0x1C,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__loop?(0x1C<32>, r24)");
        }

        [Test]
        public void AeonRw_bg_loopi__()
        {
            Given_HexString("AC07EACB");
            AssertCode(     // bg.loopi?	0x165,0x7E
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__loop?(0x165<32>, 0x7E<32>)");
        }

        [Test]
        public void AeonRw_bt_lwst____()
        {
            Given_HexString("815D");
            AssertCode(     // bt.lwst??	r10,0x38(r1)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r10 = Mem0[r1 + 56<i32>:word32]");
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
        public void AeonRw_bn_maddsf__()
        {
            Given_HexString("740CD5");
            AssertCode(     // bn.maddsf?	r0,r12,r26
                "0|U--|00100000(3): 1 instructions",
                "1|L--|r0 = __maddsf?(r12, r26)");
        }

        [Test]
        public void AeonRw_bg_max__()
        {
            Given_HexString("BB5AC044");
            AssertCode(     // bg.max?	r26,r26,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r26 = max<int32>(r26, r24)");
        }

        [Test]
        public void AeonRw_bg_maxi__()
        {
            Given_HexString("B8630064");
            AssertCode(     // bg.maxi?	r3,r3,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = max<int32>(r3, 0<i32>)");
        }

        [Test]
        public void AeonRw_bg_maxu__()
        {
            Given_HexString("BA10B8C4");
            AssertCode(     // bg.maxu?	r16,r16,r23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = max<uint32>(r16, r23)");
        }

        [Test]
        public void AeonRw_bg_maxui__()
        {
            Given_HexString("B9AD00E4");
            AssertCode(     // bg.maxui?	r13,r13,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = max<uint32>(r13, 0<32>)");
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
        public void AeonRw_bg_min__()
        {
            Given_HexString("B863C884");
            AssertCode(     // bg.min?	r3,r3,r25
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = min<int32>(r3, r25)");
        }

        [Test]
        public void AeonRw_bg_mini__()
        {
            Given_HexString("BAF700A4");
            AssertCode(     // bg.mini?	r23,r23,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = min<int32>(r23, 0<i32>)");
        }

        [Test]
        public void AeonRw_bg_minui__()
        {
            Given_HexString("B98C0924");
            AssertCode(     // bg.minui?	r12,r12,0x9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r12 = min<uint32>(r12, 9<32>)");
        }

        [Test]
        public void AeonRw_bg_minu__()
        {
            Given_HexString("BBBDC004");
            AssertCode(     // bg.minu?	r29,r29,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r29 = min<uint32>(r29, r24)");
        }

        [Test]
        public void AeonRw_bn_mlwz__()
        {
            Given_HexString("31D2DE");
            AssertCode(     // bn.mlwz?	r14,-0x24(r18),0x2
                "0|L--|00100000(3): 1 instructions",
                "1|L--|__mlwz?(r14, Mem0[r18 - 36<i32>:word32], 2<32>)");
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
        public void AeonRw_bt_mov16()
        {
            Given_HexString("84B5");
            AssertCode(     // bt.mov16	r5
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = 0x10<32>");
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
                "2|L--|v5 = Mem0[0x0052A1B0<p32>:byte]",
                "3|L--|r11 = CONVERT(v5, byte, word32)");
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
                "2|L--|v5 = SLICE(r6, byte, 0)",
                "3|L--|Mem0[0x00523A05<p32>:byte] = v5");
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
        public void AeonRw_bt_movi__()
        {
            Given_HexString("98 DF");
            AssertCode(     // bt.movi? r6,0x1F
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r6 = 0xFFFFFFFF<32>");
        }

        [Test]
        public void AeonRw_bn_msw__()
        {
            Given_HexString("37FC63");
            AssertCode(     // bn.msw?	0x60(r28),r31,0x3
                "0|L--|00100000(3): 1 instructions",
                "1|L--|__msw?(Mem0[r28 + 96<i32>:word32], r31, 3<32>)");
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
        public void AeonRw_bn_mul()
        {
            Given_HexString("408323");
            AssertCode(     // bn.mul	r4,r3,r4
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = r3 *s64 r4",
                "2|L--|MULHI_r4 = v5");
        }

        [Test]
        public void AeonRw_bn_muladdh__()
        {
            Given_HexString("6DC060");
            AssertCode(     // bn.muladdh?	r14,r0,r12
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r14 = __muladdh(0<32>, r12)");
        }

        [Test]
        public void AeonRw_bn_muladdhx__()
        {
            Given_HexString("6C8E41");
            AssertCode(     // bn.muladdhx?	r0,r14,r8
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = __muladdhx(r14, r8)");
        }

        [Test]
        public void AeonRw_bg_muli__()
        {
            Given_HexString("CCECFFE0");
            AssertCode(     // bg.muli?	r7,r12,-0x20
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = r12 *64 -32<i32>",
                "2|L--|MULHI_r7 = v4");
        }

        [Test]
        public void AeonRw_bn_mulsf__()
        {
            Given_HexString("77418C");
            AssertCode(     // bn.mulsf?	r26,r1,r17
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r26 = r1 * r17");
        }

        [Test]
        public void AeonRw_bn_mulsubh__()
        {
            Given_HexString("6C5F5A");
            AssertCode(     // bn.mulsubh?	r2,r31,r11
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r2 = __mulsubh(r31, r11)");
        }

        [Test]
        public void AeonRw_bn_mulsubhx__()
        {
            Given_HexString("6ECB7B");
            AssertCode(     // bn.mulsubhx?	r22,r11,r15
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r22 = __mulsubhx(r11, r15)");
        }

        [Test]
        public void AeonRw_bn_mulu____()
        {
            Given_HexString("42FFA2");
            AssertCode(     // bn.mulu??	r23,r31,r20
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = r31 *u64 r20",
                "2|L--|MULHI_r23 = v5");
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
        public void AeonRw_bn_nop()
        {
            // confirmed with source
            Given_HexString("000000");
            AssertCode(     // bn.nop
                "0|L--|00100000(3): 1 instructions",
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
        public void AeonRw_bn_pack_s__()
        {
            Given_HexString("64806E");
            AssertCode(     // bn.pack.s?	r4,r0,r13
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = __pack_s?(0<32>, r13)");
        }

        [Test]
        public void AeonRw_bg_pamac__()
        {
            Given_HexString("BCE43002");
            AssertCode(     // bg.pamac?	0xE,0x1,0x0,0x0,0x2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|0xE<32> = __pamac?(1<32>, 0<32>, 0<32>, 2<32>)");
        }

        [Test]
        public void AeonRw_bg_pamacrq__()
        {
            Given_HexString("BCE43001");
            AssertCode(     // bg.pamacrq?	r7,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = __pamacrq?(0<32>)");
        }

        [Test]
        public void AeonRw_bt_push__()
        {
            Given_HexString("84EB");
            AssertCode(     // bt.push?	r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__push?(r7)");
        }

        [Test]
        public void AeonRw_bn_remsf__()
        {
            Given_HexString("779887");
            AssertCode(     // bn.remsf?	r28,r24,r16
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r28 = r24 % r16");
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
        public void AeonRw_bt_return__()
        {
            Given_HexString("87E7");
            AssertCode(     // bt.return?
                "0|R--|00100000(2): 2 instructions",
                "1|L--|__return?()",
                "2|R--|return (0,0)");
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
                "1|L--|v5 = SLICE(r7, byte, 0)",
                "2|L--|Mem0[r11 + 5<i32>:byte] = v5");
        }

        [Test]
        public void AeonRw_bg_sb__()
        {
            Given_HexString("F8 EA 36 D8");
            AssertCode(     // bg.sb? 0x36D8(r10),r7
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r7, byte, 0)",
                "2|L--|Mem0[r10 + 14040<i32>:byte] = v5");
        }

        [Test]
        public void AeonRw_bn_sf2df__()
        {
            Given_HexString("730014");
            AssertCode(     // bn.sf2df?	r24,r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r24 = __sf2df(0<32>)");
        }

        [Test]
        public void AeonRw_bn_sf2i__()
        {
            Given_HexString("779703");
            AssertCode(     // bn.sf2i?	r28,r23
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r28 = CONVERT(r23, real32, int32)");
        }

        [Test]
        public void AeonRw_bn_sfeq__()
        {
            Given_HexString("5DC605");
            AssertCode(     // bn.sfeq?	r14,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r14 == r6");
        }

        [Test]
        public void AeonRw_bn_sfeqdf__()
        {
            Given_HexString("78EE8A");
            AssertCode(     // bn.sfeqdf?	r14,r17
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r14 == r17");
        }

        public void AeonRw_bn_sfeqi()
        {
            Given_HexString("5C8A21");
            AssertCode(     // bn.sfeqi	r4,0xA
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r4 == 0xA<32>");
        }

        [Test]
        public void AeonRw_bn_sfeqsf__()
        {
            Given_HexString("78D1E0");
            AssertCode(     // bn.sfeqsf?	r17,r28
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r17 == r28");
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
        public void AeonRw_bn_sfgtdf__()
        {
            Given_HexString("780C97");
            AssertCode(     // bn.sfgtdf?	r12,r18
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r12 > r18");
        }

        [Test]
        public void AeonRw_bt_sfgtsi_minus32769__()
        {
            Given_HexString("86F2");
            AssertCode(     // bt.sfgtsi.minus32769?	r23
                "0|L--|00100000(2): 1 instructions",
                "1|L--|f = __sfgtsi_minus32769(r23)");
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
        public void AeonRw_bt_sfgtui_minus32769__()
        {
            Given_HexString("8410");
            AssertCode(     // bt.sfgtui.minus32769?	r0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|f = __sfgtui_minus32769(0<32>)");
        }

        [Test]
        public void AeonRw_bn_sfledf__()
        {
            Given_HexString("7A1943");
            AssertCode(     // bn.sfledf?	r25,r8
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r25 <= r8");
        }

        [Test]
        public void AeonRw_bn_sflesf__()
        {
            Given_HexString("7A0DA1");
            AssertCode(     // bn.sflesf?	r13,r20
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = r13 <= r20");
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
        public void AeonRw_bn_sfnesf__()
        {
            Given_HexString("7A809C");
            AssertCode(     // bn.sfnesf?	r0,r19
                "0|L--|00100000(3): 1 instructions",
                "1|L--|f = 0<32> != r19");
        }

        [Test]
        public void AeonRw_bn_sh__()
        {
            Given_HexString("0CBCC9");
            AssertCode(     // bn.sh? -0x38(r28),r5
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = SLICE(r5, word16, 0)",
                "2|L--|Mem0[r28 - 56<i32>:word16] = v5");
        }

        [Test]
        public void AeonRw_bn_lbz____()
        {
            Given_HexString("15 E3 03");
            AssertCode(     // bn.lbz??\t  0x3(r3)",
                "0|L--|00100000(3): 2 instructions",
                "1|L--|v5 = Mem0[r3 + 3<i32>:byte]",
                "2|L--|r15 = CONVERT(v5, int8, word32)");
        }

        [Test]
        public void AeonRw_bn_sat__()
        {
            Given_HexString("7318C0");
            AssertCode(     // bn.sat?	r24,r24,0x18
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r24 = __sat(r24, 0x18<32>)");
        }

        [Test]
        public void AeonRw_bn_satsu__()
        {
            Given_HexString("727DEA");
            AssertCode(     // bn.satsu?	r19,r29,0x1D
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r19 = __satsu(r29, 0x1D<32>)");
        }

        [Test]
        public void AeonRw_bn_satu__()
        {
            Given_HexString("73D301");
            AssertCode(     // bn.satu?	r30,r19,0x0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r30 = __satu(r19, 0<32>)");
        }

        [Test]
        public void AeonRw_bn_satus__()
        {
            Given_HexString("70813B");
            AssertCode(     // bn.satus?	r4,r1,0x7
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = __satus(r1, 7<32>)");
        }

        [Test]
        public void AeonRw_bg_sh__()
        {
            Given_HexString("EC67345B");
            AssertCode(     // bg.sh 0x345A(r7),r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r3, word16, 0)",
                "2|L--|Mem0[r7 + 13402<i32>:word16] = v5");
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
        public void AeonRw_bn_srari__()
        {
            Given_HexString("60E721");
            AssertCode(     // bn.srari?	r7,r7,0x4
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r7 >> 4<i32>");
        }

        [Test]
        public void AeonRw_bn_srarzi__()
        {
            Given_HexString("60E402");
            AssertCode(     // bn.srarzi?	r7,r4,0x0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = __srarzi?(r4, 0<32>)");
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
        public void AeonRw_bn_srlri__()
        {
            Given_HexString("6000C8");
            AssertCode(     // bn.srlri?	r0,r0,0x19
                "0|U--|00100000(3): 1 instructions",
                "1|L--|r0 = 0<32> >>u 25<i32>");
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
        public void AeonRw_bn_sub_s()
        {
            Given_HexString("4797C2");
            AssertCode(     // bn.sub.s	r28,r23,r24
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r28 = __sub_s(r23, r24)");
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
        public void AeonRw_bn_subc_s_lwq__()
        {
            Given_HexString("7EED81");
            AssertCode(     // bn.subc.s.lwq?	r23,r13,0x0,0x0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r23 = __sub_s(r13, 0<32>, 0<32>)");
        }

        [Test]
        public void AeonRw_bn_subcn_s__()
        {
            Given_HexString("6EE7FF");
            AssertCode(     // bn.subcn.s?	r23,r7,r31
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r23 = __subcn_s(r7, r31)");
        }

        [Test]
        public void AeonRw_bn_subcn_s_lwq__()
        {
            Given_HexString("7C0C8B");
            AssertCode(     // bn.subcn.s.lwq?	r0,r12,0x0,0x1
                "0|U--|00100000(3): 1 instructions",
                "1|L--|r0 = __subcn_s_lwq(r12, 0<32>, 1<32>)");
        }

        [Test]
        public void AeonRw_bn_subcnqq_s__()
        {
            Given_HexString("60011F");
            AssertCode(     // bn.subcnqq.s?	r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|__subcnqq_s(0<32>)");
        }

        [Test]
        public void AeonRw_bn_subcqq_s__()
        {
            Given_HexString("60011D");
            AssertCode(     // bn.subcqq.s?	r0
                "0|L--|00100000(3): 1 instructions",
                "1|L--|__subcqq_s(0<32>)");
        }

        [Test]
        public void AeonRw_bn_subsf__()
        {
            Given_HexString("74E431");
            AssertCode(     // bn.subsf?	r7,r4,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r4 - r6");
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
        public void AeonRw_bt_syncp__()
        {
            Given_HexString("860C");
            AssertCode(     // bt.syncp?
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__syncp()");
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
        public void AeonRw_bt_sys__()
        {
            Given_HexString("8683");
            AssertCode(     // bt.sys?
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__sys()");
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
        public void AeonRw_bn_xor__()
        {
            Given_HexString("44E736");
            AssertCode(     // bn.xor? r7,r7,r6
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r7 = r7 ^ r6");
        }

        [Test]
        public void AeonRw_bg_xori__()
        {
            Given_HexString("DA9F9001");
            AssertCode(     // bg.xori? r20,r31,0x9001
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r20 = r31 ^ 0x9001<u16>");
        }

        [Test]
        public void AeonRw_bn_xori()
        {
            Given_HexString("58 84 08");
            AssertCode(     // bt.xori
                "0|L--|00100000(3): 1 instructions",
                "1|L--|r4 = r4 ^ 8<32>");
        }

        [Test]
        public void AeonRw_mv_opv__()
        {
            Given_HexString("B702D303");
            AssertCode(     // mv.opv?	v24,v2,v26,v12,0x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mv_opv?(v24, v2, v26, v12, 1<32>)");
        }
    }
}
