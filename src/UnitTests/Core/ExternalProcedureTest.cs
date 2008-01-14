/* 
 * Copyright (C) 1999-2008 John Källén.
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
using StringWriter = System.IO.StringWriter;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ExternalProcedureTest
	{
		[Test]
		public void ExtpBind()
		{
			ProcedureSignature sig = new ProcedureSignature(
				new Identifier(Registers.ax.Name, 0, PrimitiveType.Word16, new RegisterStorage(Registers.ax)),
				new Identifier [] {
					new Identifier(Registers.bx.Name, 1, PrimitiveType.Word16, new RegisterStorage(Registers.bx)),
					new Identifier(Registers.cl.Name, 2, PrimitiveType.Byte, new RegisterStorage(Registers.cl)) } );
			ExternalProcedure ep = new ExternalProcedure("foo", sig);
			Assert.AreEqual("Register word16 foo(Register word16 bx, Register byte cl)", ep.ToString());
			ProcedureConstant fn = new ProcedureConstant(PrimitiveType.Pointer, ep);
			Frame frame = new Frame(PrimitiveType.Word16);
			ApplicationBuilder ab = new ApplicationBuilder(frame);
			Instruction instr = ab.BuildApplication(new CallSite(0, 0), fn, ep.Signature);
			Assert.AreEqual("ax = foo(bx, cl)", instr.ToString());

		}
	}
}
