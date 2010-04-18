using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.Intel
{
    [TestFixture]
    public class RewriteFpuInstructionTests
    {
        private Program prog;
        private TestRewriter rw;
        private IntelArchitecture arch;
        private FakeRewriterHost host;
        private Procedure proc;
        
        [SetUp]
        public void Setup()
        {
            prog = new Program();
            arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            proc = new Procedure("test", new Frame(arch.FramePointerType));
            host = new FakeRewriterHost(prog);
            rw = new TestRewriter(null, proc, host, arch, new IntelRewriterState(proc.Frame));
        }

        [Test]
        public void FstswSahf()
        {
            rw.ConvertInstructions(
                new IntelInstruction(Opcode.fstsw, PrimitiveType.Word16, PrimitiveType.Word32, new RegisterOperand(Registers.ax)),
                new IntelInstruction(Opcode.sahf, PrimitiveType.Byte, PrimitiveType.Word32));

        }
    }
}
