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
using Decompiler.Analysis;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ApplicationBuilderTests
	{
		private IntelArchitecture arch;
        private Frame frame;
		private Identifier ret;
		private Identifier arg04;
		private Identifier arg08;
		private Identifier arg0C;
		private Identifier regOut;
		private ProcedureSignature sig;
        private ApplicationBuilder ab;

		public ApplicationBuilderTests()
		{
			arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            frame = arch.CreateFrame();
			ret = frame.EnsureRegister(Registers.eax);
			arg04 = new Identifier("arg04", -1, PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32));
			arg08 = new Identifier("arg08", -1, PrimitiveType.Word16, new StackArgumentStorage(8, PrimitiveType.Word16));
			arg0C = new Identifier("arg0C", -1, PrimitiveType.Byte, new StackArgumentStorage(0x0C, PrimitiveType.Byte));
			regOut = new Identifier("edxOut", -1, PrimitiveType.Word32, new OutArgumentStorage(frame.EnsureRegister(Registers.edx)));
			sig = new ProcedureSignature(ret,
				new Identifier[] { arg04, arg08, arg0C, regOut });
		
        }

		[Test]
		public void BindReturnValue()
		{
            ab  = new ApplicationBuilder(frame, new CallSite(16, 0), new Identifier("foo", -1, PrimitiveType.Word32, null), sig);
			Identifier r = ab.Bind(ret, new CallSite(0, 0));
			Assert.AreEqual("eax", r.ToString());
		}

		[Test]
		public void BindOutParameter()
		{
            ab = new ApplicationBuilder(frame, new CallSite(16, 0), new Identifier("foo", -1, PrimitiveType.Word32, null), sig);
            Identifier o = ab.Bind(regOut, new CallSite(0, 0));
			Assert.AreEqual("edx", o.ToString());
		}

		[Test]
		public void BuildApplication()
		{
			Assert.IsTrue(sig.FormalArguments[3].Storage is OutArgumentStorage);
            ab  = new ApplicationBuilder(frame, new CallSite(16, 0), new Identifier("foo", -1, PrimitiveType.Word32, null), sig);
            Instruction instr = ab.CreateInstruction();
			Assert.AreEqual("eax = foo(dwLoc0C, wLoc08, bLoc04, &edx)", instr.ToString());
		}
	}
}
