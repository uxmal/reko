#region License
/* 
 * Copyright (C) 1999-2016 Pavel Tomin.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using NUnit.Framework;
using System.Text;
using Reko.Arch.X86;
using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    class VarargsFormatScannerTests
    {
        private ProcedureBuilder m;
        private X86ArchitectureFlat32 x86;
        private PowerPcArchitecture32 ppc;
        private Program program;
        private ProcessorState state;
        private VarargsFormatScanner vafs;
        private ProcedureCharacteristics printfChr;
        private FunctionType x86PrintfSig;
        private FunctionType x86SprintfSig;
        private FunctionType ppcPrintfSig;

        [SetUp]
        public void Setup()
        {
            this.x86 = new X86ArchitectureFlat32();
            this.ppc = new PowerPcArchitecture32();
            this.m = new ProcedureBuilder();
            this.printfChr = new ProcedureCharacteristics()
            {
                VarargsParserClass =
                    "Reko.Core.Analysis.PrintfFormatParser,Reko.Core"
            };
            this.x86PrintfSig = new FunctionType(
                null,
                null,
                StackId(null,   4, CStringType()),
                StackId("...",  8, new UnknownType()));
            this.x86SprintfSig = new FunctionType(
                null,
                null,
                StackId(null,   4, CStringType()),
                StackId(null,   8, CStringType()),
                StackId("...", 12, new UnknownType()));
            this.ppcPrintfSig = new FunctionType(
                null,
                null,
                RegId(null,  ppc, "r3", CStringType()),
                RegId("...", ppc, "r4", new UnknownType()));
        }

        private SegmentMap CreateSegmentMap(uint uiAddr, uint size)
        {
            var mem = new MemoryArea(Address.Ptr32(uiAddr), new byte[size]);
            var seg = new ImageSegment(".data", mem, AccessMode.ReadWrite);
            return new SegmentMap(Address.Ptr32(uiAddr), seg);
        }

        private VarargsFormatScanner CreateVaScanner(Program program)
        {
            this.state = program.Architecture.CreateProcessorState();
            var frame = program.Architecture.CreateFrame();
            return new VarargsFormatScanner(program, frame, state);
        }

        private void WriteString(Program program, uint uiAddr, string str)
        {
            var imgW = program.CreateImageWriter(Address.Ptr32(uiAddr));
            imgW.WriteString(str, Encoding.ASCII);
        }

        private DataType CStringType()
        {
            return new Pointer(PrimitiveType.Char, 4);
        }

        private Identifier StackId(string name, int offset, DataType dt)
        {
            return new Identifier(
                name,
                dt,
                new StackArgumentStorage(offset, dt));
        }

        private Identifier RegId(
            string name,
            IProcessorArchitecture arch,
            string reg,
            DataType dt)
        {
            return new Identifier(
                name,
                dt,
                arch.GetRegister(reg));
        }

        private void Given_VaScanner(IProcessorArchitecture arch)
        {
            var platform = new DefaultPlatform(null, arch);
            var segmentMap = CreateSegmentMap(0, 128);
            this.program = new Program(segmentMap, arch, platform);
            this.vafs = CreateVaScanner(program);
        }

        private void Given_StackString(int stackOffset, string str)
        {
            uint uiAddr = 0x13;
            var sp = m.Register(program.Architecture.StackRegister);
            var stackAccess = m.IAdd(sp, stackOffset);
            state.SetValueEa(stackAccess, Constant.Word32(uiAddr));
            WriteString(program, uiAddr, str);
        }

        private void Given_RegString(string reg, string str)
        {
            uint uiAddr = 0x13;
            var r = program.Architecture.GetRegister(reg);
            state.SetRegister(r, Constant.Word32(uiAddr));
            WriteString(program, uiAddr, str);
        }

        [Test]
        public void Vafs_NoVarargs()
        {
            Given_VaScanner(x86);
            var emptyChr = new ProcedureCharacteristics();
            var emptySig = new FunctionType();
            Assert.IsFalse(vafs.TryScan(null, null));
            Assert.IsFalse(vafs.TryScan(emptySig, null));
            Assert.IsFalse(vafs.TryScan(null, emptyChr));
            Assert.IsFalse(vafs.TryScan(emptySig, emptyChr));
            Assert.IsFalse(vafs.TryScan(x86PrintfSig, null));
            Assert.IsFalse(vafs.TryScan(x86PrintfSig, emptyChr));
            Assert.IsFalse(vafs.TryScan(null, printfChr));
            Assert.IsFalse(vafs.TryScan(emptySig, printfChr));
        }

        [Test]
        public void Vafs_X86Printf()
        {
            Given_VaScanner(x86);
            Given_StackString(4, "%d %f");
            Assert.IsTrue(vafs.TryScan(x86PrintfSig, printfChr));
            var c = Constant.Word32(666);
            var instr = vafs.BuildInstruction(c, new CallSite(4, 0));
            Assert.AreEqual(
                "0x0000029A(Mem0[esp:(ptr char)], Mem0[esp + 4:int32], " +
                           "Mem0[esp + 8:real64])",
                instr.ToString());
        }

        [Test]
        public void Vafs_X86Sprintf()
        {
            Given_VaScanner(x86);
            Given_StackString(8, "%c");
            Assert.IsTrue(vafs.TryScan(x86SprintfSig, printfChr));
            var ep = new ExternalProcedure("sprintf", x86SprintfSig);
            var pc = new ProcedureConstant(new CodeType(), ep);
            var instr = vafs.BuildInstruction(pc, new CallSite(4, 0));
            Assert.AreEqual(
                "sprintf(Mem0[esp:(ptr char)], Mem0[esp + 4:(ptr char)], " +
                        "Mem0[esp + 8:char])",
                instr.ToString());
            var appl = (Application)((SideEffect)instr).Expression;
            var sig = ((ProcedureConstant)appl.Procedure).Procedure.Signature;
            Assert.AreEqual(
                "(fn void ((ptr char), (ptr char), char))",
                sig.ToString());
        }

        [Test]
        [Ignore("Varargs scanning has not implemented on PowerPc")]
        public void Vafs_PpcPrintf()
        {
            Given_VaScanner(ppc);
            Given_RegString("r3", "%d%d");
            Assert.IsTrue(vafs.TryScan(ppcPrintfSig, printfChr));
            var c = Constant.Word32(0x123);
            var instr = vafs.BuildInstruction(c, new CallSite(4, 0));
            Assert.AreEqual(
                "0x00000123(r3, r4, r5)",
                instr.ToString());
        }
    }
}
