#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using Rhino.Mocks;
using NUnit.Framework;  
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class BlockWorkItem_X86Tests
    {
        private IntelArchitecture arch;
        private Procedure proc;
        private Block block;
        private LowLevelStatementStream stm;
        private IntelEmitter emitter;
        private IScanner scanner;
        private IRewriterHost2 host;
        private MockRepository repository;
        private IntelState state;
        private BlockWorkitem2 wi;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
        }

        private void BuildTest32(Action<IntelAssembler> m)
        {
            BuildTest(new IntelArchitecture(ProcessorMode.ProtectedFlat), new Address(0x10000), m);
        }

        private void BuildTest16(Action<IntelAssembler> m)
        {
            BuildTest(new IntelArchitecture(ProcessorMode.Real),new Address(0x0C00,0x000), m);
        }

        private class RewriterHost : IRewriterHost2
        {
            Dictionary<string, PseudoProcedure> pprocs = new Dictionary<string,PseudoProcedure>();

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
        }

        private void BuildTest(IntelArchitecture arch, Address addr, Action<IntelAssembler> m)
        {
            this.arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            proc = new Procedure("test", arch.CreateFrame());
            block = new Block(proc, "testblock");
            stm = new LowLevelStatementStream(0x1000, block);
            state = new IntelState();
            emitter = new IntelEmitter();
            var asm = new IntelAssembler(arch, arch.WordWidth, addr, emitter, new List<EntryPoint>());
            scanner = repository.Stub<IScanner>();
            host = new RewriterHost();
            using (repository.Record())
            {
                m(asm);
            }
            var image = new ProgramImage(addr, emitter.Bytes);
            var rw = arch.CreateRewriter2(new ImageReader(image, addr), proc.Frame, host);
            wi = new BlockWorkitem2(null, arch, rw, state, proc.Frame,  block);
        }


        [Test]
        public void WalkX86ServiceCall()
        {
            // Checks to see if a sequence return value (es:bx) trashes the state appropriately.
            IntelState state = new IntelState();
            state.Set(Registers.es, Constant.Word16(0));
            state.Set(Registers.bx, Constant.Word16(0));
            state.Set(Registers.ah, new Constant(PrimitiveType.Word16, 0x2F));

            BuildTest16(delegate(IntelAssembler m)
            {
                m.Int(0x21);
            });

            //BlockWorkitem2 wi;
            //stm.Instruction.Accept(wi);

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
    }
}
