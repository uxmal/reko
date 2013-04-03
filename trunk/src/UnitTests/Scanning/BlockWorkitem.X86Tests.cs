#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Environments.Msdos;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using Rhino.Mocks;
using NUnit.Framework;  
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class BlockWorkItem_X86Tests
    {
        private IntelArchitecture arch;
        private Procedure proc;
        private Block block;
        private IScanner scanner;
        private RewriterHost host;
        private MockRepository repository;
        private ProcessorState state;
        private BlockWorkitem wi;
        private ProgramImage image;
        private string nl = Environment.NewLine;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
        }

        private void BuildTest32(Action<IntelAssembler> m)
        {
            BuildTest(new IntelArchitecture(ProcessorMode.Protected32), new Address(0x10000), new FakePlatform(), m);
        }

        private void BuildTest16(Action<IntelAssembler> m)
        {
            var arch = new IntelArchitecture(ProcessorMode.Real);
            BuildTest(arch, new Address(0x0C00, 0x000), new MsdosPlatform(arch), m);
        }

        private class RewriterHost : IRewriterHost
        {
            Dictionary<string, PseudoProcedure> pprocs = new Dictionary<string, PseudoProcedure>();
            Dictionary<uint, ProcedureSignature> sigs = new Dictionary<uint, ProcedureSignature>();
            Dictionary<uint, PseudoProcedure> importThunks;

            public RewriterHost(Dictionary<uint, PseudoProcedure> importThunks)
            {
                this.importThunks = importThunks;
            }

            public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
            {
                PseudoProcedure p;
                if (!pprocs.TryGetValue(name, out p))
                {
                    p = new PseudoProcedure(name, returnType, arity);
                    pprocs.Add(name, p);
                }
                return p;
            }

            public void SetCallSignatureAdAddress(Address addrCallInstruction, ProcedureSignature signature)
            {
                sigs.Add(addrCallInstruction.Linear, signature);
            }


            public PseudoProcedure GetImportThunkAtAddress(uint addrThunk)
            {
                PseudoProcedure p;
                if (importThunks.TryGetValue(addrThunk, out p))
                    return p;
                else
                    return null;
            }
        }

        private void BuildTest(IntelArchitecture arch, Address addr, Platform platform, Action<IntelAssembler> m)
        {
            this.arch = new IntelArchitecture(ProcessorMode.Protected32);
            proc = new Procedure("test", arch.CreateFrame());
            block = proc.AddBlock("testblock");
            this.state = arch.CreateProcessorState();
            var asm = new IntelAssembler(arch, addr, new List<EntryPoint>());
            scanner = repository.Stub<IScanner>();
            host = new RewriterHost(asm.ImportThunks);
            using (repository.Record())
            {
                m(asm);
                scanner.Stub(x => x.Platform).Return(platform);
                scanner.Stub(x => x.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Is.Anything)).Return(block);
            }
            image = asm.GetImage();
            var rw = arch.CreateRewriter(new ImageReader(image, addr), this.state, proc.Frame, host);
            wi = new BlockWorkitem(scanner, rw, state, proc.Frame, addr);
        }


        private Identifier Reg(IntelRegister r)
        {
            return new Identifier(r.Name, 0, r.DataType, r);
        }

        [Test]
        public void WalkX86ServiceCall()
        {
            // Checks to see if a sequence return value (es:bx) trashes the state appropriately.
            BuildTest16(delegate(IntelAssembler m)
            {
                m.Int(0x21);

                state.SetRegister(Registers.es, Constant.Word16(0));
                state.SetRegister(Registers.bx, Constant.Word16(0));
                state.SetRegister(Registers.ah, new Constant(PrimitiveType.Word16, 0x2F));
            });

            wi.Process();

            Assert.IsFalse(state.GetRegister(Registers.es).IsValid, "should have trashed ES");
            Assert.IsFalse(state.GetRegister(Registers.bx).IsValid, "should have trashed BX");
        }

        [Test]
        public void WalkBswap()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Bswap(m.ebp);
            });

            state.SetRegister(Registers.ebp, new Constant(PrimitiveType.Word32, 0x12345678));
            wi.Process();
            Assert.AreSame(Constant.Invalid, state.GetRegister(Registers.ebp));
        }

        [Test]
        public void WalkMovConst()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Mov(m.si, 0x606);
            });
            state.SetRegister(Registers.esi, new Constant(PrimitiveType.Word32, 0x42424242));
            wi.Process();
            Assert.AreEqual(0x42420606, state.GetRegister(Registers.esi).ToInt32());
        }

        [Test]
        public void XorWithSelf()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Xor(m.eax, m.eax);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x00010000))).Return(block);
            });
            state.SetRegister(Registers.eax, Constant.Invalid);
            wi.Process();
            Assert.AreEqual(0, state.GetRegister(Registers.eax).ToInt32());
        }


        [Test]
        public void SubWithSelf()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Sub(m.eax, m.eax);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x00010000))).Return(block);
            });
            state.SetRegister(Registers.eax, Constant.Invalid);
            wi.Process();
            Assert.AreEqual(0, state.GetRegister(Registers.eax).ToInt32());
        }

        [Test]
        public void PseudoProcsShouldNukeRecipientRegister()
        {
            BuildTest16(delegate(IntelAssembler m)
            {
                m.In(m.al, m.dx);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Is.Anything)).Return(block);
            });
            state.SetRegister(Registers.al, Constant.Byte(3));
            wi.Process();
            Assert.AreSame(Constant.Invalid, state.GetRegister(Registers.al));
        }

        [Test]
        public void RewriteIndirectCall()
        {
            var addr = new Address(0xC00, 0x0000);
            BuildTest16(delegate(IntelAssembler m)
            {
                scanner.Stub(x => x.GetCallSignatureAtAddress(Arg<Address>.Is.Anything)).Return(
                    new ProcedureSignature(
                        Reg(Registers.ax),
                        new Identifier[] { Reg(Registers.cx) }));

                m.Call(m.MemW(Registers.cs, Registers.bx, 4));
            });
            wi.Process();
            var sw = new StringWriter();
            block.WriteStatements(Console.Out);
            block.WriteStatements(sw);
            string sExp =
                "\tax = SEQ(cs, Mem0[ds:bx + 0x0004:word16])(cx)" + nl;
            Assert.AreEqual(sExp, sw.ToString());
        }


        [Test]
        //$TODO: big-endian version of this, please.
        public void IndirectJumpGated()
        {
            BuildTest16(delegate(IntelAssembler m)
            {
                m.And(m.bx, m.Const(3));
                m.Add(m.bx, m.bx);
                m.Jmp(m.MemW(Registers.cs, Registers.bx, "table"));
                m.Label("table");

                scanner.Expect(x => x.EnqueueVectorTable(
                    Arg<Address>.Is.Anything,
                    Arg<Address>.Is.Anything,
                    Arg<PrimitiveType>.Is.Same(PrimitiveType.Word16),
                    Arg<ushort>.Is.Anything,
                    Arg<bool>.Is.Equal(false),
                    Arg<Procedure>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything));
                scanner.Stub(x => x.TerminateBlock(
                    Arg<Block>.Is.Anything,
                    Arg<Address>.Is.Anything));
                scanner.Expect(x => x.CreateReader(
                    Arg<Address>.Is.Anything)).Return(new ImageReader(new byte[] {
                        0x34, 0x12,
                        0x36, 0x12,
                        0x38, 0x12,
                        0x3A, 0x12,
                        0xCC, 0xCC},
                        0));
                ExpectJumpTarget(0x0C00, 0x0000, "l0C00_0000");
                var block1234 = ExpectJumpTarget(0x0C00, 0x1234, "foo1");
                var block1236 = ExpectJumpTarget(0x0C00, 0x1236, "foo2");
                var block1238 = ExpectJumpTarget(0x0C00, 0x1238, "foo3");
                var block123A = ExpectJumpTarget(0x0C00, 0x123A, "foo4");
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x0000))).Return(block);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x0003))).Return(block);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x0005))).Return(block);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x1234))).Return(block1234);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x1236))).Return(block1236);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x1238))).Return(block1238);
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Matches(addr => addr.Offset == 0x123A))).Return(block123A);
                scanner.Stub(x => x.Image).Return(new ProgramImage(new Address(0xc00, 0), new byte[100]));
                
            });

            wi.Process();
            var sw = new StringWriter();
            block.WriteStatements(Console.Out);
            block.WriteStatements(sw);
            string sExp = "\tbx = bx & 0x0003" + nl +
                "\tSZO = cond(bx)" + nl +
                "\tC = false" + nl +
                "\tbx = bx + bx" + nl + 
                "\tSCZO = cond(bx)" + nl +
                "\tswitch (bx) { foo1 foo2 foo3 foo4 }" + nl;
            Assert.AreEqual(sExp, sw.ToString());
            Assert.IsTrue(proc.ControlGraph.Blocks.Contains(block));
        }


        private Block ExpectJumpTarget(ushort selector, ushort offset, string blockLabel)
        {
            var block = new Block(proc, blockLabel);
            scanner.Expect(x => x.EnqueueJumpTarget(
                Arg<Address>.Matches(q => (Niz(q, selector, offset))),
                Arg<Procedure>.Is.Anything,
                Arg<ProcessorState>.Is.Anything)).Return(block);
            return block;
        }

        private bool Niz(Address q, ushort selector, ushort offset)
        {
            return q.Selector == selector && q.Offset == offset;
        }

        [Test]
        public void RepMovsw()
        {
            var follow = new Block(proc, "follow");
            BuildTest16(delegate(IntelAssembler m)
            {
                m.Rep();
                m.Movsw();
                m.Mov(m.bx, m.dx);

                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(a => a.Offset == 2),
                    Arg<Procedure>.Is.Same(proc),
                    Arg<ProcessorState>.Is.Anything)).Return(follow);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(a => a.Offset == 2),
                    Arg<Procedure>.Is.Same(proc),
                    Arg<ProcessorState>.Is.Anything)).Return(block);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(a => a.Offset == 0),
                    Arg<Procedure>.Is.Same(proc),
                    Arg<ProcessorState>.Is.Anything)).Return(block);
                scanner.Expect(x => x.TerminateBlock(
                    Arg<Block>.Is.Anything,
                    Arg<Address>.Is.Anything));

            });
            follow.Procedure = proc;
            wi.Process();
            Assert.IsTrue(proc.ControlGraph.ContainsEdge(block, follow), "follow should follow block");
            Assert.IsTrue(proc.ControlGraph.ContainsEdge(block, block), "block should loop back onto itself");
        }

        [Test]
        public void XorFlags()
        {
            BuildTest16(delegate(IntelAssembler m)
            {
                m.Xor(m.esi, m.esi);
                m.Label("x");
                m.Inc(m.esi);
                m.Jmp("x");

                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(a => a.Offset == 0x0003),
                    Arg<Procedure>.Is.Same(proc),
                    Arg<ProcessorState>.Is.Anything)).Return(new Block(proc, "l0003"));
                scanner.Expect(x => x.TerminateBlock(
                    Arg<Block>.Is.Anything,
                    Arg<Address>.Is.Anything));
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Is.Anything)).Return(block);
            });
            wi.Process();
            var sExp =
                "testblock:" + nl +
                "\tesi = esi ^ esi" + nl +
                "\tSZO = cond(esi)" + nl +
                "\tC = false" + nl +
                "\tesi = esi + 0x00000001" + nl +
                "\tSZO = cond(esi)" + nl;
            var sw = new StringWriter();
            block.Write(sw);
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void IndirectCallToConstant()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Mov(m.ebx, m.MemDw("_GetDC"));
                m.Call(m.ebx);
                m.Ret();

                m.Import("_GetDC", "GetDC", "user32.dll");

                scanner.Stub(x => x.GetCallSignatureAtAddress(Arg<Address>.Is.Anything)).Return(null);
                scanner.Stub(x => x.TerminateBlock(Arg<Block>.Is.Anything, Arg<Address>.Is.Anything));
                scanner.Stub(x => x.FindContainingBlock(Arg<Address>.Is.Anything)).Return(block);
            });
            wi.Process();
            repository.VerifyAll();
            var sExp =
                "testblock:" + nl +
                "\tebx = GetDC" + nl +
                "\teax = GetDC(Mem0[esp:word32])" + nl +
                "\treturn" + nl;
            var sw = new StringWriter();
            block.Write(sw);
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(sExp, sw.ToString());
        }
    }
}
