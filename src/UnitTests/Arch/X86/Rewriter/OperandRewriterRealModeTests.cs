#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using Reko.Core.Machine;
using System.ComponentModel.Design;
using Reko.Core.Memory;
using System.Collections.Generic;
using Reko.Core.Loading;
using Reko.Arch.X86.Rewriter;

namespace Reko.UnitTests.Arch.X86.Rewriter
{
    /// <summary>
    /// Tests for operator rewriting when dealing with real mode.
    /// </summary>
    [TestFixture]
    public class OperandRewriterRealModeTests
    {
        private OperandRewriter orw;
        private IntelArchitecture arch;
        private X86State state;
        private Procedure proc;
        private X86Instruction instr;

        [OneTimeSetUp]
        public void Setup()
        {
            var sc = new ServiceContainer();
            arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            var mem = new ByteMemoryArea(Address.Ptr32(0x10000), new byte[4]);
            var program = new Program(
                new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute)),
                arch,
                new DefaultPlatform(sc, arch));
            var procAddress = Address.Ptr32(0x10000000);
            instr = new X86Instruction(Mnemonic.nop, InstrClass.Linear, PrimitiveType.Word16, PrimitiveType.Word16)
            {
                Address = procAddress,
            };

            proc = Procedure.Create(arch, procAddress, arch.CreateFrame());
            orw = new OperandRewriter16(arch, new ExpressionEmitter(), proc.Frame, new FakeRewriterHost(program));
            state = (X86State) arch.CreateProcessorState();
        }

        [Test]
        public void X86Orw16_RewriteSegConst()
        {
            var m = new MemoryOperand(
                PrimitiveType.Byte,
                Registers.bx,
                Constant.Int32(32));
            var e = orw.CreateMemoryAccess(instr, m);
            Assert.AreEqual("Mem0[ds:bx + 0x20<16>:byte]", e.ToString());
        }
    }
}
