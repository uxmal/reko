#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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

using Moq;
using NUnit.Framework;
using Reko.Arch.PowerPC;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.SysV;
using Reko.Environments.Windows;
using Reko.Scanning;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class VarargsFormatScannerTests
    {
        private ProcedureBuilder m;
        private Win32Platform win32;
        private Win_x86_64_Platform win_x86_64;
        private SysVPlatform sysV_ppc;
        private Program program;
        private ProcessorState state;
        private Frame frame;
        private VarargsFormatScanner vafs;
        private ProcedureCharacteristics printfChr;
        private FunctionType x86PrintfSig;
        private FunctionType x86SprintfSig;
        private FunctionType win_x86_64PrintfSig;
        private FunctionType ppcPrintfSig;
        private ServiceContainer sc;
        private Address addrInstr;
        private FakeDecompilerEventListener listener;
        private ProcedureConstant dummyPc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            var cfg = new Mock<IConfigurationService>();
            var env = new Mock<PlatformDefinition>();
            cfg.Setup(c => c.GetEnvironment(It.IsAny<string>())).Returns(env.Object);
            env.Setup(e => e.Architectures).Returns(new List<PlatformArchitectureDefinition>());
            sc.AddService<IConfigurationService>(cfg.Object);
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
            this.win32 = new Win32Platform(sc, new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>()));
            this.win_x86_64 = new Win_x86_64_Platform(sc, new X86ArchitectureFlat64(sc, "x86-protected-64", new Dictionary<string, object>()));
            this.sysV_ppc = new SysVPlatform(sc, new PowerPcBe32Architecture(sc, "ppc-be-32", new Dictionary<string, object>()));
            this.m = new ProcedureBuilder();
            this.printfChr = new ProcedureCharacteristics()
            {
                VarargsParserClass =
                    "Reko.Libraries.Libc.PrintfFormatParser,Reko.Libraries.Libc"
            };
            this.x86PrintfSig = new FunctionType(
                [StackId(null, 4, CStringType32())], []) { ReturnAddressOnStack = 4, IsVariadic = true };
            this.x86SprintfSig = new FunctionType(
                [
                    StackId(null,   4, CStringType32()),
                    StackId(null,   8, CStringType32())
                ], []) { ReturnAddressOnStack = 4, IsVariadic = true };
            this.win_x86_64PrintfSig = new FunctionType(
                [ RegId(null, win_x86_64, "rcx", CStringType64())],
                []) { ReturnAddressOnStack=8, IsVariadic = true };
            this.ppcPrintfSig = new FunctionType(
                [ RegId(null,  sysV_ppc, "r3", CStringType32())],
                []) { IsVariadic = true };
            this.addrInstr = Address.Ptr32(0x123400);
            this.listener = new FakeDecompilerEventListener();
            sc.AddService<IDecompilerEventListener>(listener);
            this.dummyPc = new ProcedureConstant(PrimitiveType.Ptr32, new ExternalProcedure("dummy", x86PrintfSig));
        }

        private SegmentMap CreateSegmentMap(uint uiAddr, uint size)
        {
            var mem = new ByteMemoryArea(Address.Ptr32(uiAddr), new byte[size]);
            var seg = new ImageSegment(".data", mem, AccessMode.ReadWrite);
            return new SegmentMap(Address.Ptr32(uiAddr), seg);
        }

        private VarargsFormatScanner CreateVaScanner(Program program)
        {
            this.state = program.Architecture.CreateProcessorState();
            this.frame = program.Architecture.CreateFrame();
            return new VarargsFormatScanner(program, program.Architecture, state, sc, listener);
        }

        private void WriteString32(Program program, uint uiAddr, string str)
        {
            var imgW = program.CreateImageWriter(program.Architecture, Address.Ptr32(uiAddr));
            imgW.WriteString(str, Encoding.ASCII);
        }

        private void WriteString64(Program program, uint uiAddr, string str)
        {
            var imgW = program.CreateImageWriter(program.Architecture, Address.Ptr64(uiAddr));
            imgW.WriteString(str, Encoding.ASCII);
        }

        private DataType CStringType32()
        {
            return new Pointer(PrimitiveType.Char, 32);
        }

        private DataType CStringType64()
        {
            return new Pointer(PrimitiveType.Char, 64);
        }

        private Identifier StackId(string name, int offset, DataType dt)
        {
            return new Identifier(
                name,
                dt,
                new StackStorage(offset, dt));
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
            else if (sig.ReturnValue is not null)
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
            this.program = new Program(new ByteProgramMemory(segmentMap), platform.Architecture, platform);
            this.vafs = CreateVaScanner(program);
        }

        private ApplicationBuilder CreateApplicationBuilder(CallSite site)
        {
            return new FrameApplicationBuilder(program.Architecture, frame, site);
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
            var ab = CreateApplicationBuilder(new CallSite(4, 0));
            Assert.IsFalse(vafs.TryScan(addrInstr, dummyPc, null, null, ab, out _));
            Assert.IsFalse(vafs.TryScan(addrInstr, dummyPc, emptySig, null, ab, out _));
            Assert.IsFalse(vafs.TryScan(addrInstr, dummyPc, null, emptyChr, ab, out _));
            Assert.IsFalse(vafs.TryScan(addrInstr, dummyPc, emptySig, emptyChr, ab, out _));
            Assert.IsFalse(vafs.TryScan(addrInstr, dummyPc, x86PrintfSig, null, ab, out _));
            Assert.IsFalse(vafs.TryScan(addrInstr, dummyPc, x86PrintfSig, emptyChr, ab, out _));
            Assert.IsFalse(vafs.TryScan(addrInstr, dummyPc, null, printfChr, ab, out _));
            Assert.IsFalse(vafs.TryScan(addrInstr, dummyPc, emptySig, printfChr, ab, out _));
        }

        [Test]
        public void Vafs_X86Printf()
        {
            Given_VaScanner(win32);
            Given_StackString(4, "%d %f");
            var c = Constant.Word32(666);
            var ab = win32.Architecture.CreateFrameApplicationBuilder(frame, new CallSite(4, 0));
            Assert.IsTrue(vafs.TryScan(addrInstr, dummyPc, x86PrintfSig, printfChr, ab, out var result));
            var instr = vafs.BuildInstruction(c, x86PrintfSig, result.Signature, printfChr, ab);
            Assert.AreEqual(
                "0x29A<32>(Mem0[esp:(ptr32 char)], Mem0[esp + 4<i32>:int32], " +
                           "Mem0[esp + 8<i32>:real64])",
                instr.ToString());
        }

        [Test]
        public void Vafs_X86Sprintf()
        {
            Given_VaScanner(win32);
            Given_StackString(8, "%c");
            var ep = new ExternalProcedure("sprintf", x86SprintfSig);
            var pc = new ProcedureConstant(new CodeType(), ep);
            var ab = win32.Architecture.CreateFrameApplicationBuilder(frame, new CallSite(4, 0));
            Assert.IsTrue(vafs.TryScan(addrInstr, dummyPc, x86SprintfSig, printfChr, ab, out var result));
            var instr = vafs.BuildInstruction(pc, x86SprintfSig, result.Signature, printfChr, ab);
            Assert.AreEqual(
                "sprintf(Mem0[esp:(ptr32 char)], Mem0[esp + 4<i32>:(ptr32 char)], " +
                        "Mem0[esp + 8<i32>:char])",
                instr.ToString());
            var appl = (Application)((SideEffect)instr).Expression;
            var sig = ((ProcedureConstant)appl.Procedure).Signature;
            Assert.AreEqual(
                "(fn void ((ptr32 char), (ptr32 char), char))",
                sig.ToString());
            Assert.AreEqual(
                "(fn void ((ptr32 char), (ptr32 char)))",
                ep.Signature.ToString(), "VarargsFormatScanner mutated signature");
        }

        [Test]
        public void Vafs_X86_64Printf()
        {
            Given_VaScanner(win_x86_64);
            Given_RegString64("rcx", "%d %f %s %u %x");
            var c = Constant.Word32(666);
            var ab = win_x86_64.Architecture.CreateFrameApplicationBuilder(frame, new CallSite(8, 0));

            Assert.IsTrue(vafs.TryScan(addrInstr, dummyPc, win_x86_64PrintfSig, printfChr, ab, out var result));
            var instr = vafs.BuildInstruction(c, win_x86_64PrintfSig, result.Signature, printfChr, ab);
            Assert.AreEqual(
                "0x29A<32>(\"%d %f %s %u %x\", SLICE(rdx, int32, 0), SLICE(xmm2, real64, 0), r9, Mem0[rsp + 32<i64>:uint32], Mem0[rsp + 40<i64>:uint32])",
                instr.ToString());
        }

        [Test]
        public void Vafs_PpcPrintf()
        {
            Given_VaScanner(sysV_ppc);
            Given_RegString32("r3", "%d%d");
            var c = Constant.Word32(0x123);
            var ab = sysV_ppc.Architecture.CreateFrameApplicationBuilder(frame, new CallSite(0, 0));

            Assert.IsTrue(vafs.TryScan(addrInstr, dummyPc, ppcPrintfSig, printfChr, ab, out var result));
            var instr = vafs.BuildInstruction(c, ppcPrintfSig, result.Signature, printfChr, ab);
            Assert.AreEqual(
                "0x123<32>(\"%d%d\", r4, r5)",
                instr.ToString());
        }

        [Test]
        public void Vafs_ReplaceArgs()
        {
            var platform = new Mock<IPlatform>();
            var cc = new X86CallingConvention(4, 4, false, false);
            platform.Setup(p => p.GetCallingConvention("")).Returns(cc);

            var newSig = VarargsFormatScanner.ReplaceVarargs(
                platform.Object,
                x86PrintfSig, 
                new DataType[] { PrimitiveType.Int16, new Pointer(PrimitiveType.Char, 32) });
            System.Diagnostics.Debug.Print("{0}", DumpSignature("test", newSig));
            Assert.AreEqual(
                "void test(Stack +0004 (ptr32 char), Stack +0008 int16, Stack +000C (ptr32 char))",
                DumpSignature("test", newSig));
        }
    }
}


