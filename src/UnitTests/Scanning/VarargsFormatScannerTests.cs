#region License
/* 
 * Copyright (C) 1999-2017 Pavel Tomin.
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
using Reko.Arch.PowerPC;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.SysV;
using Reko.Environments.Windows;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class VarargsFormatScannerTests
    {
        private MockRepository mr;
        private ProcedureBuilder m;
        private Win32Platform win32;
        private Win_x86_64_Platform win_x86_64;
        private SysVPlatform sysV_ppc;
        private Program program;
        private ProcessorState state;
        private VarargsFormatScanner vafs;
        private ProcedureCharacteristics printfChr;
        private FunctionType x86PrintfSig;
        private FunctionType x86SprintfSig;
        private FunctionType win_x86_64PrintfSig;
        private FunctionType ppcPrintfSig;
        private ServiceContainer sc;
        private Address addrInstr;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.sc = new ServiceContainer();
            var cfg = mr.Stub<IConfigurationService>();
            var env = mr.Stub<OperatingEnvironment>();
            cfg.Stub(c => c.GetEnvironment("")).IgnoreArguments().Return(env);
            env.Stub(e => e.Architectures).Return(new List<IPlatformArchitectureElement>());
            cfg.Replay();
            env.Replay();
            sc.AddService<IConfigurationService>(cfg);
            this.win32 = new Win32Platform(sc, new X86ArchitectureFlat32());
            this.win_x86_64 = new Win_x86_64_Platform(sc, new X86ArchitectureFlat64());
            this.sysV_ppc = new SysVPlatform(sc, new PowerPcArchitecture32 { Name = "ppc32" });
            this.m = new ProcedureBuilder();
            this.printfChr = new ProcedureCharacteristics()
            {
                VarargsParserClass =
                    "Reko.Libraries.Libc.PrintfFormatParser,Reko.Libraries.Libc"
            };
            this.x86PrintfSig = new FunctionType(
                null,
                StackId(null,   4, CStringType32()),
                StackId("...",  8, new UnknownType()));
            this.x86SprintfSig = new FunctionType(
                null,
                StackId(null,   4, CStringType32()),
                StackId(null,   8, CStringType32()),
                StackId("...", 12, new UnknownType()));
            this.win_x86_64PrintfSig = new FunctionType(
                null,
                RegId(null, win_x86_64, "rcx", CStringType64()),
                RegId("...", win_x86_64, "rdx", new UnknownType()));
            this.ppcPrintfSig = new FunctionType(
                null,
                RegId(null,  sysV_ppc, "r3", CStringType32()),
                RegId("...", sysV_ppc, "r4", new UnknownType()));
            this.addrInstr = Address.Ptr32(0x123400);
            var listener = new FakeDecompilerEventListener();
            sc.AddService<DecompilerEventListener>(listener);
        }

        private SegmentMap CreateSegmentMap(uint uiAddr, uint size)
        {
            var mem = new MemoryArea(Address.Ptr32(uiAddr), new byte[size]);
            var seg = new ImageSegment(".data", mem, AccessMode.ReadWrite);
            return new SegmentMap(Address.Ptr32(uiAddr), seg);
        }

        private VarargsFormatScanner CreateVaScanner(Program program)
        {
            this.state = program.Architecture.CreateProcessorState(program.SegmentMap);
            var frame = program.Architecture.CreateFrame();
            return new VarargsFormatScanner(program, frame, state, sc);
        }

        private void WriteString32(Program program, uint uiAddr, string str)
        {
            var imgW = program.CreateImageWriter(Address.Ptr32(uiAddr));
            imgW.WriteString(str, Encoding.ASCII);
        }

        private void WriteString64(Program program, uint uiAddr, string str)
        {
            var imgW = program.CreateImageWriter(Address.Ptr64(uiAddr));
            imgW.WriteString(str, Encoding.ASCII);
        }

        private DataType CStringType32()
        {
            return new Pointer(PrimitiveType.Char, 4);
        }

        private DataType CStringType64()
        {
            return new Pointer(PrimitiveType.Char, 8);
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
            IPlatform platform,
            string reg,
            DataType dt)
        {
            return new Identifier(
                name,
                dt,
                platform.Architecture.GetRegister(reg));
        }

        private string DumpSignature(string name, FunctionType sig)
        {
            var sb = new StringBuilder();
            if (sig.HasVoidReturn)
            {
                sb.Append("void ");
            }
            else if (sig.ReturnValue != null)
            {
                sb.Append(sig.ReturnValue.Storage);
                sb.AppendFormat("{0} {1} ", sig.ReturnValue.Storage, sig.ReturnValue.DataType);
            }
            sb.AppendFormat("{0}(", name);
            var sep = "";
            foreach (var p in sig.Parameters)
            {
                sb.AppendFormat("{0}{1} {2}", sep, p.Storage, p.DataType);
                sep = ", ";
            }
            sb.AppendFormat(")");
            return sb.ToString();
        }

        private void Given_VaScanner(IPlatform platform)
        {
            var segmentMap = CreateSegmentMap(0, 128);
            this.program = new Program(segmentMap, platform.Architecture, platform);
            this.vafs = CreateVaScanner(program);
        }

        private void Given_StackString(int stackOffset, string str)
        {
            uint uiAddr = 0x13;
            var sp = m.Register(program.Architecture.StackRegister);
            var stackAccess = m.IAdd(sp, stackOffset);
            state.SetValueEa(stackAccess, Constant.Word32(uiAddr));
            WriteString32(program, uiAddr, str);
        }

        private void Given_RegString32(string reg, string str)
        {
            uint uiAddr = 0x13;
            var r = program.Architecture.GetRegister(reg);
            state.SetRegister(r, Constant.Word32(uiAddr));
            WriteString32(program, uiAddr, str);
        }

        private void Given_RegString64(string reg, string str)
        {
            uint uiAddr = 0x13;
            var r = program.Architecture.GetRegister(reg);
            state.SetRegister(r, Constant.Word64(uiAddr));
            WriteString64(program, uiAddr, str);
        }

        [Test]
        public void Vafs_NoVarargs()
        {
            Given_VaScanner(win32);
            var emptyChr = new ProcedureCharacteristics();
            var emptySig = new FunctionType();
            Assert.IsFalse(vafs.TryScan(addrInstr, null, null));
            Assert.IsFalse(vafs.TryScan(addrInstr, emptySig, null));
            Assert.IsFalse(vafs.TryScan(addrInstr, null, emptyChr));
            Assert.IsFalse(vafs.TryScan(addrInstr, emptySig, emptyChr));
            Assert.IsFalse(vafs.TryScan(addrInstr, x86PrintfSig, null));
            Assert.IsFalse(vafs.TryScan(addrInstr, x86PrintfSig, emptyChr));
            Assert.IsFalse(vafs.TryScan(addrInstr, null, printfChr));
            Assert.IsFalse(vafs.TryScan(addrInstr, emptySig, printfChr));
        }

        [Test]
        public void Vafs_X86Printf()
        {
            Given_VaScanner(win32);
            Given_StackString(4, "%d %f");
            Assert.IsTrue(vafs.TryScan(addrInstr, x86PrintfSig, printfChr));
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
            Given_VaScanner(win32);
            Given_StackString(8, "%c");
            Assert.IsTrue(vafs.TryScan(addrInstr, x86SprintfSig, printfChr));
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
        public void Vafs_X86_64Printf()
        {
            Given_VaScanner(win_x86_64);
            Given_RegString64("rcx", "%d %f %s %u %x");
            Assert.IsTrue(vafs.TryScan(addrInstr, win_x86_64PrintfSig, printfChr));
            var c = Constant.Word32(666);
            var instr = vafs.BuildInstruction(c, new CallSite(8, 0));
            Assert.AreEqual(
                "0x0000029A(rcx, rdx, xmm2, r9, Mem0[rsp + 32:uint64], Mem0[rsp + 40:uint64])",
                instr.ToString());
        }

        [Test]
        public void Vafs_PpcPrintf()
        {
            Given_VaScanner(sysV_ppc);
            Given_RegString32("r3", "%d%d");
            Assert.IsTrue(vafs.TryScan(addrInstr, ppcPrintfSig, printfChr));
            var c = Constant.Word32(0x123);
            var instr = vafs.BuildInstruction(c, new CallSite(4, 0));
            Assert.AreEqual(
                "0x00000123(r3, r4, r5)",
                instr.ToString());
        }

        [Test]
        public void Vafs_ReplaceArgs()
        {
            var platform = mr.Stub<IPlatform>();
            var cc = new X86CallingConvention(4, 4, 4, false, false);
            platform.Stub(p => p.GetCallingConvention("")).Return(cc);
            mr.ReplayAll();

            var newSig = VarargsFormatScanner.ReplaceVarargs(
                platform,
                x86PrintfSig, 
                new DataType[] { PrimitiveType.Int16, new Pointer(PrimitiveType.Char, 4) });
            System.Diagnostics.Debug.Print("{0}", DumpSignature("test", newSig));
            Assert.AreEqual(
                "void test(Stack +0004 (ptr char), Stack +0008 int16, Stack +000C (ptr char))",
                DumpSignature("test", newSig));
        }
    }
}
