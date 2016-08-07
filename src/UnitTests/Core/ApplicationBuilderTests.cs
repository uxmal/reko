#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Analysis;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
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
		private FunctionType sig;
        private ApplicationBuilder ab;

		public ApplicationBuilderTests()
		{
			arch = new X86ArchitectureFlat32();
            frame = arch.CreateFrame();
			ret = frame.EnsureRegister(Registers.eax);
			arg04 = new Identifier("arg04",   PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32));
			arg08 = new Identifier("arg08",   PrimitiveType.Word16, new StackArgumentStorage(8, PrimitiveType.Word16));
			arg0C = new Identifier("arg0C",   PrimitiveType.Byte, new StackArgumentStorage(0x0C, PrimitiveType.Byte));
			regOut = new Identifier("edxOut", PrimitiveType.Word32, new OutArgumentStorage(frame.EnsureRegister(Registers.edx)));
            sig = new FunctionType(
                null,
                ret,
                new Identifier[] { arg04, arg08, arg0C, regOut });
        }

		[Test]
        public void AppBld_BindReturnValue()
		{
            ab  = new FrameApplicationBuilder(arch, frame, new CallSite(4, 0), new Identifier("foo", PrimitiveType.Word32, null), false);
			var r = ab.Bind(ret);
			Assert.AreEqual("eax", r.ToString());
		}

		[Test]
        public void AppBld_BindOutParameter()
		{
            ab = new FrameApplicationBuilder(arch, frame, new CallSite(4, 0), new Identifier("foo", PrimitiveType.Word32, null), false);
            var o = ab.Bind(regOut);
			Assert.AreEqual("edx", o.ToString());
		}

		[Test]
        public void AppBld_BuildApplication()
		{
			Assert.IsTrue(sig.Parameters[3].Storage is OutArgumentStorage);
            ab = new FrameApplicationBuilder(arch, frame, new CallSite(4, 0), new Identifier("foo", PrimitiveType.Word32, null), false);
            var instr = ab.CreateInstruction(sig, null);
			Assert.AreEqual("eax = foo(Mem0[esp:word32], Mem0[esp + 4:word16], Mem0[esp + 8:byte], out edx)", instr.ToString());
		}

        [Test]
        public void AppBld_BindToCallingFrame()
        {
            var caller = new Procedure("caller", new Frame(PrimitiveType.Word16));
            caller.Frame.EnsureStackLocal(-4, PrimitiveType.Word32, "bindToArg04");
            caller.Frame.EnsureStackLocal(-6, PrimitiveType.Word16, "bindToArg02");

            var callee = new Procedure("callee", new  Frame (PrimitiveType.Word16));
            var wArg = callee.Frame.EnsureStackArgument(0, PrimitiveType.Word16);
            var dwArg = callee.Frame.EnsureStackArgument(2, PrimitiveType.Word32);
            callee.Signature = new FunctionType(
                null,
                null,
                new Identifier[] { wArg, dwArg });
            var cs = new CallSite(0, 0)
            {
                StackDepthOnEntry = 6
            };
            ab = new FrameApplicationBuilder(arch, caller.Frame, cs, new ProcedureConstant(PrimitiveType.Pointer32, callee), true); 
            var instr = ab.CreateInstruction(callee.Signature, callee.Characteristics);
            Assert.AreEqual("callee(bindToArg02, bindToArg04)", instr.ToString());
        }

        [Test(Description="The byte is smaller than the target register, so we expect a 'DPB' instruction")]
        public void AppBld_BindByteToRegister()
        {
            var caller = new Procedure("caller", new Frame(PrimitiveType.Pointer32));
            var callee = new Procedure("callee", new Frame(PrimitiveType.Pointer32));
            var ab = new FrameApplicationBuilder(
                arch, 
                callee.Frame,
                new CallSite(4, 0), 
                new Identifier("foo", PrimitiveType.Pointer32, null),
                true);
            var sig = new FunctionType(null, new Identifier("bRet", PrimitiveType.Byte, Registers.eax));
            var instr = ab.CreateInstruction(sig, null);
            Assert.AreEqual("eax = DPB(eax, foo(), 0)", instr.ToString());
        }

        [Ignore("Variadic calls not implemented yet")]
        [Test(Description ="Variadic signature specified, but no way of parsing the parameters.")]
        public void AppBld_NoVariadic_Characteristics()
        {
            var caller = new Procedure("caller", new Frame(PrimitiveType.Pointer32));
            var callee = new Procedure("callee", new Frame(PrimitiveType.Pointer32));
            var ab = new FrameApplicationBuilder(
                arch, 
                caller.Frame,
                new CallSite(4, 0), 
                new ProcedureConstant(PrimitiveType.Pointer32, callee),
                true);
            var sig = ProcedureSignature.Action(new Identifier("...", new UnknownType(), new StackArgumentStorage(0, null)));
            var instr = ab.CreateInstruction(sig, null);
            Assert.AreEqual("callee(0x00000000)", instr.ToString());//$BUG: obviously wrong
        }
	}
}
