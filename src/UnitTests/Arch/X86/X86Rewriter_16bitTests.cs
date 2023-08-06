using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.X86
{
    [TestFixture]
    public class X86Rewriter_16bitTests : Arch.RewriterTestBase
    {
        private readonly IntelArchitecture arch;
        private readonly Address addrBase;

        public X86Rewriter_16bitTests()
        {
            var sc = CreateServiceContainer();
            arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            addrBase = Address.SegPtr(0x0C00, 0x0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrBase;

        [Test]
        public void X86Rw_phsubsw()
        {
            Given_HexString("0F380710");
            AssertCode(     // phsubsw  mm2,[bx+si]
                "0|L--|0C00:0000(4): 3 instructions",
                "1|L--|v7 = Mem0[ds:bx + si:word64]",
                "2|L--|mm2_v7 = SEQ(mm2, v7)",
                "3|L--|mm2 = __phsubs<int16[8],int16[4]>(mm2_v7)");
        }

    }
}
