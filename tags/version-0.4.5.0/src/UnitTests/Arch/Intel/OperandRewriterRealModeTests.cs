#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Arch.Intel
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
        private IntelInstruction instr;

		[TestFixtureSetUp]
		public void Setup()
		{
			arch = new IntelArchitecture(ProcessorMode.Real);
            var image = new LoadedImage(Address.Ptr32(0x10000), new byte[4]);
			var prog = new Program(
                image,
                image.CreateImageMap(),
                arch,
                null);
			var procAddress = Address.Ptr32(0x10000000);
            instr = new IntelInstruction(Opcode.nop, PrimitiveType.Word16, PrimitiveType.Word16)
            {
                Address = procAddress,
            };

            proc = Procedure.Create(procAddress, arch.CreateFrame());
			orw = new OperandRewriter16(arch, proc.Frame, new FakeRewriterHost(prog));
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
