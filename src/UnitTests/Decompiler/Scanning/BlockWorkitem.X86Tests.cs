#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using Reko.Scanning;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class BlockWorkItem_X86Tests
    {
        private Procedure proc;
        private Block block;
        private Mock<IScannerServices> scanner;
        private RewriterHost host;
        private ProcessorState state;
        private BlockWorkitem wi;
        private Program lr;
        private string nl = Environment.NewLine;
        private ServiceContainer sc;
        private Program program;

        [SetUp]
        public void Setup()
        {
            var cfgSvc = new Mock<IConfigurationService>();
            var env = new Mock<PlatformDefinition>();
            var tlSvc = new Mock<ITypeLibraryLoaderService>();
            var eventListener = new Mock<IDecompilerEventListener>();
            cfgSvc.Setup(c => c.GetEnvironment("ms-dos")).Returns(env.Object);
            env.Setup(c => c.TypeLibraries).Returns(new List<TypeLibraryDefinition>());
            env.Setup(c => c.CharacteristicsLibraries).Returns(new List<TypeLibraryDefinition>());
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemService());
            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<ITypeLibraryLoaderService>(tlSvc.Object);
            sc.AddService<IEventListener>(eventListener.Object);
            sc.AddService<IDecompilerEventListener>(eventListener.Object);
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
        }

        private void BuildTest32(Action<X86Assembler> m)
        {
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", []);
            BuildTest(arch, Address.Ptr32(0x10000), new FakePlatform(sc, arch), m);
        }

        private void BuildTest16(Action<X86Assembler> m)
        {
            var arch = new X86ArchitectureReal(sc, "x86-real-16", []);
            BuildTest(arch, Address.SegPtr(0x0C00, 0x000), new MsdosPlatform(sc, arch), m);
        }

        private class RewriterHost : IRewriterHost, IDynamicLinker
        {
            Dictionary<string, IntrinsicProcedure> pprocs = new Dictionary<string, IntrinsicProcedure>();
            Dictionary<ulong, FunctionType> sigs = new Dictionary<ulong, FunctionType>();
            Dictionary<Address, ImportReference> importThunks;
            Dictionary<string, FunctionType> signatures;
            Dictionary<string, DataType> globals;

            public RewriterHost(
                Dictionary<Address, ImportReference> importThunks,
                Dictionary<string, FunctionType> signatures,
                Dictionary<string, DataType> globals)
            {
                this.importThunks = importThunks;
                this.signatures = signatures;
                this.globals = globals;
            }

            public Constant GlobalRegisterValue => null;

            public void BwiX86_SetCallSignatureAdAddress(Address addrCallInstruction, FunctionType signature)
            {
                sigs.Add(addrCallInstruction.ToLinear(), signature);
            }

            public IProcessorArchitecture GetArchitecture(string archLabel)
            {
                throw new NotImplementedException();
            }

            public Expression GetImport(Address addrThunk, Address addrInstr)
            {
                return null;
            }

            public ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr)
            {
                if (importThunks.TryGetValue(addrThunk, out ImportReference p))
                    return p.ResolveImportedProcedure(this, null, new ProgramAddress(null, addrInstr), default);
                else
                    return null;
            }

            public ExternalProcedure ResolveProcedure(string moduleName, string importName, IPlatform platform)
            {
                if (signatures.TryGetValue(importName, out FunctionType sig))
                    return new ExternalProcedure(importName, sig);
                else
                    return null;
            }

            public ExternalProcedure ResolveProcedure(string moduleName, int ordinal, IPlatform platform)
            {
                throw new NotImplementedException();
            }

            public Expression ResolveImport(string moduleName, string globalName, IPlatform platform)
            {
                throw new NotImplementedException();
            }

            public Expression ResolveImport(string moduleName, int ordinal, IPlatform platform)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
            {
                throw new NotImplementedException();
            }

            public Expression ResolveToImportedValue(Statement stm, Constant c)
            {
                throw new NotImplementedException();
            }

            public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, out Constant value)
            {
                throw new NotImplementedException();
            }

            public void Error(Address address, string format, params object[] args)
            {
                throw new NotImplementedException();
            }

            public void Warn(Address address, string format, params object[] args)
            {
                throw new NotImplementedException();
            }
        }

        private void BuildTest(IntelArchitecture arch, Address addr, IPlatform platform, Action<X86Assembler> m)
        {
            proc = new Procedure(arch, "test", addr, arch.CreateFrame());
            block = proc.AddBlock(addr, "testblock");
            var asm = new X86Assembler(arch, addr, []);
            scanner = new Mock<IScannerServices>();
            scanner.Setup(s => s.Services).Returns(sc);
            this.state = arch.CreateProcessorState();
            m(asm);
            lr = asm.GetImage();
            host = new RewriterHost(
                asm.ImportReferences,
                new Dictionary<string, FunctionType>
                {
                    {
                        "GetDC",
                        new FunctionType(
                            [
                                new Identifier("arg",
                                    new TypeReference(
                                        "HWND",
                                        new Pointer(VoidType.Instance, 32)),
                                    new StackStorage(4, new TypeReference(
                                        "HWND",
                                        new Pointer(VoidType.Instance, 32))))
                            ],
                            [ new Identifier("", new Pointer(VoidType.Instance, 32), RegisterStorage.Reg32("eax", 0)) ]
                            )
                        {
                            StackDelta = 4,
                        }
                    }
               },
               new Dictionary<string, DataType>());
            var rw = arch.CreateRewriter(
                lr.SegmentMap.Segments.Values.First().MemoryArea.CreateLeReader(addr),
                this.state,
                proc.Frame,
                host);
            this.program = new Program
            {
                Architecture = arch,
                Memory = new ByteProgramMemory(lr.SegmentMap),
                SegmentMap = lr.SegmentMap,
                ImageMap = lr.ImageMap,
                Platform = platform,
            };

            scanner.Setup(x => x.FindContainingBlock(It.IsAny<Address>())).Returns(block);
            scanner.Setup(x => x.GetTrace(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<ProcessorState>(),
                It.IsAny<IStorageBinder>())).Returns(rw);
            scanner.Setup(x => x.Services).Returns(sc);

            wi = new BlockWorkitem(scanner.Object, program, arch, state, addr);
        }


        private Identifier Reg(RegisterStorage r)
        {
            return new Identifier(r.Name, r.DataType, r);
        }

        [Test]
        public void BwiX86_WalkX86ServiceCall()
        {
            // Checks to see if a sequence return value (es:bx) trashes the state appropriately.
            BuildTest16(m =>
            {
                m.Int(0x21);

                state.SetRegister(Registers.es, Constant.Word16(0));
                state.SetRegister(Registers.bx, Constant.Word16(0));
                state.SetRegister(Registers.ah, Constant.Word16(0x2F));
            });

            wi.Process();

            Assert.IsFalse(state.GetRegister(Registers.es).IsValid, "should have trashed ES");
            Assert.IsFalse(state.GetRegister(Registers.bx).IsValid, "should have trashed BX");
        }

        [Test]
        public void BwiX86_WalkBswap()
        {
            BuildTest32(m =>
            {
                m.Bswap(m.ebp);
            });

            state.SetRegister(Registers.ebp, Constant.Word32(0x12345678));
            wi.Process();
            Assert.IsTrue(state.GetRegister(Registers.ebp) is InvalidConstant);
        }

        [Test]
        public void BwiX86_WalkMovConst()
        {
            BuildTest32(m =>
            {
                m.Mov(m.si, 0x606);
            });
            state.SetRegister(Registers.esi, Constant.Word32(0x42424242));
            wi.Process();
            Assert.AreEqual(0x42420606, state.GetRegister(Registers.esi).ToInt32());
        }

        [Test]
        public void BwiX86_XorWithSelf()
        {
            BuildTest32(m =>
            {
                m.Xor(m.eax, m.eax);
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x00010000))).Returns(block);
            });
            state.SetRegister(Registers.eax, InvalidConstant.Create(PrimitiveType.Word32));
            wi.Process();
            Assert.AreEqual(0, state.GetRegister(Registers.eax).ToInt32());
        }


        [Test]
        public void BwiX86_SubWithSelf()
        {
            BuildTest32(m =>
            {
                m.Sub(m.eax, m.eax);
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x00010000))).Returns(block);
            });
            state.SetRegister(Registers.eax, InvalidConstant.Create(PrimitiveType.Word32));
            wi.Process();
            Assert.AreEqual(0, state.GetRegister(Registers.eax).ToInt32());
        }

        [Test]
        public void BwiX86_IntrinsicsShouldNukeRecipientRegister()
        {
            BuildTest16(m =>
            {
                m.In(m.al, m.dx);
                scanner.Setup(x => x.FindContainingBlock(It.IsAny<Address>())).Returns(block);
            });
            state.SetRegister(Registers.al, Constant.Byte(3));
            wi.Process();
            Assert.IsTrue(state.GetRegister(Registers.al) is InvalidConstant);
        }

        [Test]
        public void BwiX86_RewriteIndirectCall()
        {
            BuildTest16(m =>
            {
                m.Call(m.MemW(Registers.cs, Registers.bx, 4));
            });
            var uc = new UserCallData
            {
                Address = Address.SegPtr(0xC00, 0),
                Signature = new FunctionType(
                        [ Reg(Registers.cx) ],
                        [ Reg(Registers.ax) ])
            };
            program.User.Calls.Add(uc.Address, uc);

            wi.Process();
            var sw = new StringWriter();
            block.WriteStatements(sw);
            string sExp =
                "\tax = 0xC00<16>:Mem0[ds:bx + 4<i16>:word16](cx)" + nl;
            Assert.AreEqual(sExp, sw.ToString());
        }


        [Test]
        //$TODO: big-endian version of this, please.
        public void BwiX86_IndirectJumpGated()
        {
            BuildTest16(m =>
            {
                block.Address = Address.SegPtr(0x0C00, 0x0123);

                m.And(m.bx, m.Const(3));
                m.Add(m.bx, m.bx);
                m.Jmp(m.MemW(Registers.cs, Registers.bx, "table"));

                m.Label("table");
                m.Dw(0x1234);
                m.Dw(0x0C00);
                m.Repeat(30, mm => mm.Dw(0xC3));

                scanner.Setup(x => x.TerminateBlock(
                    It.IsAny<Block>(),
                    It.IsAny<Address>()));
           
                var block1234 = ExpectJumpTarget(0x0C00, 0x0034, "foo1");
                var block1236 = ExpectJumpTarget(0x0C00, 0x0036, "foo2");
                var block1238 = ExpectJumpTarget(0x0C00, 0x0038, "foo3");
                var block123A = ExpectJumpTarget(0x0C00, 0x003A, "foo4");
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x0000))).Returns(block);
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x0003))).Returns(block);
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x0005))).Returns(block);
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x0034))).Returns(block1234);
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x0036))).Returns(block1236);
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x0038))).Returns(block1238);
                scanner.Setup(x => x.FindContainingBlock(It.Is<Address>(addr => addr.Offset == 0x003A))).Returns(block123A);
            });

            var bmem = (ByteMemoryArea) this.program.SegmentMap.Segments.Values.First().MemoryArea;
            bmem.WriteBytes(
                new byte[] {
                    0x34, 0x00,
                    0x36, 0x00,
                    0x38, 0x00,
                    0x3A, 0x00,
                    0xCC, 0xCC
                },
                0x000A, 0x000A);

            wi.Process();
            var sw = new StringWriter();
            block.WriteStatements(sw);
            string sExp =
                "\tv9 = bx" + nl +
                "\tbx = bx & 3<16>" + nl +
                "\tSZ = cond(bx)" + nl +
                "\tO = 0<32>" + nl +
                "\tC = 0<32>" + nl +
                "\tbx = bx + bx" + nl + 
                "\tSCZO = cond(bx)" + nl +
                "\tswitch (v9) { foo1 foo2 foo3 foo4 }" + nl;
            Assert.AreEqual(sExp, sw.ToString());
            Assert.IsTrue(proc.ControlGraph.Blocks.Contains(block));
            scanner.Verify();
        }

        private Block ExpectJumpTarget(ushort selector, ushort offset, string blockLabel)
        {
            var addr = Address.SegPtr(selector, offset);
            var block = new Block(proc, Address.SegPtr(selector, offset), blockLabel);
            scanner.Setup(s => s.EnqueueJumpTarget(
                It.IsAny<Address>(),
                addr,
                It.IsAny<Procedure>(),
                It.IsAny<ProcessorState>()))
                .Returns(block)
                .Verifiable();
            return block;
        }

        [Test]
        public void BwiX86_RepMovsw()
        {
            var follow = new Block(proc, Address.SegPtr(0x0C00, 0), "follow"); // the code that follows the 'rep movsw'
            BuildTest16(m =>
            {
                m.Std();
                m.Rep();
                m.Movsw();
                m.Mov(m.bx, m.dx);

                scanner.Setup(f => f.GetImportedProcedure(
                    It.IsAny<IProcessorArchitecture>(),
                    It.IsAny<Address>(),
                    It.IsAny<Address>())).Returns((ExternalProcedure)null);

                scanner.SetupSequence(x => x.EnqueueJumpTarget(
                    It.IsNotNull<Address>(),
                    It.Is<Address>(a => a.Offset == 0x0003),
                    proc,
                    It.IsAny<ProcessorState>()))
                        .Returns(follow)
                        .Returns(block);
                scanner.Setup(x => x.EnqueueJumpTarget(
                    It.IsNotNull<Address>(),
                    It.Is<Address>(a => a.Offset == 0x0001),
                    proc,
                    It.IsAny<ProcessorState>())).Returns(block);
                scanner.Setup(x => x.TerminateBlock(
                    It.IsAny<Block>(),
                    It.IsAny<Address>()));
                scanner.Setup(s => s.GetTrampoline(
                    It.IsAny<IProcessorArchitecture>(),
                    It.IsAny<Address>())).Returns((Procedure)null);
            });
            follow.Procedure = proc;
            wi.Process();
            Assert.AreEqual("l0C00_0001_1", block.Succ[0].DisplayName, "block should loop back onto itself");
            Assert.AreEqual("follow", block.Succ[1].DisplayName, "block should terminate if cx == 0 check is true");
        }

        [Test]
        public void BwiX86_XorFlags()
        {
            BuildTest16(m =>
            {
                m.Xor(m.esi, m.esi);
                m.Label("x");
                m.Inc(m.esi);
                m.Jmp("x");

                scanner.Setup(x => x.EnqueueJumpTarget(
                    It.IsNotNull<Address>(),
                    It.Is<Address>(a => a.Offset == 0x0003),
                    proc,
                    It.IsAny<ProcessorState>())).Returns(new Block(proc, Address.Ptr16(0x3), "l0003"));
                scanner.Setup(x => x.TerminateBlock(
                    It.IsAny<Block>(),
                    It.IsAny<Address>()));
                scanner.Setup(x => x.FindContainingBlock(It.IsAny<Address>())).Returns(block);
                scanner.Setup(f => f.GetImportedProcedure(
                    It.IsAny<IProcessorArchitecture>(),
                    It.IsAny<Address>(),
                    It.IsAny<Address>())).Returns((ExternalProcedure)null);
                scanner.Setup(s => s.GetTrampoline(
                    It.IsAny<IProcessorArchitecture>(),
                    It.IsAny<Address>())).Returns((Procedure) null);
            });
            wi.Process();
            var sExp =
                "testblock:" + nl +
                "\tesi = esi ^ esi" + nl +
                "\tSZ = cond(esi)" + nl +
                "\tO = 0<32>" + nl +
                "\tC = 0<32>" + nl + 
                "\tesi = esi + 1<32>" + nl +
                "\tSZO = cond(esi)" + nl +
                 "\tgoto 0C00:0003" + nl;
            var sw = new StringWriter();
            block.Write(sw);
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void BwiX86_IndirectCallToConstant()
        {
            BuildTest32(m =>
            {
                m.Mov(m.ebx, m.MemDw("_GetDC"));
                m.Call(m.ebx);
                m.Ret();

                m.Import("_GetDC", "GetDC", "user32.dll");

                scanner.Setup(x => x.TerminateBlock(It.IsAny<Block>(), It.IsAny<Address>()));
                scanner.Setup(x => x.FindContainingBlock(It.IsAny<Address>())).Returns(block);
                scanner.Setup(x => x.SetProcedureReturnAddressBytes(
                    It.IsNotNull<Procedure>(),
                    4,
                    It.IsAny<Address>()));
            });
            wi.Process();

            var sExp =
                "testblock:" + nl +
                "\tebx = GetDC" + nl +
                "\teax = GetDC(Mem0[esp:HWND])" + nl +
                "\tesp = esp + 4<32>" + nl +
                "\treturn" + nl;
            var sw = new StringWriter();
            block.Write(sw);
            Assert.AreEqual(sExp, sw.ToString());
        }
    }
}
