using Decompiler.Assemblers.x86;
using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Arch.Intel
{
    [TestFixture]
    public class RewriteFpuInstructionTests : Arch.RewriterTestBase
    {
        private X86ArchitectureFlat32 arch;
        private IntelAssembler asm;
        private LoaderResults asmResult;
        private Address loadAddress = new Address(0x0010000);
        private FakeRewriterHost host;

        [SetUp]
        public void Setup()
        {
            arch = new X86ArchitectureFlat32();
            asm = new IntelAssembler(arch, loadAddress, new List<EntryPoint>());
            host = new FakeRewriterHost(null);
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame)
        {
            return new X86Rewriter(arch, host, new X86State(arch), asmResult.Image.CreateReader(0), frame);
        }

        public override Address LoadAddress
        {
            get { return loadAddress; } 
        }

        private void BuildTest(Action<IntelAssembler> m)
        {
            m(asm);
            asmResult = asm.GetImage();
        }

        [Test]
        public void X86Rw_fstsw_sahf_jp()
        {
            BuildTest(m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Sahf();
                m.Jc("foo");
            });
            AssertCode(
                "0|00010000(4): 1 instructions",
                "1|L--|SCZO = FPUF",              //$TODO: P flag as well
                "2|00010004(2): 1 instructions",
                "3|T--|if (Test(ULT,C)) branch 00010000"
                );
        }

        [Test]
        public void X86Rw_fstsw_ax_40_je()
        {
            BuildTest(m =>
            {
                m.Label("foo");
                m.Fstsw(m.ax);
                m.Test(m.ah, 0x40);
                m.Jnz("foo");
            });
            AssertCode(
                "0|00010000(8): 2 instructions",
                "1|L--|SCZO = FPUF",
                "2|T--|if (Test(EQ,FPUF)) branch 00010000"
                );            
        }
    }
}
