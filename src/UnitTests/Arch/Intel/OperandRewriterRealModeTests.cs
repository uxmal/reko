#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.UnitTests.Arch.Intel
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

		[TestFixtureSetUp]
		public void Setup()
		{
			arch = new X86ArchitectureReal();
            var mem = new MemoryArea(Address.Ptr32(0x10000), new byte[4]);
			var prog = new Program(
                new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute)),
                arch,
                new DefaultPlatform(null, arch));
			var procAddress = Address.Ptr32(0x10000000);
            instr = new X86Instruction(Opcode.nop, PrimitiveType.Word16, PrimitiveType.Word16)
            {
                Address = procAddress,
            };

            proc = Procedure.Create(procAddress, arch.CreateFrame());
			orw = new OperandRewriter16(arch, new ExpressionEmitter(), proc.Frame, new FakeRewriterHost(prog));
            state = (X86State)arch.CreateProcessorState();
        }

		[Test]
		public void X86Orw16_RewriteSegConst()
		{
			var m = new MemoryOperand(
				PrimitiveType.Byte,
				Registers.bx,
				Constant.Int32(32));
			var e = orw.CreateMemoryAccess(instr, m, state);
			Assert.AreEqual("Mem0[ds:bx + 0x0020:byte]", e.ToString());
		}
	}
}
