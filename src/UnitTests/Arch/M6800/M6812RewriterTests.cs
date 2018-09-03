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
            var bytes = OperatingEnvironmentElement.LoadHexBytes(hexBytes)
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
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_clr()
        {
            Given_Code("6980");
            AssertCode(     // clr\t$0000,sp
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_suba_imm()
        {
            Given_Code("8042");
            AssertCode(     // suba\t#$42
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_cmpa_imm()
        {
            Given_Code("8142");
            AssertCode(     // cmpa\t#$42
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_ldab_direct()
        {
            Given_Code("F64242");
            AssertCode(     // ldab\t$4242
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_aba()
        {
            Given_Code("1806");
            AssertCode(     // aba
                "0|L--|0000(2): 2 instructions",
                "1|L--|a = a + b",
                "2|L--|NZCV = cond(a)");
        }

        [Test]
        public void M6812Rw_adca_immediate()
        {
            Given_Code("8942");
            AssertCode(     // adca\t#$42
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_adca_direct()
        {
            Given_Code("8942");
            AssertCode(     // adca\t#$42
                "0|L--|0000(1): 1 instructions",
                "1|L--|a = a + Mem0[0042:byte] + C",
                "2|L--|NZCV = cond(a)");
        }

        [Test]
        public void M6812Rw_addd_imm()
        {
            Given_Code("C31234");
            AssertCode(     // addd\t#$1234
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_bcc()
        {
            Given_Code("24FE");
            AssertCode(     // bcc\t0000
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_lbcc()
        {
            Given_Code("1824FFFC");
            AssertCode(     // lbcc\t0000
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void M6812Rw_dbeq()
        {
            Given_Code("041403");
            AssertCode(     // dbeq\td,0000
                "0|L--|0000(1): 1 instructions",
                "1|L--|@@@");
        }
    }
}
