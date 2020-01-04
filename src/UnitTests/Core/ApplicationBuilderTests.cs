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
			arch = new X86ArchitectureFlat32("x86-protected-32");
            frame = arch.CreateFrame();
			ret = frame.EnsureRegister(Registers.eax);
			arg04 = new Identifier("arg04",   PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32));
			arg08 = new Identifier("arg08",   PrimitiveType.Word16, new StackArgumentStorage(8, PrimitiveType.Word16));
			arg0C = new Identifier("arg0C",   PrimitiveType.Byte, new StackArgumentStorage(0x0C, PrimitiveType.Byte));
			regOut = new Identifier("edxOut", PrimitiveType.Word32, new OutArgumentStorage(frame.EnsureRegister(Registers.edx)));
            sig = FunctionType.Func(
                ret,
                new Identifier[] { arg04, arg08, arg0C, regOut });
        }

		[Test]
        public void AppBld_BindReturnValue()
		{
            ab  = arch.CreateFrameApplicationBuilder(frame, new CallSite(4, 0), new Identifier("foo", PrimitiveType.Word32, null));
			var r = ab.Bind(ret);
			Assert.AreEqual("eax", r.ToString());
		}

		[Test]
        public void AppBld_BindOutParameter()
		{
            ab = arch.CreateFrameApplicationBuilder(frame, new CallSite(4, 0), new Identifier("foo", PrimitiveType.Word32, null));
            var o = ab.Bind(regOut);
			Assert.AreEqual("edx", o.ToString());
		}

		[Test]
        public void AppBld_BuildApplication()
		{
			Assert.IsTrue(sig.Parameters[3].Storage is OutArgumentStorage);
            ab = arch.CreateFrameApplicationBuilder(frame, new CallSite(4, 0), new Identifier("foo", PrimitiveType.Word32, null));
            var instr = ab.CreateInstruction(sig, null);
			Assert.AreEqual("eax = foo(Mem0[esp:word32], Mem0[esp + 4:word16], Mem0[esp + 8:byte], out edx)", instr.ToString());
		}

        [Test]
        public void AppBld_BindToCallingFrame()
        {
            var caller = new Procedure(arch, "caller", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Word16));
            caller.Frame.EnsureStackLocal(-4, PrimitiveType.Word32, "bindToArg04");
            caller.Frame.EnsureStackLocal(-6, PrimitiveType.Word16, "bindToArg02");

            var callee = new Procedure(arch,"callee", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Word16));
            var wArg = callee.Frame.EnsureStackArgument(0, PrimitiveType.Word16);
            var dwArg = callee.Frame.EnsureStackArgument(2, PrimitiveType.Word32);
            callee.Signature = FunctionType.Action(wArg, dwArg);
            var cs = new CallSite(0, 0)
            {
                StackDepthOnEntry = 6
            };
            ab = new FrameApplicationBuilder(arch, caller.Frame, cs, new ProcedureConstant(PrimitiveType.Ptr32, callee), true);
            var instr = ab.CreateInstruction(callee.Signature, callee.Characteristics);
            Assert.AreEqual("callee(bindToArg02, bindToArg04)", instr.ToString());
        }

        [Test(Description="The byte is smaller than the target register, so we expect a 'SEQ' instruction")]
        public void AppBld_BindByteToRegister()
        {
            var callee = new Procedure(arch, "callee", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
            var ab = arch.CreateFrameApplicationBuilder(
                callee.Frame,
                new CallSite(4, 0),
                new Identifier("foo", PrimitiveType.Ptr32, null));
            var sig = FunctionType.Func(new Identifier("bRet", PrimitiveType.Byte, Registers.eax));
            var instr = ab.CreateInstruction(sig, null);
            Assert.AreEqual("eax = SEQ(0x000000, foo())", instr.ToString());
        }

        [Test(Description ="Variadic signature specified, but no way of parsing the parameters.")]
        public void AppBld_NoVariadic_Characteristics()
        {
            var caller = new Procedure(arch, "caller", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
            var callee = new Procedure(arch, "callee", Address.Ptr32(0x00123500), new Frame(PrimitiveType.Ptr32));
            var ab = arch.CreateFrameApplicationBuilder(
                caller.Frame,
                new CallSite(4, 0),
                new ProcedureConstant(PrimitiveType.Ptr32, callee));
            var unk = new UnknownType();
            var sig = FunctionType.Action(new Identifier("...", unk, new StackArgumentStorage(0, unk)));
            var instr = ab.CreateInstruction(sig, null);
            Assert.AreEqual("callee(0x00000000)", instr.ToString());//$BUG: obviously wrong
        }

        [Test(Description = "Calling convention returns values in a reserved slot on the stack.")]
        public void AppBld_BindStackReturnValue()
        {
            var caller = new Procedure(arch, "caller", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
            var rand = new Procedure(arch, "rand", Address.Ptr32(0x00123500), new Frame(PrimitiveType.Ptr32));
            var ab = arch.CreateFrameApplicationBuilder(
                caller.Frame,
                new CallSite(4, 0),
                new ProcedureConstant(PrimitiveType.Ptr32, rand));

            var sig = FunctionType.Func(new Identifier("", PrimitiveType.Int32, new StackArgumentStorage(4, PrimitiveType.Int32)));
            var instr = ab.CreateInstruction(sig, null);
            Assert.AreEqual("Mem0[esp:int32] = rand()", instr.ToString());
        }

        [Test(Description = "Calling convention returns values in a reserved slot on the stack.")]
        public void AppBld_BindStackReturnValue_WithArgs()
        {
            var caller = new Procedure(arch, "caller", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
            var fputs = new Procedure(arch, "fputs", Address.Ptr32(0x00123500), new Frame(PrimitiveType.Ptr32));
            var ab = arch.CreateFrameApplicationBuilder(
                caller.Frame,
                new CallSite(4, 0),
                new ProcedureConstant(PrimitiveType.Ptr32, fputs));
            var sig = FunctionType.Func(
                    new Identifier("", PrimitiveType.Int32, new StackArgumentStorage(12, PrimitiveType.Int32)),
                    new Identifier("str", PrimitiveType.Ptr32, new StackArgumentStorage(8, PrimitiveType.Int32)),
                    new Identifier("stm", PrimitiveType.Ptr32, new StackArgumentStorage(4, PrimitiveType.Int32)));
            var instr = ab.CreateInstruction(sig, null);
            Assert.AreEqual("Mem0[esp + 8:int32] = fputs(Mem0[esp + 4:int32], Mem0[esp:int32])", instr.ToString());
        }
    }
}
