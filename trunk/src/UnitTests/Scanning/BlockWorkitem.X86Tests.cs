#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Arch.Intel;
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
        private RtlStatementStream stm;
        private IntelEmitter emitter;
        private IScanner scanner;
        private RewriterHost host;
        private MockRepository repository;
        private IntelState state;
        private BlockWorkitem wi;
        private string nl = Environment.NewLine;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
        }

        private void BuildTest32(Action<IntelAssembler> m)
        {
            BuildTest(new IntelArchitecture(ProcessorMode.ProtectedFlat), new Address(0x10000), new FakePlatform(), m);
        }

        private void BuildTest16(Action<IntelAssembler> m)
        {
            var arch = new IntelArchitecture(ProcessorMode.Real);
            BuildTest(arch ,new Address(0x0C00,0x000), new MsdosPlatform(arch), m);
        }

        private class RewriterHost : IRewriterHost2
        {
            Dictionary<string, PseudoProcedure> pprocs = new Dictionary<string,PseudoProcedure>();
            Dictionary<uint, ProcedureSignature> sigs = new Dictionary<uint, ProcedureSignature>();

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
        }

        private void BuildTest(IntelArchitecture arch, Address addr, Platform platform, Action<IntelAssembler> m)
        {
            this.arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            proc = new Procedure("test", arch.CreateFrame());
            block = proc.AddBlock("testblock");
            stm = new RtlStatementStream(0x1000, block);
            state = new IntelState();
            emitter = new IntelEmitter();
            var asm = new IntelAssembler(arch, addr, emitter, new List<EntryPoint>());
            scanner = repository.Stub<IScanner>();
            host = new RewriterHost();
            using (repository.Record())
            {
                m(asm);
                scanner.Stub(x => x.Platform).Return(platform);
                scanner.Stub(x => x.Architecture).Return(arch);

            }
            var image = new ProgramImage(addr, emitter.Bytes);
            var rw = arch.CreateRewriter2(new ImageReader(image, addr), state, proc.Frame, host);
            wi = new BlockWorkitem(scanner, rw, state, proc.Frame,  block);
        }


        private Identifier Reg(IntelRegister r)
        {
            return new Identifier(r.Name, 0, r.DataType, new RegisterStorage(r));
        }

        [Test]
        public void WalkX86ServiceCall()
        {
            // Checks to see if a sequence return value (es:bx) trashes the state appropriately.
            BuildTest16(delegate(IntelAssembler m)
            {
                m.Int(0x21);

                state.Set(Registers.es, Constant.Word16(0));
                state.Set(Registers.bx, Constant.Word16(0));
                state.Set(Registers.ah, new Constant(PrimitiveType.Word16, 0x2F));
            });

            wi.Process();

            Assert.IsFalse(state.Get(Registers.es).IsValid, "should have trashed ES");
            Assert.IsFalse(state.Get(Registers.bx).IsValid, "should have trashed BX");
        }

        [Test]
        public void WalkBswap()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Bswap(m.ebp);
            });

            state.Set(Registers.ebp, new Constant(PrimitiveType.Word32, 0x12345678));
            wi.Process();
            Assert.AreSame(Constant.Invalid, state.Get(Registers.ebp));
        }

        [Test]
        public void WalkMovConst()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Mov(m.si, 0x606);
            });
            state.Set(Registers.esi, new Constant(PrimitiveType.Word32, 0x42424242));
            wi.Process();
            Assert.AreEqual(0x42420606, state.Get(Registers.esi).ToInt32());
        }

        [Test]
        public void XorWithSelf()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Xor(m.eax, m.eax);
            });
            state.Set(Registers.eax, Constant.Invalid);
            wi.Process();
            Assert.AreEqual(0, state.Get(Registers.eax).ToInt32());
        }


        [Test]
        public void SubWithSelf()
        {
            BuildTest32(delegate(IntelAssembler m)
            {
                m.Sub(m.eax, m.eax);
            });
            state.Set(Registers.eax, Constant.Invalid);
            wi.Process();
            Assert.AreEqual(0, state.Get(Registers.eax).ToInt32());
        }

        [Test]
        public void PseudoProcsShouldNukeRecipientRegister()
        {
            BuildTest16(delegate(IntelAssembler m)
            {
                m.In(m.al, m.dx);
            });
            state.Set(Registers.al, Constant.Byte(3));
            wi.Process();
            Assert.AreSame(Constant.Invalid, state.Get(Registers.al));
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
                "\tsp = sp - 0x0002" + nl + 
                "\tax = SEQ(cs, Mem0[ds:bx + 0x0004:word16])(cx)" + nl;
            Assert.AreEqual(sExp, sw.ToString());
        }


        [Test]
        public void IndirectJumpGated()
        {
            BuildTest16(delegate(IntelAssembler m)
            {
                m.And(m.bx, m.Const(3));
                m.Jmp(m.MemW(Registers.cs, Registers.bx, "table"));
                m.Label("table");
                m.Dw(0x1234);
                m.Dw(0x1236);
                m.Dw(0x123F);
                m.Dw(0x1241);
                m.Dw(0xCCCC);

                scanner.Expect(x => x.EnqueueVectorTable(
                    Arg<Address>.Is.Anything,
                    Arg<Address>.Is.Anything,
                    Arg<PrimitiveType>.Is.Same(PrimitiveType.Word16),
                    Arg<ushort>.Is.Anything,
                    Arg<bool>.Is.Equal(false),
                    Arg<Procedure>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything));
            });
            wi.Process();
            var sw = new StringWriter();
            block.WriteStatements(Console.Out);
            block.WriteStatements(sw);
            string sExp = "\tbx = bx & 0x0003" + nl +
                "\tSZO = cond(bx)" + nl +
                "\tC = false" + nl +
                "\tgoto Mem0[0x0C00:bx + 0x0008:word16]" + nl;
            Assert.AreEqual(sExp, sw.ToString());
            Assert.IsTrue(proc.ControlGraph.Nodes.Contains(block));
        }

        [Test]
        public void RepMovsw()
        {
            var follow =  new Block(proc, "follow");
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

                scanner.Expect(x => x.FindContainingBlock(
                    Arg<Address>.Matches(a => a.Offset == 0x0000))).Return(block);

            });
            wi.Process();
            Assert.IsTrue(proc.ControlGraph.ContainsEdge(block, follow));
            Assert.IsTrue(proc.ControlGraph.ContainsEdge(block, block));

        }
    }
}
