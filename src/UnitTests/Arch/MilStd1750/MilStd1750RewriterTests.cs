using NUnit.Framework;
using Reko.Arch.MilStd1750;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.MilStd1750
{
    public class MilStd1750RewriterTests : RewriterTestBase
    {
        private readonly MilStd1750Architecture arch;

        public MilStd1750RewriterTests()
        {
            this.arch = new MilStd1750Architecture(CreateServiceContainer(), "MS1750Rwa", new Dictionary<string, object>());
        }
        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr16(0x0100);

        [Test]
        public void MS1750Rw_a()
        {
            Given_HexString("A00E0001");
            AssertCode(     // a	gp0,1,gp14
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp0 = gp0 + Mem0[gp14 + 1<16>:word16]",
                "2|L--|CPZN = cond(gp0)");
        }

        [Test]
        public void MS1750Rw_ab()
        {
            Given_HexString("1102");
            AssertCode(     // ab	gp13,#2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = gp2 + Mem0[gp13 + 2<16>:word16]",
                "2|L--|CPZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_aim()
        {
            Given_HexString("4AB18060");
            AssertCode(     // aim	gp11,#0x8060
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp11 = gp11 + 0x8060<16>",
                "2|L--|CPZN = cond(gp11)");
        }

        [Test]
        public void MS1750Rw_aisp()
        {
            Given_HexString("A210");
            AssertCode(     // aisp	gp1,#1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp1 = gp1 + 1<16>",
                "2|L--|CPZN = cond(gp1)");
        }

        [Test]
        public void MS1750Rw_andm()
        {
            Given_HexString("4A270001");
            AssertCode(     // andm	gp2,#1
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp2 = gp2 & 1<16>",
                "2|L--|PZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_andr()
        {
            Given_HexString("E34D");
            AssertCode(     // andr     gp4,gp13
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp4 = gp4 & gp13",
                "2|L--|PZN = cond(gp4)");
        }

        [Test]
        public void MS1750Rw_andx()
        {
            Given_HexString("40EB");
            AssertCode(     // andx	gp12,gp11
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = gp2 & Mem0[gp12 + gp11:word16]",
                "2|L--|PZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_ar()
        {
            Given_HexString("A110");
            AssertCode(     // ar	gp1,gp0
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp1 = gp1 + gp0",
                "2|L--|CPZN = cond(gp1)");
        }

        [Test]
        public void MS1750Rw_bex()
        {
            Given_HexString("770B");
            AssertCode(     // bex	053C
                "0|T--|0100(1): 1 instructions",
                "1|L--|__syscall(0xB<16>)");
        }

        [Test]
        public void MS1750Rw_ble()
        {
            Given_HexString("78F1");
            AssertCode(     // ble	011B
                "0|T--|0100(1): 1 instructions",
                "1|T--|if (Test(LE,ZN)) branch 00F1");
        }

        [Test]
        public void MS1750Rw_blt()
        {
            Given_HexString("76C9");
            AssertCode(     // blt	01A4
                "0|T--|0100(1): 1 instructions",
                "1|T--|if (Test(LT,N)) branch 00C9");
        }

        [Test]
        public void MS1750Rw_bpt()
        {
            Given_HexString("FFFF");
            AssertCode(     // bpt
                "0|H--|0100(1): 1 instructions",
                "1|L--|__bpt()");
        }

        [Test]
        public void MS1750Rw_br()
        {
            Given_HexString("7403");
            AssertCode(     // br	03EB
                "0|T--|0100(1): 1 instructions",
                "1|T--|goto 0103");
        }

        [Test]
        public void MS1750Rw_c()
        {
            Given_HexString("F07E0002");
            AssertCode(     // c	gp7,2,gp14
                "0|L--|0100(2): 1 instructions",
                "1|L--|PZN = cond(gp7 - Mem0[gp14 + 2<16>:word16])");
        }

        [Test]
        public void MS1750Rw_cbl()
        {
            Given_HexString("F4F58000");
            AssertCode(     // cbl	gp15,0x8000,gp5
                "0|L--|0100(2): 7 instructions",
                "1|L--|v2 = gp5 + 0x8000<16>",
                "2|L--|v3 = Mem0[v2:word16]",
                "3|L--|v4 = Mem0[v2 + 1<i16>:word16]",
                "4|L--|C = v3 > v4",
                "5|L--|N = gp15 < v3",
                "6|L--|Z = v3 <= gp15 && gp15 <= v4",
                "7|L--|P = v4 < gp15");
        }

        [Test]
        public void MS1750Rw_cim()
        {
            Given_HexString("4A1A8000");
            AssertCode(     // cim	gp1,#0x8000
                "0|L--|0100(2): 1 instructions",
                "1|L--|PZN = cond(gp1 - 0x8000<16>)");
        }

        [Test]
        public void MS1750Rw_cisp()
        {
            Given_HexString("F212");
            AssertCode(     // cisp	gp1,#3
                "0|L--|0100(1): 1 instructions",
                "1|L--|PZN = cond(gp1 - 3<16>)");
        }

        [Test]
        public void MS1750Rw_co()
        {
            Given_HexString("48004000");
            AssertCode(     // co	gp0
                "0|S--|0100(2): 1 instructions",
                "1|L--|__console_output(gp0)");
        }

        [Test]
        public void MS1750Rw_da()
        {
            Given_HexString("A600805D");
            AssertCode(     // da	gp0,0x805D
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp0_gp1 = gp0_gp1 + Mem0[0x805D<p16>:word32]",
                "2|L--|CPZN = cond(gp0_gp1)");
        }

        [Test]
        public void MS1750Rw_dabs()
        {
            Given_HexString("A543");
            AssertCode(     // dabs	gp4,gp3
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp4_gp5 = abs(gp3_gp4)",
                "2|L--|PZN = cond(gp4_gp5)");
        }

        [Test]
        public void MS1750Rw_db()
        {
            Given_HexString("1C43");
            AssertCode(     // db	gp12,#0x43
                "0|L--|0100(1): 4 instructions",
                "1|L--|v3 = 0x43<16>",
                "2|L--|gp12 = gp0_gp1 /16 v3",
                "3|L--|gp13 = gp0_gp1 % v3",
                "4|L--|PZN = cond(gp12)");
        }

        [Test]
        public void MS1750Rw_dcr()
        {
            Given_HexString("F713");
            AssertCode(     // dcr	gp1,gp3
                "0|L--|0100(1): 1 instructions",
                "1|L--|PZN = cond(gp1_gp2 - gp3_gp4)");
        }

        [Test]
        public void MS1750Rw_disp()
        {
            Given_HexString("D219");
            AssertCode(     // disp	gp1,#0xA
                "0|L--|0100(1): 4 instructions",
                "1|L--|v3 = 0xA<16>",
                "2|L--|gp1 = gp1 / v3",
                "3|L--|gp2 = gp1 % v3",
                "4|L--|PZN = cond(gp1)");
        }

        [Test]
        public void MS1750Rw_dlb()
        {
            Given_HexString("04F6");
            AssertCode(     // dlb	gp12,#0xF6
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0_gp2 = Mem0[gp12 + 0xF6<16>:word32]",
                "2|L--|PZN = cond(gp0_gp2)");
        }

        [Test]
        public void MS1750Rw_dlr()
        {
            Given_HexString("8705");
            AssertCode(     // dlr	gp0,gp5
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0_gp1 = gp5_gp6",
                "2|L--|PZN = cond(gp0_gp1)");
        }

        [Test]
        public void MS1750Rw_dm()
        {
            Given_HexString("C6BD6487");
            AssertCode(     // dm	gp8,0x6487,gp13
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp11_gp12 = gp11_gp12 * Mem0[gp13 + 0x6487<16>:word32]",
                "2|L--|PZN = cond(gp11_gp12)");
        }

        [Test]
        public void MS1750Rw_dmr()
        {
            Given_HexString("C727");
            AssertCode(     // dmr	gp2,gp7
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2_gp3 = gp2_gp3 * gp7_gp8",
                "2|L--|PZN = cond(gp2_gp3)");
        }

        [Test]
        public void MS1750Rw_dneg()
        {
            Given_HexString("B511");
            AssertCode(     // dneg	gp1,gp1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp1_gp2 = -gp1_gp2",
                "2|L--|PZN = cond(gp1_gp2)");
        }

        [Test]
        public void MS1750Rw_dsar()
        {
            Given_HexString("6E6F");
            AssertCode(     // dsar	gp6,gp15
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp6_gp7 = __shift_arithmetic(gp6_gp7, gp15)",
                "2|L--|PZN = cond(gp6_gp7)");
        }

        [Test]
        public void MS1750Rw_dsr()
        {
            Given_HexString("B742");
            AssertCode(     // dsr	gp4,gp2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp4_gp5 = gp4_gp5 - gp2_gp3",
                "2|L--|CPZN = cond(gp4_gp5)");
        }

        [Test]
        public void MS1750Rw_dsra()
        {
            Given_HexString("67F2");
            AssertCode(     // dsra	gp2,#0x10
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2_gp3 = gp2_gp3 >> 0x10<16>",
                "2|L--|PZN = cond(gp2_gp3)");
        }

        [Test]
        public void MS1750Rw_dst()
        {
            Given_HexString("968E0001");
            AssertCode(     // dst	gp8,1,gp14
                "0|L--|0100(2): 1 instructions",
                "1|L--|Mem0[gp14 + 1<16>:word32] = gp8_gp9");
        }

        [Test]
        public void MS1750Rw_efa()
        {
            Given_HexString("AA8C0000");
            AssertCode(     // efa	gp8,gp12
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp8_gp9_gp10 = gp8_gp9_gp10 + Mem0[gp12:real48]",
                "2|L--|PZN = cond(gp8_gp9_gp10)");
        }

        [Test]
        public void MS1750Rw_efar()
        {
            Given_HexString("AB52");
            AssertCode(     // efar	gp5,gp2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp5_gp6_gp7 = gp5_gp6_gp7 + gp2_gp3_gp4",
                "2|L--|PZN = cond(gp5_gp6_gp7)");
        }

        [Test]
        public void MS1750Rw_efc()
        {
            Given_HexString("FA20802D");
            AssertCode(     // efc	gp2,0x802D
                "0|L--|0100(2): 1 instructions",
                "1|L--|PZN = cond(gp2_gp3_gp4 - Mem0[0x802D<p16>:real48])");
        }

        [Test]
        public void MS1750Rw_efd()
        {
            Given_HexString("DA3E000A");
            AssertCode(     // efd	gp3,0xA,gp14
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp3_gp4_gp5 = gp3_gp4_gp5 / Mem0[gp14 + 0xA<16>:real48]",
                "2|L--|PZN = cond(gp3_gp4_gp5)");
        }

        [Test]
        public void MS1750Rw_efdr()
        {
            Given_HexString("DB52");
            AssertCode(     // efdr	gp5,gp2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp5_gp6_gp7 = gp5_gp6_gp7 / gp2_gp3_gp4",
                "2|L--|PZN = cond(gp5_gp6_gp7)");
        }

        [Test]
        public void MS1750Rw_efix()
        {
            Given_HexString("EA05");
            AssertCode(     // efix	gp0,gp5
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0_gp1 = CONVERT(gp5_gp6_gp7, real48, int32)",
                "2|L--|PZN = cond(gp0_gp1)");
        }

        [Test]
        public void MS1750Rw_efl()
        {
            Given_HexString("8A428000");
            AssertCode(     // efl	gp4,0x8000,gp2
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp4_gp5_gp6 = Mem0[gp2 + 0x8000<16>:real48]",
                "2|L--|PZN = cond(gp4_gp5_gp6)");
        }

        [Test]
        public void MS1750Rw_eflt()
        {
            Given_HexString("EB62");
            AssertCode(     // eflt	gp6,gp2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp6_gp7_gp8 = CONVERT(gp2_gp3, int32, real48)",
                "2|L--|PZN = cond(gp6_gp7_gp8)");
        }

        [Test]
        public void MS1750Rw_efm()
        {
            Given_HexString("CA308003");
            AssertCode(     // efm	gp3,0x8003
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp3_gp4_gp5 = gp3_gp4_gp5 * Mem0[0x8003<p16>:real48]",
                "2|L--|PZN = cond(gp3_gp4_gp5)");
        }

        [Test]
        public void MS1750Rw_efmr()
        {
            Given_HexString("CB55");
            AssertCode(     // efmr	gp5,gp5
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp5_gp6_gp7 = gp5_gp6_gp7 * gp5_gp6_gp7",
                "2|L--|PZN = cond(gp5_gp6_gp7)");
        }

        [Test]
        public void MS1750Rw_efsr()
        {
            Given_HexString("BB82");
            AssertCode(     // efsr	gp8,gp2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp8_gp9_gp10 = gp8_gp9_gp10 - gp2_gp3_gp4",
                "2|L--|PZN = cond(gp8_gp9_gp10)");
        }

        [Test]
        public void MS1750Rw_efst()
        {
            Given_HexString("9A088090");
            AssertCode(     // efst	gp0,0x8090,gp8
                "0|L--|0100(2): 1 instructions",
                "1|L--|Mem0[gp8 + 0x8090<16>:real48] = gp0_gp1_gp2");
        }

        [Test]
        public void MS1750Rw_fab()
        {
            Given_HexString("2020");
            AssertCode(     // fab	gp12,#0x20
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0_gp1 = gp0_gp1 + Mem0[gp12 + 0x20<16>:real32]",
                "2|L--|CPZN = cond(gp0_gp1)");
        }


        [Test]
        public void MS1750Rw_fabs()
        {
            Given_HexString("AC60");
            AssertCode(     // fabs	gp6,gp0
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp6_gp7 = fabsf(gp0_gp1)",
                "2|L--|PZN = cond(gp6_gp7)");
        }

        [Test]
        public void MS1750Rw_fdb()
        {
            Given_HexString("2D20");
            AssertCode(     // fdb	gp13,#0x20
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0_gp1 = gp0_gp1 / Mem0[gp13 + 0x20<16>:real32]",
                "2|L--|PZN = cond(gp0_gp1)");
        }

        [Test]
        public void MS1750Rw_fmb()
        {
            Given_HexString("2B20");
            AssertCode(     // fmb	gp15,#0x20
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0_gp1 = gp0_gp1 * Mem0[gp15 + 0x20<16>:real32]",
                "2|L--|PZN = cond(gp0_gp1)");
        }

        [Test]
        public void MS1750Rw_fmr()
        {
            Given_HexString("C900");
            AssertCode(     // fmr	gp0,gp0
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0_gp1 = gp0_gp1 * gp0_gp1",
                "2|L--|PZN = cond(gp0_gp1)");
        }

        [Test]
        public void MS1750Rw_fneg()
        {
            Given_HexString("BC77");
            AssertCode(     // fneg	gp7,gp7
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp7_gp8 = -gp7_gp8",
                "2|L--|PZN = cond(gp7_gp8)");
        }

        [Test]
        public void MS1750Rw_incm()
        {
            Given_HexString("A30E0003");
            AssertCode(     // incm	#1,3,gp14
                "0|L--|0100(2): 3 instructions",
                "1|L--|v3 = Mem0[gp14 + 3<16>:word16] + 1<16>",
                "2|L--|Mem0[gp14 + 3<16>:word16] = v3",
                "3|L--|CPZN = cond(v3)");
        }

        [Test]
        public void MS1750Rw_jc()
        {
            Given_HexString("7020");
            AssertCode(     // jc	0140
                "0|T--|0100(1): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 0120");
        }

        [Test]
        public void MS1750Rw_js()
        {
            Given_HexString("726D616C");
            AssertCode(     // js	gp6,#0x616C,gp13
                "0|T--|0100(2): 3 instructions",
                "1|L--|v2 = gp13 + 0x616C<16>",
                "2|L--|gp6 = 0102",
                "3|T--|call v2 (0)");
        }

        [Test]
        public void MS1750Rw_lb()
        {
            Given_HexString("0001");
            AssertCode(     // lb	gp12,#1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = Mem0[gp12 + 1<16>:word16]",
                "2|L--|PZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_lbx()
        {
            Given_HexString("4000");
            AssertCode(     // lbx	gp12,gp0
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = Mem0[gp12 + gp0:word16]",
                "2|L--|PZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_lim()
        {
            Given_HexString("85008000");
            AssertCode(     // lim	gp0,#0x8000
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp0 = 0x8000<16>",
                "2|L--|PZN = cond(gp0)");
        }

        [Test]
        public void MS1750Rw_lm()
        {
            Given_HexString("89331102");
            AssertCode(     // lm	#0xB,0x1102,gp3
                "0|L--|0100(2): 5 instructions",
                "1|L--|v3 = gp3 + 0x1102<16>",
                "2|L--|gp0 = Mem0[v3:word16]",
                "3|L--|gp1 = Mem0[v3 + 1<i16>:word16]",
                "4|L--|gp2 = Mem0[v3 + 2<i16>:word16]",
                "5|L--|gp3 = Mem0[v3 + 3<i16>:word16]");
        }

        [Test]
        public void MS1750Rw_lub()
        {
            Given_HexString("8B0B0000");
            AssertCode(     // lub	gp0,gp11
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp0 = SEQ(SLICE(Mem0[gp11:word16], byte, 0), SLICE(gp0, byte, 0))",
                "2|L--|PZN = cond(gp0)");
        }

        [Test]
        public void MS1750Rw_lr()
        {
            Given_HexString("81EF");
            AssertCode(     // lr	gp14,gp15
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp14 = gp15",
                "2|L--|PZN = cond(gp14)");
        }

        [Test]
        public void MS1750Rw_llb()
        {
            Given_HexString("8C0B0000");
            AssertCode(     // llb	gp0,gp11
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp0 = SEQ(SLICE(gp0, byte, 8), SLICE(Mem0[gp11:word16], byte, 0))",
                "2|L--|PZN = cond(gp0)");
        }

        [Test]
        public void MS1750Rw_misn()
        {
            Given_HexString("C301");
            AssertCode(     // misn	gp1,#1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp1 = gp1 * -1<i16>",
                "2|L--|PZN = cond(gp1)");
        }

        [Test]
        public void MS1750Rw_mov()
        {
            Given_HexString("9302");
            AssertCode(     // mov	gp0,gp2
                "0|L--|0100(1): 3 instructions",
                "1|L--|v4 = gp0",
                "2|L--|v5 = gp2",
                "3|L--|__mov(v4, v5)");
        }

        [Test]
        public void MS1750Rw_lisp()
        {
            Given_HexString("8240");
            AssertCode(     // lisp	gp4,#1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp4 = 1<16>",
                "2|L--|PZN = cond(gp4)");
        }

        [Test]
        public void MS1750Rw_neg()
        {
            Given_HexString("B411");
            AssertCode(     // neg	gp1,gp1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp1 = -gp1",
                "2|L--|PZN = cond(gp1)");
        }

        [Test]
        public void MS1750Rw_nop()
        {
            Given_HexString("FF00");
            AssertCode(     // nop
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void MS1750Rw_orb()
        {
            Given_HexString("3030");
            AssertCode(     // orb	gp12,#0x30
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = gp2 | Mem0[gp12 + 0x30<16>:word16]",
                "2|L--|PZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_orim()
        {
            Given_HexString("4A48FF00");
            AssertCode(     // orim	gp4,#0xFF00
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp4 = gp4 | 0xFF00<16>",
                "2|L--|PZN = cond(gp4)");
        }

        [Test]
        public void MS1750Rw_orr()
        {
            Given_HexString("E102");
            AssertCode(     // orr	gp0,gp2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0 = gp0 | gp2");
        }

        [Test]
        public void MS1750Rw_popm()
        {
            Given_HexString("8FEE");
            AssertCode(     // popm	gp14,gp14
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void MS1750Rw_popm_wraparound()
        {
            Given_HexString("8FF3");
            AssertCode(     // popm gp15,gp3
                "0|L--|0100(1): 6 instructions",
                "1|L--|gp0 = Mem0[gp15:word16]",
                "2|L--|gp15 = gp15 + 1<i16>",
                "3|L--|gp1 = Mem0[gp15:word16]",
                "4|L--|gp15 = gp15 + 1<i16>",
                "5|L--|gp2 = Mem0[gp15:word16]",
                "6|L--|gp15 = gp15 + 1<i16>");
        }

        [Test]
        public void MS1750Rw_pshm()
        {
            Given_HexString("9FEE");
            AssertCode(     // pshm	gp14,gp14
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void MS1750Rw_sar()
        {
            Given_HexString("6B21");
            AssertCode(     // sar	gp2,gp1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = __shift_arithmetic(gp2, gp1)",
                "2|L--|PZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_s()
        {
            Given_HexString("B08C2E01");
            AssertCode(     // s	gp8,0x2E01,gp12
                "0|L--|0100(2): 2 instructions",
                "1|L--|gp8 = gp8 - Mem0[gp12 + 0x2E01<16>:word16]",
                "2|L--|CPZN = cond(gp8)");
        }

        [Test]
        public void MS1750Rw_sb()
        {
            Given_HexString("50000004");
            AssertCode(     // sb	#0,4
                "0|L--|0100(2): 2 instructions",
                "1|L--|v2 = Mem0[0x0004<p16>:word16]",
                "2|L--|Mem0[0x0004<p16>:word16] = v2 | 1<16>");
        }

        [Test]
        public void MS1750Rw_sbr()
        {
            Given_HexString("5197");
            AssertCode(     // sbr	#9,gp7
                "0|L--|0100(1): 1 instructions",
                "1|L--|gp7 = gp7 | 0x200<16>");
        }

        [Test]
        public void MS1750Rw_sisp()
        {
            Given_HexString("B2F0");
            AssertCode(     // sisp	gp15,#1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp15 = gp15 - 1<16>",
                "2|L--|CPZN = cond(gp15)");
        }

        [Test]
        public void MS1750Rw_sjs()
        {
            Given_HexString("7EF00115");
            AssertCode(     // sjs	gp15,0115
                "0|T--|0100(2): 1 instructions",
                "1|T--|call 0115 (2)");
        }

        [Test]
        public void MS1750Rw_sjs_indexed()
        {
            Given_HexString("7E01 08DD"); // 27065832A09591C5883C66ED");
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|call gp1 + 0x8DD<16> (2)");
        }

        [Test]
        public void MS1750Rw_sll()
        {
            Given_HexString("6001");
            AssertCode(     // sll	gp1,#1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp1 = gp1 << 1<16>",
                "2|L--|PZN = cond(gp1)");
        }

        [Test]
        public void MS1750Rw_slr()
        {
            Given_HexString("6A52");
            AssertCode(     // slr	gp5,gp2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp5 = __shift_logical(gp5, gp2)",
                "2|L--|PZN = cond(gp5)");
        }

        [Test]
        public void MS1750Rw_soj()
        {
            Given_HexString("739003F2");
            AssertCode(     // soj	gp9,03F2
                "0|T--|0100(2): 3 instructions",
                "1|L--|gp9 = gp9 - 1<16>",
                "2|L--|PZN = cond(gp9)",
                "3|T--|if (gp9 != 0<16>) branch 03F2");
        }

        [Test]
        public void MS1750Rw_sr()
        {
            Given_HexString("B121");
            AssertCode(     // sr	gp2,gp1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = gp2 - gp1",
                "2|L--|CPZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_sra()
        {
            Given_HexString("62E2");
            AssertCode(     // sra	gp2,#0xF
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = gp2 >> 0xF<16>",
                "2|L--|PZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_srl()
        {
            Given_HexString("6104");
            AssertCode(     // srl	gp4,#1
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp4 = gp4 >>u 1<16>",
                "2|L--|PZN = cond(gp4)");
        }

        [Test]
        public void MS1750Rw_st()
        {
            Given_HexString("903E0001");
            AssertCode(     // st	gp3,1,gp14
                "0|L--|0100(2): 1 instructions",
                "1|L--|Mem0[gp14 + 1<16>:word16] = gp3");
        }

        [Test]
        public void MS1750Rw_stc()
        {
            Given_HexString("911E0002");
            AssertCode(     // stc	#1,2,gp14
                "0|L--|0100(2): 1 instructions",
                "1|L--|Mem0[gp14 + 2<16>:word16] = 1<16>");
        }

        [Test]
        public void MS1750Rw_stlb()
        {
            Given_HexString("9CDB0002");
            AssertCode(     // stlb	gp13,2,gp11
                "0|L--|0100(2): 3 instructions",
                "1|L--|v2 = SLICE(gp13, byte, 0)",
                "2|L--|v4 = Mem0[gp11 + 2<16>:word16]",
                "3|L--|Mem0[gp11 + 2<16>:word16] = SEQ(SLICE(v4, byte, 8), v2)");
        }

        [Test]
        public void MS1750Rw_stub()
        {
            Given_HexString("9BDB0002");
            AssertCode(     // stub	gp13,2,gp11
                "0|L--|0100(2): 3 instructions",
                "1|L--|v2 = SLICE(gp13, byte, 0)",
                "2|L--|v4 = Mem0[gp11 + 2<16>:word16]",
                "3|L--|Mem0[gp11 + 2<16>:word16] = SEQ(v2, SLICE(v4, byte, 0))");
        }

        [Test]
        public void MS1750Rw_tbr()
        {
            Given_HexString("5784");
            AssertCode(     // tbr	#8,gp4
                "0|L--|0100(1): 2 instructions",
                "1|L--|v3 = gp4 & 1<u16> << 8<16>",
                "2|L--|Z = v3 == 0<16>");
        }

        [Test]
        public void MS1750Rw_urs()
        {
            Given_HexString("7FF0");
            AssertCode(     // urs	gp15
                "0|T--|0100(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void MS1750Rw_xio_unknown()
        {
            Given_HexString("4894AD51");
            AssertCode(     // unknown	#0xAD51
                "0|S--|0100(2): 1 instructions",
                "1|L--|__xio_unknown(0xAD51<u16>)");
        }

        [Test]
        public void MS1750Rw_xorr()
        {
            Given_HexString("E500");
            AssertCode(     // xorr	gp0,gp0
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp0 = gp0 ^ gp0",
                "2|L--|PZN = cond(gp0)");
        }

        [Test]
        public void MS1750Rw_xbr()
        {
            Given_HexString("EC20");
            AssertCode(     // xbr	gp2
                "0|L--|0100(1): 2 instructions",
                "1|L--|gp2 = __xbr(gp2)",
                "2|L--|PZN = cond(gp2)");
        }

        [Test]
        public void MS1750Rw_xwr()
        {
            Given_HexString("EDFB");
            AssertCode(     // xwr	gp15,gp11
                "0|L--|0100(1): 4 instructions",
                "1|L--|v4 = gp11",
                "2|L--|gp11 = gp15",
                "3|L--|gp15 = v4",
                "4|L--|PZN = cond(gp15)");
        }
    }
}
