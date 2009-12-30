using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.Intel
{
    [TestFixture]
    public class FstswChainMatcherTests
    {
        IntelArchitecture arch;
        CodeEmitter emitter;
        IntelAssembler asm;
        OperandRewriter orw;
        List<IntelInstruction> instrs;

        [SetUp]
        public void Setup()
        {
            arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            asm = new IntelAssembler(arch, PrimitiveType.Word32, new Address(0x10000), new IntelEmitter(), new List<EntryPoint>());
            Procedure proc = new Procedure("test", arch.CreateFrame());
            orw = new OperandRewriter(null, arch, proc.Frame);
            emitter = new CodeEmitter(arch, null, proc, proc.AddBlock("testblok"));
        }

        [Test]
        public void FailMatch()
        {
            asm.Mov(asm.ax, asm.Const(1));
            var m = GetMatcher();
            Assert.IsFalse(m.Matches(0));
        }

        [Test]
        public void MatchSahfSequence()
        {
            asm.Fstsw(asm.ax);
            asm.Sahf();
            var m = GetMatcher();
            Assert.IsTrue(m.Matches(0));
        }

        [Test]
        public void EmitSahf()
        {
            asm.Fstsw(asm.ax);
            asm.Sahf();
            var m = GetMatcher();
            m.Matches(0);
            m.Rewrite(emitter);
            Assert.AreEqual(1, emitter.Block.Statements.Count);
            Assert.AreEqual("SCZO = FPUF", emitter.Block.Statements[0].ToString());
            Assert.AreEqual(Opcode.nop, instrs[1].code);
        }
        
        [Test]
        public void MatchTestAh40()
        {
            asm.Fstsw(asm.ax);
            asm.Test(asm.ah, asm.Const(0x40));
            asm.Jz("foo");
            var m = GetMatcher();
            Assert.IsTrue(m.Matches(0));
        }

        [Test]
        public void EmitTestAh40()
        {
            asm.Fstsw(asm.ax);
            asm.Test(asm.ah, asm.Const(0x40));
            asm.Jnz("foo");
            var m = GetMatcher();
            Assert.AreEqual(Opcode.jnz, instrs[2].code);
            Assert.IsTrue(m.Matches(0));
            m.Rewrite(emitter);
            Assert.AreEqual(1, emitter.Block.Statements.Count);
            Assert.AreEqual("SCZO = FPUF", emitter.Block.Statements[0].ToString());
            Assert.AreEqual(Opcode.jz, instrs[2].code);
        }

        [Test]
        public void EmitTestAx01()
        {
            asm.Fstsw(asm.ax);
            asm.Test(asm.ax, asm.Const(0x0100));
            asm.Jz("foo");
            var m = GetMatcher();
            Assert.IsTrue(m.Matches(0));
            m.Rewrite(emitter);
            Assert.AreEqual(Opcode.jge, instrs[2].code);
        }

        private FstswChainMatcher GetMatcher()
        {
            ProgramImage image = asm.GetImage();
            IntelDisassembler dasm = new IntelDisassembler(image.CreateReader(0), PrimitiveType.Word32);
            instrs = new List<IntelInstruction>();
            while (image.IsValidAddress(dasm.Address))
            {
                instrs.Add(dasm.Disassemble());
            }
            return new FstswChainMatcher(instrs.ToArray(), orw);
        }
    }
}
