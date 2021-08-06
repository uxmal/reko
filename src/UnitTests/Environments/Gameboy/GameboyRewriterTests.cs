using NUnit.Framework;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Environments.Gameboy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Environments.Gameboy
{
    [TestFixture]
    public class GameboyRewriterTests : Arch.RewriterTestBase
    {
        private readonly GameboyArchitecture arch;
        private readonly Address addr;

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        public GameboyRewriterTests()
        {
            this.arch = new GameboyArchitecture(CreateServiceContainer(), "lr35902", new Dictionary<string, object>());
            this.addr = Address.Ptr16(0x0100);
        }


        [Test]
        public void GameboyRw_nop()
        {
            Given_HexString("00");
            AssertCode(     // nop
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void GameboyRw_jp()
        {
            Given_HexString("C35001");
            AssertCode(     // jp	0150
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 0150");
        }

        [Test]
        public void GameboyRw_adc()
        {
            Given_HexString("CEED");
            AssertCode(     // adc	a,0EDh
                "0|L--|0100(2): 3 instructions",
                "1|L--|a = a + 0xED<8> + C");
        }

        [Test]
        public void GameboyRw_ld()
        {
            Given_HexString("66");
            AssertCode(     // ld	h,(hl)
                "0|L--|0100(1): 1 instructions",
                "1|L--|h = Mem0[hl:byte]");
        }

        [Test]
        public void GameboyRw_call_z()
        {
            Given_HexString("CCFF24");
            AssertCode(     // call	z,24FF
                "0|T--|0100(3): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 0103",
                "2|T--|call 24FF (2)");
        }

        [Test]
        public void GameboyRw_inc()
        {
            Given_HexString("03");
            AssertCode(     // inc	bc
                "0|L--|0100(1): 1 instructions",
                "1|L--|bc = bc + 1<16>");
        }

        [Test]
        public void GameboyRw_add()
        {
            Given_HexString("83");
            AssertCode(     // add	a,e
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = a + e",
                "2|L--|ZHC = cond(a)",
                "3|L--|N = false");
        }

        [Test]
        public void GameboyRw_di()
        {
            Given_HexString("F3");
            AssertCode(     // di
                "0|L--|0100(1): 1 instructions",
                "1|L--|__disable_interrupts()");
        }

        [Test]
        public void GameboyRw_xor()
        {
            Given_HexString("AF");
            AssertCode(     // xor	a
                "0|L--|0100(1): 5 instructions",
                "1|L--|a = a ^ a",
                "2|L--|Z = cond(a)",
                "3|L--|N = false",
                "4|L--|H = false",
                "5|L--|C = false");
        }

        [Test]
        public void GameboyRw_jr_nz()
        {
            Given_HexString("20FC");
            AssertCode(     // jr	nz,015F
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00FE");
        }

        [Test]
        public void GameboyRw_call()
        {
            Given_HexString("CDDF33");
            AssertCode(     // call	33DF
                "0|T--|0100(3): 1 instructions",
                "1|T--|call 33DF (2)");
        }

        [Test]
        public void GameboyRw_ldh()
        {
            Given_HexString("E042");
            AssertCode(     // ldh	(42h),a
                "0|L--|0100(2): 1 instructions",
                "1|L--|Mem0[0xFF42<p16>:byte] = a");
        }

        [Test]
        public void GameboyRw_ei()
        {
            Given_HexString("FB");
            AssertCode(     // ei
                "0|L--|0100(1): 1 instructions",
                "1|L--|__enable_interrupts()");
        }

        [Test]
        public void GameboyRw_push()
        {
            Given_HexString("C5");
            AssertCode(     // push	bc
                "0|L--|0100(1): 2 instructions",
                "1|L--|sp = sp - 2<i16>",
                "2|L--|Mem0[sp:word16] = bc");
        }

        [Test]
        public void GameboyRw_cp()
        {
            Given_HexString("FE01");
            AssertCode(     // cp	1h
                "0|L--|0100(2): 1 instructions",
                "1|L--|ZHC = cond(a - 1<8>)");
        }

        [Test]
        public void GameboyRw_ret_nc()
        {
            Given_HexString("D0");
            AssertCode(     // ret	nc
                "0|R--|0100(1): 2 instructions",
                "1|T--|if (Test(ULT,C)) branch 0101",
                "2|R--|return (2,0)");
        }

        [Test]
        public void GameboyRw_and()
        {
            Given_HexString("E67F");
            AssertCode(     // and	7Fh
                "0|L--|0100(2): 5 instructions",
                "1|L--|a = a & 0x7F<8>",
                "2|L--|Z = cond(a)",
                "3|L--|N = false",
                "4|L--|H = true",
                "5|L--|C = false");
        }

        [Test]
        public void GameboyRw_jr()
        {
            Given_HexString("180E");
            AssertCode(     // jr	3416
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 0110");
        }

        [Test]
        public void GameboyRw_or()
        {
            Given_HexString("B6");
            AssertCode(     // or	(hl)
                "0|L--|0100(1): 5 instructions",
                "1|L--|a = a | Mem0[hl:byte]",
                "2|L--|Z = cond(a)",
                "3|L--|N = false",
                "4|L--|H = false",
                "5|L--|C = false");
        }

        [Test]
        public void GameboyRw_reti()
        {
            Given_HexString("D9");
            AssertCode(     // reti
                "0|R--|0100(1): 2 instructions",
                "1|L--|__enable_interrupts()",
                "2|R--|return (2,0)");
        }

        [Test]
        public void GameboyRw_halt()
        {
            Given_HexString("76");
            AssertCode(     // halt
                "0|L--|0100(1): 1 instructions",
                "1|L--|__halt()");
        }

        [Test]
        public void GameboyRw_swap()
        {
            Given_HexString("CB37");
            AssertCode(     // swap	a
                "0|L--|0100(2): 5 instructions",
                "1|L--|a = __swap_nybbles(a)",
                "2|L--|Z = cond(a)",
                "3|L--|N = false",
                "4|L--|H = false",
                "5|L--|C = false");
        }

        [Test]
        public void GameboyRw_bit()
        {
            Given_HexString("CB5B");
            AssertCode(     // bit	3h,e
                "0|L--|0100(2): 3 instructions",
                "1|L--|Z = __test_bit(e, 3<8>)",
                "2|L--|N = false",
                "3|L--|H = true");
        }

        [Test]
        public void GameboyRw_res()
        {
            Given_HexString("CB9B");
            AssertCode(     // res	3h,e
                "0|L--|0100(2): 1 instructions",
                "1|L--|e = __reset_bit(e, 3<8>)");
        }

        [Test]
        public void GameboyRw_srl()
        {
            Given_HexString("CB3F");
            AssertCode(     // srl	a
                "0|L--|0100(2): 4 instructions",
                "1|L--|a = a >>u 1<8>",
                "2|L--|ZC = cond(a)",
                "3|L--|N = false",
                "4|L--|H = false");
        }

        [Test]
        public void GameboyRw_sla()
        {
            Given_HexString("CB25");
            AssertCode(     // sla	l
                "0|L--|0100(2): 4 instructions",
                "1|L--|l = l << 1<8>",
                "2|L--|ZC = cond(l)",
                "3|L--|N = false",
                "4|L--|H = false");
        }

        [Test]
        public void GameboyRw_jp_z()
        {
            Given_HexString("CA9214");
            AssertCode(     // jp	z,1492
                "0|T--|0100(3): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 1492");
        }

        [Test]
        public void GameboyRw_rl()
        {
            Given_HexString("CB16");
            AssertCode(     // rl	(hl)
                "0|L--|0100(2): 4 instructions",
                "1|L--|Mem0[hl:byte] = __rcl(Mem0[hl:byte], 1<8>, C)",
                "2|L--|ZC = cond(Mem0[hl:byte])",
                "3|L--|N = false",
                "4|L--|H = false");
        }

        [Test]
        public void GameboyRw_sbc()
        {
            Given_HexString("9E");
            AssertCode(     // sbc	a,(hl)
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = a - Mem0[hl:byte] - C",
                "2|L--|ZHC = cond(a)",
                "3|L--|N = true");
        }

        [Test]
        public void GameboyRw_rla()
        {
            Given_HexString("17");
            AssertCode(     // rla
                "0|L--|0100(1): 5 instructions",
                "1|L--|a = __rcl(a, 1<8>, C)",
                "2|L--|C = cond(a)",
                "3|L--|Z = false",
                "4|L--|N = false",
                "5|L--|H = false");
        }

        [Test]
        public void GameboyRw_cpl()
        {
            Given_HexString("2F");
            AssertCode(     // cpl
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = ~a",
                "2|L--|N = true",
                "3|L--|H = true");
        }

        [Test]
        public void GameboyRw_scf()
        {
            Given_HexString("37");
            AssertCode(     // scf
                "0|L--|0100(1): 3 instructions",
                "1|L--|N = false",
                "2|L--|H = false",
                "3|L--|C = true");
        }

        [Test]
        public void GameboyRw_ld_postdec()
        {
            Given_HexString("32");
            AssertCode(     // ld	(hl-),a
                "0|L--|0100(1): 3 instructions",
                "1|L--|v4 = hl",
                "2|L--|hl = hl - 1<i16>",
                "3|L--|Mem0[v4:byte] = a");
        }

        [Test]
        public void GameboyRw_ccf()
        {
            Given_HexString("3F");
            AssertCode(     // ccf
                "0|L--|0100(1): 3 instructions",
                "1|L--|C = !C",
                "2|L--|N = false",
                "3|L--|H = false");
        }

        [Test]
        public void GameboyRw_sub()
        {
            Given_HexString("90");
            AssertCode(     // sub	b
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = a - b",
                "2|L--|ZHC = cond(a)",
                "3|L--|N = true");
        }

        [Test]
        public void GameboyRw_rst()
        {
            Given_HexString("FF");
            AssertCode(     // rst	38h
                "0|L--|0100(1): 1 instructions",
                "1|T--|call 0038 (2)");
        }

        [Test]
        public void GameboyRw_ld_immediate()
        {
            Given_HexString("3100E0");
            AssertCode(     // ld	sp,0E000h
                "0|L--|0100(3): 1 instructions",
                "1|L--|sp = 0xE000<16>");
        }


        [Test]
        public void GameboyRw_dec()
        {
            Given_HexString("05");
            AssertCode(     // dec	b
                "0|L--|0100(1): 3 instructions",
                "1|L--|b = b - 1<8>",
                "2|L--|ZH = cond(b)",
                "3|L--|N = true");
        }

        [Test]
        public void GameboyRw_dec_regpair()
        {
            Given_HexString("1B");
            AssertCode(     // dec	de
                "0|L--|0100(1): 1 instructions",
                "1|L--|de = de - 1<16>");
        }


        [Test]
        public void GameboyRw_ld_absaddress()
        {
            Given_HexString("EAB7CB");
            AssertCode(     // ld	(0CBB7h),a
                "0|L--|0100(3): 1 instructions",
                "1|L--|Mem0[0xCBB7<p16>:byte] = a");
        }

        [Test]
        public void GameboyRw_add_sp_imm()
        {
            Given_HexString("E802");
            AssertCode(     // add	sp,2h
                "0|L--|0100(2): 4 instructions",
                "1|L--|sp = sp + 2<i16>",
                "2|L--|HC = cond(sp)",
                "3|L--|Z = false",
                "4|L--|N = false");
        }

        [Test]
        public void GameboyRw_add_reg_pair()
        {
            Given_HexString("09");
            AssertCode(     // add	hl,bc
                "0|L--|0100(1): 3 instructions",
                "1|L--|hl = hl + bc",
                "2|L--|HC = cond(hl)",
                "3|L--|N = false");
        }

        [Test]
        public void GameboyRw_add_8_bit()
        {
            Given_HexString("83");
            AssertCode(     // add	a,e
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = a + e",
                "2|L--|ZHC = cond(a)",
                "3|L--|N = false");
        }

        [Test]
        public void GameboyRw_inc_8bit()
        {
            Given_HexString("04");
            AssertCode(     // inc	b
                "0|L--|0100(1): 3 instructions",
                "1|L--|b = b + 1<8>",
                "2|L--|ZH = cond(b)",
                "3|L--|N = false");
        }


        [Test]
        public void GameboyRw_pop()
        {
            Given_HexString("E1");
            AssertCode(     // pop	hl
                "0|L--|0100(1): 2 instructions",
                "1|L--|hl = Mem0[sp:word16]",
                "2|L--|sp = sp + 2<i16>");
        }

        [Test]
        public void GameboyRw_stop()
        {
            Given_HexString("10");
            AssertCode(     // stop
                "0|H--|0100(1): 1 instructions",
                "1|L--|__stop()");
        }

        [Test]
        public void GameboyRw_rra()
        {
            Given_HexString("1F");
            AssertCode(     // rra
                "0|L--|0100(1): 5 instructions",
                "1|L--|a = __rcr(a, 1<8>, C)",
                "2|L--|C = cond(a)",
                "3|L--|Z = false",
                "4|L--|N = false",
                "5|L--|H = false");
        }

        [Test]
        public void GameboyRw_rlca()
        {
            Given_HexString("07");
            AssertCode(     // rlca
                "0|L--|0100(1): 5 instructions",
                "1|L--|a = __rol(a, 1<8>)",
                "2|L--|C = cond(a)",
                "3|L--|Z = false",
                "4|L--|N = false",
                "5|L--|H = false");
        }

        [Test]
        public void GameboyRw_rrca()
        {
            Given_HexString("0F");
            AssertCode(     // rrca
                "0|L--|0100(1): 5 instructions",
                "1|L--|a = __ror(a, 1<8>)",
                "2|L--|C = cond(a)",
                "3|L--|Z = false",
                "4|L--|N = false",
                "5|L--|H = false");
        }

        [Test]
        public void GameboyRw_set()
        {
            Given_HexString("CBFA");
            AssertCode(     // set	7h,d
                "0|L--|0100(2): 1 instructions",
                "1|L--|d = __set_bit(d, 7<8>)");
        }

        [Test]
        public void GameboyRw_daa()
        {
            Given_HexString("27");
            AssertCode(     // daa
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = __decimal_adjust(a)");
        }

        [Test]
        public void GameboyRw_sra()
        {
            Given_HexString("CB2B");
            AssertCode(     // sra	e
                "0|L--|0100(2): 5 instructions",
                "1|L--|e = e >> 1<8>",
                "2|L--|Z = cond(e)",
                "3|L--|N = false",
                "4|L--|H = false",
                "5|L--|C = false");
        }
    }
}
