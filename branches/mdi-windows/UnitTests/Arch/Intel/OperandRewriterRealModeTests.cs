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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
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
		private IntelRewriterState state;
		private Procedure proc;

		[TestFixtureSetUp]
		public void Setup()
		{
			arch = new IntelArchitecture(ProcessorMode.Real);
			Program prog = new Program();
			prog.Image = new ProgramImage(new Address(0x10000), new byte[4]);
			Address procAddress = new Address(0x10000000);
            proc = Procedure.Create(null, procAddress, arch.CreateFrame());
			orw = new OperandRewriter(new FakeRewriterHost(prog), arch, proc.Frame);
			state = new IntelRewriterState(proc.Frame);
		}

		[Test]
		public void RewriteSegConst()
		{
			MemoryOperand m = new MemoryOperand(
				PrimitiveType.Byte,
				Registers.bx,
				new Constant(PrimitiveType.Int32, 32));
			Expression e = orw.CreateMemoryAccess(m, state);
			Assert.AreEqual("Mem0[ds:bx + 0x0020:byte]", e.ToString());
		}
	}
}
