using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Rtl;

namespace Reko.UnitTests.Arch.M6800
{
    [TestFixture]
    public class M6812RewriterTests : RewriterTestBase
    {
        private Reko.Arch.M6800.M6812Architecture arch;
        private Address addr;
        private MemoryArea image;

        public M6812RewriterTests()
        {
            this.arch = new Reko.Arch.M6800.M6812Architecture("m6812");
            this.addr = Address.Ptr16(0);
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return addr; }
        }


        private void Given_Code(string hex)
        {
            base.Rewrite(hex);
        }

        protected override MemoryArea RewriteCode(string hexBytes)
        {
            var bytes = PlatformDefinition.LoadHexBytes(hexBytes)
                .ToArray();
            this.image = new MemoryArea(LoadAddress, bytes);
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(
                new BeImageReader(image, image.BaseAddress),
                arch.CreateProcessorState(),
                binder,
                host);
        }

        [Test]
        public void M6812Rw_pshd()
        {
            Given_Code("3B");
            AssertCode(     // pshd
                "0|L--|0000(1): 2 instructions",
                "1|L--|sp = sp - 2",
                "2|L--|Mem0[sp:word16] = d");
        }

        [Test]
        public void M6812Rw_clr()
        {
            Given_Code("6980");
            AssertCode(     // clr\t$0000,sp
                "0|L--|0000(2): 5 instructions",
                "1|L--|Mem0[sp + 0x0000:byte] = 0x00",
                "2|L--|N = false",
                "3|L--|Z = true",
                "4|L--|V = false",
                "5|L--|C = false");
        }

        [Test]
        public void M6812Rw_suba_imm()
        {
            Given_Code("8042");
            AssertCode(     // suba\t#$42
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a - 0x42",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_cmpa_imm()
        {
            Given_Code("8142");
            AssertCode(     // cmpa\t#$42
                "0|L--|0000(2): 1 instructions",
                "1|L--|NZVC = cond(a - 0x42)");
        }

        [Test]
        public void M6812Rw_ldab_direct()
        {
            Given_Code("F64242");
            AssertCode(     // ldab\t$4242
                "0|L--|0000(3): 3 instructions",
                "1|L--|b = Mem0[0x4242:byte]",
                "2|L--|NZ = cond(b)",
                "3|L--|V = false");
        }

        [Test]
        public void M6812Rw_aba()
        {
            Given_Code("1806");
            AssertCode(     // aba
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a + b",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_adca_immediate()
        {
            Given_Code("8942");
            AssertCode(     // adca\t#$42
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a + 0x42 + C");
        }

        [Test]
        public void M6812Rw_adca_direct()
        {
            Given_Code("9942");
            AssertCode(     // adca\t$42
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a + Mem0[0x0042:byte] + C",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_addd_imm()
        {
            Given_Code("C31234");
            AssertCode(     // addd\t#$1234
                "0|L--|0000(3): 2 instructions",
                "1|L--|d = d + 0x1234",
                "2|L--|NZVC = cond(d)");
        }

        [Test]
        public void M6812Rw_bcc()
        {
            Given_Code("24FE");
            AssertCode(     // bcc\t0000
                "0|T--|0000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0000");
        }

        [Test]
        public void M6812Rw_lbcc()
        {
            Given_Code("1824FFFC");
            AssertCode(     // lbcc\t0000
                "0|T--|0000(4): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0000");
        }

        [Test]
        public void M6812Rw_dbeq()
        {
            Given_Code("041403");
            AssertCode(     // dbeq\td,0000
                "0|T--|0000(3): 2 instructions",
                "1|L--|d = d - 0x0001",
                "2|T--|if (d == 0x0000) branch 0000");
        }

        [Test]
        public void M6812Rw_oraa_predec()
        {
            Given_Code("AA2C");
            AssertCode(     // oraa\t$04,-x
                "0|L--|0000(2): 4 instructions",
                "1|L--|x = x + -4",
                "2|L--|a = a | Mem0[x:byte]",
                "3|L--|NZ = cond(a)",
                "4|L--|V = false");
        }

        [Test]
        public void M6812Rw_suba_postinc()
        {
            Given_Code("A033");
            AssertCode(     // suba\t$04,x+
                "0|L--|0000(2): 4 instructions",
                "1|L--|v4 = x",
                "2|L--|x = x + 4",
                "3|L--|a = a - Mem0[v4:byte]",
                "4|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6812Rw_eora_9_bit_offset()
        {
            Given_Code("A8E1FF");
            AssertCode(     // eora\t$FFFF,x
                "0|L--|0000(3): 3 instructions",
                "1|L--|a = a ^ Mem0[x + 0xFFFF:byte]",
                "2|L--|NZ = cond(a)",
                "3|L--|V = false");
        }

        [Test]
        public void M6812Rw_andb_accumulator_offset()
        {
            Given_Code("E4F4");
            AssertCode(     // andb\ta,sp
                "0|L--|0000(2): 3 instructions",
                "1|L--|b = b & Mem0[sp + (uint16) a:byte]",
                "2|L--|NZ = cond(b)",
                "3|L--|V = false");
        }

        [Test]
        public void M6812Rw_jmp_indirect()
        {
            Given_Code("05FF");
            AssertCode(     // jmp\t[d,pc]
                "0|T--|0000(2): 1 instructions",
                "1|T--|goto Mem0[0x0002 + d:ptr16]");
        }

        [Test]
        public void M6812Rw_asr_post()
        {
            Given_Code("6730");
            AssertCode(    // asr
                "0|L--|0000(2): 5 instructions",
                "1|L--|v3 = x",
                "2|L--|x = x + 1",
                "3|L--|v4 = Mem0[v3:byte] >> 1",
                "4|L--|Mem0[v3:byte] = v4",
                "5|L--|NZVC = cond(v4)");
        }

        [Test]
        public void M6812Rw_bclr_pre()
        {
            Given_Code("0D6002");
            AssertCode(    // bclr
                "0|L--|0000(3): 5 instructions",
                "1|L--|y = y + 1",
                "2|L--|v3 = Mem0[y:byte] & 0xFD",
                "3|L--|Mem0[y:byte] = v3",
                "4|L--|NZ = cond(v3)",
                "5|L--|V = false");
        }

        [Test]
        public void M6812Rw_bra()
        {
            Given_Code("20FE");
            AssertCode(    // bra
                "0|T--|0000(2): 1 instructions",
                "1|T--|goto 0000");
        }
    }
}
