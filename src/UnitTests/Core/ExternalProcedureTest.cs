#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using StringWriter = System.IO.StringWriter;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class ExternalProcedureTest
	{
		[Test]
		public void ExtpBind()
		{
			var sig = FunctionType.Func(
				new Identifier(Registers.ax.Name, PrimitiveType.Word16, Registers.ax),
				new Identifier [] {
					new Identifier(Registers.bx.Name, PrimitiveType.Word16, Registers.bx),
					new Identifier(Registers.cl.Name, PrimitiveType.Byte, Registers.cl) } );
			var ep = new ExternalProcedure("foo", sig);
			Assert.AreEqual("Register word16 foo(Register word16 bx, Register byte cl)", ep.ToString());
			var fn = new ProcedureConstant(PrimitiveType.Ptr32, ep);
            var arch = new FakeArchitecture();
            var frame = arch.CreateFrame();
			var ab = arch.CreateFrameApplicationBuilder(frame, new CallSite(0, 0), fn);
            var instr = ab.CreateInstruction(ep.Signature, ep.Characteristics);
			Assert.AreEqual("ax = foo(bx, cl)", instr.ToString());
		}
	}
}
