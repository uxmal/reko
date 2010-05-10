using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
        private StringWriter writer;
        private readonly string nl = Environment.NewLine;
        
        [SetUp]
        public void Setup()
        {
            prog = new Program();
            arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            proc = new Procedure("test", new Frame(arch.FramePointerType));
            host = new FakeRewriterHost(prog);
            var prw = new FakeProcedureRewriter(arch, host, proc);
            rw = new TestRewriter(prw, proc, host, arch, new IntelRewriterState(proc.Frame));
            writer = new StringWriter();
        }

        [Test]
        public void FstswSahf()
        {
            rw.ConvertInstructions(
                new IntelInstruction(Opcode.fstsw, PrimitiveType.Word16, PrimitiveType.Word32, new RegisterOperand(Registers.ax)),
                new IntelInstruction(Opcode.sahf, PrimitiveType.Byte, PrimitiveType.Word32));
            rw.Block.WriteStatements(writer);
            Console.WriteLine(writer);
            string sExp =
                "	ax = (word16) FPUF << 8" + nl +
                "	SCZO = ah" + nl;

            Assert.AreEqual(sExp, writer.ToString());
        }

        [Test]
        public void FstswTestAhEq()
        {
            rw.ConvertInstructions(
                new IntelInstruction(Opcode.fcompp, PrimitiveType.Real64, PrimitiveType.Word32),
                new IntelInstruction(Opcode.fstsw, PrimitiveType.Word16, PrimitiveType.Word32, new RegisterOperand(Registers.ax)),
                new IntelInstruction(Opcode.test, PrimitiveType.Byte, PrimitiveType.Word32, new RegisterOperand(Registers.ah), ImmediateOperand.Byte(0x44)),
                new IntelInstruction(Opcode.jpe, PrimitiveType.Word32, PrimitiveType.Word32, ImmediateOperand.Word32(0x4711)));
            rw.Block.WriteStatements(writer);
            Console.WriteLine(writer);
            string sExp =
                "\tFPUF = cond(rArg0 - rArg1)" + nl + 
                "\tax = (word16) FPUF << 8" + nl +
                "\tSCZO = cond(ah & 0x44)" + nl +
                "\tC = false" + nl+ 
                "\tbranch Test(NE,FPUF) l00004711" + nl;
            Assert.AreEqual(sExp, writer.ToString());
        }
    }
}
