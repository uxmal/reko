#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
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
            ab  = new ApplicationBuilder(arch, frame, new CallSite(4, 0), new Identifier("foo", -1, PrimitiveType.Word32, null), sig);
			var r = ab.Bind(ret);
			Assert.AreEqual("eax", r.ToString());
		}

		[Test]
		public void BindOutParameter()
		{
            ab = new ApplicationBuilder(arch, frame, new CallSite(4, 0), new Identifier("foo", -1, PrimitiveType.Word32, null), sig);
            var o = ab.Bind(regOut);
			Assert.AreEqual("edx", o.ToString());
		}

		[Test]
		public void BuildApplication()
		{
			Assert.IsTrue(sig.FormalArguments[3].Storage is OutArgumentStorage);
            ab = new ApplicationBuilder(arch, frame, new CallSite(4, 0), new Identifier("foo", -1, PrimitiveType.Word32, null), sig);
            var instr = ab.CreateInstruction();
			Assert.AreEqual("eax = foo(Mem0[esp + 4:word32], Mem0[esp + 8:word16], Mem0[esp + 12:byte], &edx)", instr.ToString());
		}

        [Test]
        public void BindToCallingFrame()
        {
            Procedure caller = new Procedure("caller", new Frame(PrimitiveType.Word16));
            caller.Frame.EnsureStackLocal(-4, PrimitiveType.Word32, "bindToArg04");
            caller.Frame.EnsureStackLocal(-6, PrimitiveType.Word16, "bindToArg02");

            Procedure callee = new Procedure("callee", new  Frame (PrimitiveType.Word16));
            callee.Frame.EnsureStackArgument(4, PrimitiveType.Word16);
            callee.Frame.EnsureStackArgument(4, PrimitiveType.Word32);

            throw new NotImplementedException();
            //ab = new ApplicationBuilder(arch, caller, new CallSite(6 + 2, 0), callee, callee.Signature); 
            //var id2 = id.Storage.BindFormalArgumentToFrame(arch, caller, 
            //Assert.AreEqual("bindToArg04", id2.ToString());

            //id = callee.EnsureStackArgument(2, PrimitiveType.Word16);
            //id2 = id.Storage.BindFormalArgumentToFrame(arch, caller, new CallSite(6 + 2, 0));
            //Assert.AreEqual("bindToArg02", id2.ToString());
        }
	}
}
