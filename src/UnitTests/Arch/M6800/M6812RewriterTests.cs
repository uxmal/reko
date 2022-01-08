using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Memory;
using Reko.Core.Rtl;

namespace Reko.UnitTests.Arch.M6800
{
    [TestFixture]
    public class M6812RewriterTests : RewriterTestBase
    {
        private Reko.Arch.M6800.M6812Architecture arch;
        private Address addr;

        public M6812RewriterTests()
        {
            this.arch = new Reko.Arch.M6800.M6812Architecture(CreateServiceContainer(), "m6812", new Dictionary<string, object>());
            this.addr = Address.Ptr16(0);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void M6812Rw_pshd()
        {
            Given_HexString("3B");
            AssertCode(     // pshd
                "0|L--|0000(1): 2 instructions",
                "1|L--|sp = sp - 2<i16>",
                "2|L--|Mem0[sp:word16] = d");
        }

        [Test]
        public void M6812Rw_rola()
        {
            Given_HexString("45");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|a = __rcl<byte,int8,bool>(a, 1<i8>, C)",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_rolb()
        {
            Given_HexString("55");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|b = __rcl<byte,int8,bool>(b, 1<i8>, C)",
                "2|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6812Rw_rora()
        {
            Given_HexString("46");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|a = __rcr<byte,int8,bool>(a, 1<i8>, C)",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_rorb()
        {
            Given_HexString("56");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|b = __rcr<byte,int8,bool>(b, 1<i8>, C)",
                "2|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6812Rw_suba_imm()
        {
            Given_HexString("8042");
            AssertCode(     // suba\t#$42
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a - 0x42<8>",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_cmpa_imm()
        {
            Given_HexString("8142");
            AssertCode(     // cmpa\t#$42
                "0|L--|0000(2): 1 instructions",
                "1|L--|NZVC = cond(a - 0x42<8>)");
        }

        [Test]
        public void M6812Rw_ldab_direct()
        {
            Given_HexString("F64242");
            AssertCode(     // ldab\t$4242
                "0|L--|0000(3): 3 instructions",
                "1|L--|b = Mem0[0x4242<p16>:byte]",
                "2|L--|NZ = cond(b)",
                "3|L--|V = false");
        }

        [Test]
        public void M6812Rw_lsla()
        {
            Given_HexString("48");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|a = a << 1<i8>",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_lslb()
        {
            Given_HexString("58");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|b = b << 1<i8>",
                "2|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6812Rw_lsld()
        {
            Given_HexString("59");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|d = d << 1<i8>",
                "2|L--|NZVC = cond(d)");
        }

        [Test]
        public void M6812Rw_lsra()
        {
            Given_HexString("44");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|a = a >>u 1<i8>",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_lsrb()
        {
            Given_HexString("54");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|b = b >>u 1<i8>",
                "2|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6812Rw_lsrd()
        {
            Given_HexString("49");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|d = d >>u 1<i8>",
                "2|L--|NZVC = cond(d)");
        }

        [Test]
        public void M6812Rw_aba()
        {
            Given_HexString("1806");
            AssertCode(     // aba
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a + b",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_adca_immediate()
        {
            Given_HexString("8942");
            AssertCode(     // adca\t#$42
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a + 0x42<8> + C");
        }

        [Test]
        public void M6812Rw_adca_direct()
        {
            Given_HexString("9942");
            AssertCode(     // adca\t$42
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a + Mem0[0x0042<p16>:byte] + C",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_addd_imm()
        {
            Given_HexString("C31234");
            AssertCode(     // addd\t#$1234
                "0|L--|0000(3): 2 instructions",
                "1|L--|d = d + 0x1234<16>",
                "2|L--|NZVC = cond(d)");
        }

        [Test]
        public void M6812Rw_bcc()
        {
            Given_HexString("24FE");
            AssertCode(     // bcc\t0000
                "0|T--|0000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0000");
        }

        [Test]
        public void M6812Rw_call_hl()
        {
            Given_HexString("4AFFAA70");
            AssertCode(
                "0|T--|0000(4): 1 instructions",
                "1|T--|call SEQ(0x70<8>, 0xFFAA<p16>) (3)");
        }

        [Test]
        public void M6812Rw_call_ind_x()
        {
            Given_HexString("4B2642");
            AssertCode(
                "0|T--|0000(3): 2 instructions",
                "1|L--|x = x + 7<i16>",
                "2|T--|call SEQ(0x42<8>, x) (3)");
        }

        [Test]
        public void M6812Rw_call_xb_00()
        {
            Given_HexString("4B00");
            AssertCode(
                "0|T--|0000(2): 1 instructions",
                "1|T--|call Mem0[x + 0<16>:byte] (3)");
        }

        [Test]
        public void M6812Rw_call_xb_2a()
        {
            Given_HexString("4B2AD5");
            AssertCode(
                "0|T--|0000(3): 2 instructions",
                "1|L--|x = x + -6<i16>",
                "2|T--|call SEQ(0xD5<8>, x) (3)");
        }

        [Test]
        public void M6812Rw_clr()
        {
            Given_HexString("6980");
            AssertCode(     // clr\t$0000,sp
                "0|L--|0000(2): 5 instructions",
                "1|L--|Mem0[sp + 0<16>:byte] = 0<8>",
                "2|L--|N = false",
                "3|L--|Z = true",
                "4|L--|V = false",
                "5|L--|C = false");
        }

        [Test]
        public void M6812Rw_lbcc()
        {
            Given_HexString("1824FFFC");
            AssertCode(     // lbcc\t0000
                "0|T--|0000(4): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0000");
        }

        [Test]
        public void M6812Rw_dbeq()
        {
            Given_HexString("041403");
            AssertCode(     // dbeq\td,0000
                "0|T--|0000(3): 2 instructions",
                "1|L--|d = d - 1<16>",
                "2|T--|if (d == 0<16>) branch 0000");
        }

        [Test]
        public void M6812Rw_oraa_predec()
        {
            Given_HexString("AA2C");
            AssertCode(     // oraa\t$04,-x
                "0|L--|0000(2): 4 instructions",
                "1|L--|x = x + -4<i16>",
                "2|L--|a = a | Mem0[x:byte]",
                "3|L--|NZ = cond(a)",
                "4|L--|V = false");
        }

        [Test]
        public void M6812Rw_suba_postinc()
        {
            Given_HexString("A033");
            AssertCode(     // suba\t$04,x+
                "0|L--|0000(2): 4 instructions",
                "1|L--|v4 = x",
                "2|L--|x = x + 4<i16>",
                "3|L--|a = a - Mem0[v4:byte]",
                "4|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_eora_9_bit_offset()
        {
            Given_HexString("A8E1FF");
            AssertCode(     // eora\t$FFFF,x
                "0|L--|0000(3): 3 instructions",
                "1|L--|a = a ^ Mem0[x + 0xFFFF<16>:byte]",
                "2|L--|NZ = cond(a)",
                "3|L--|V = false");
        }

        [Test]
        public void M6812Rw_andb_accumulator_offset()
        {
            Given_HexString("E4F4");
            AssertCode(     // andb\ta,sp
                "0|L--|0000(2): 3 instructions",
                "1|L--|b = b & Mem0[sp + CONVERT(a, byte, uint16):byte]",
                "2|L--|NZ = cond(b)",
                "3|L--|V = false");
        }

        [Test]
        public void M6812Rw_jmp_indirect()
        {
            Given_HexString("05FF");
            AssertCode(     // jmp\t[d,pc]
                "0|T--|0000(2): 1 instructions",
                "1|T--|goto Mem0[0x0002<p16> + d:ptr16]");
        }

        [Test]
        public void M6812Rw_asr_post()
        {
            Given_HexString("6730");
            AssertCode(    // asr
                "0|L--|0000(2): 5 instructions",
                "1|L--|v3 = x",
                "2|L--|x = x + 1<i16>",
                "3|L--|v4 = Mem0[v3:byte] >> 1<i8>",
                "4|L--|Mem0[v3:byte] = v4",
                "5|L--|NZVC = cond(v4)");
        }

        [Test]
        public void M6812Rw_asra()
        {
            Given_HexString("47");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|a = a >> 1<i8>",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_asrb()
        {
            Given_HexString("57");
            AssertCode(
                "0|L--|0000(1): 2 instructions",
                "1|L--|b = b >> 1<i8>",
                "2|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6812Rw_bclr_pre()
        {
            Given_HexString("0D6002");
            AssertCode(    // bclr
                "0|L--|0000(3): 5 instructions",
                "1|L--|y = y + 1<i16>",
                "2|L--|v3 = Mem0[y:byte] & 0xFD<8>",
                "3|L--|Mem0[y:byte] = v3",
                "4|L--|NZ = cond(v3)",
                "5|L--|V = false");
        }

        [Test]
        public void M6812Rw_bra()
        {
            Given_HexString("20FE");
            AssertCode(    // bra
                "0|T--|0000(2): 1 instructions",
                "1|T--|goto 0000");
        }
    }
}
