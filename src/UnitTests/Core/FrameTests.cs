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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.IO;
using System.ComponentModel.Design;
using System.Collections.Generic;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class FrameTests
	{
        private IntelArchitecture arch;

        [SetUp]
        public void Setup()
        {
            arch = new X86ArchitectureReal(new ServiceContainer(), "x86-real-16", new Dictionary<string, object>());
        }

        private void RunTest(string sExpected, Frame frame)
        {
            var sw = new StringWriter();
            sw.WriteLine();
            frame.Write(sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

		[Test]
		public void FrRegisterTest()
		{
			IStorageBinder f = new Frame(arch, PrimitiveType.Word16);
			Identifier id0 = f.EnsureRegister(Registers.ax);
			f.EnsureRegister(Registers.bx);
			Identifier id2 = f.EnsureRegister(Registers.ax);
			Assert.AreEqual(id0, id2);
		}

		[Test]
		public void FrSequenceTest()
		{
			var f = new Frame(arch, PrimitiveType.Word16);
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier dx = f.EnsureRegister(Registers.dx);
			Identifier dxax = f.EnsureSequence(PrimitiveType.Word32, dx.Storage, ax.Storage);

            var sExpected =
            #region
                @"
// Mem0:Mem
// fp:fp
// %continuation:%continuation
// ax:ax
// dx:dx
// dx_ax:Sequence dx:ax
// return address size: 0
";
                #endregion
            RunTest(sExpected, f);

			Identifier dxax2 = f.EnsureSequence(PrimitiveType.Word32, dx.Storage, ax.Storage);
			Assert.IsTrue(dxax2 == dxax);
		}

        [Test]
        public void FrGrfTest()
        {
            var arch = new X86ArchitectureReal(new ServiceContainer(), "x86-real-16", new Dictionary<string, object>());
            var f = new Frame(arch, PrimitiveType.Word16);
            f.EnsureFlagGroup(arch.GetFlagGroup("SZ"));
            var sExpected =
                #region
                @"
// Mem0:Mem
// fp:fp
// %continuation:%continuation
// SZ:SZ
// return address size: 0
";
                #endregion
            RunTest(sExpected, f);
        }


        [Test]
		public void FrLocals()
		{
			var f = new Frame(arch, PrimitiveType.Word16);
			f.EnsureStackLocal(-2, PrimitiveType.Word16);
			f.EnsureStackLocal(-4, PrimitiveType.Word32);
            var sExpected =
            #region Expected
                @"
// Mem0:Mem
// fp:fp
// %continuation:%continuation
// wLoc02:Stack -0002
// dwLoc04:Stack -0004
// return address size: 0
";
                #endregion
            RunTest(sExpected, f);
            Assert.IsNotNull((StackStorage) f.Identifiers[3].Storage);
		}

		[Test]
		public void FrSequenceAccess()
		{
			var f = new Frame(arch, PrimitiveType.Word16);
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier dx = f.EnsureRegister(Registers.dx);
			Identifier dx_ax = f.EnsureSequence(PrimitiveType.Word32, dx.Storage, ax.Storage);
			SequenceStorage vDx_ax = (SequenceStorage) dx_ax.Storage;
            var sExpected =
            #region Expected
                @"
// Mem0:Mem
// fp:fp
// %continuation:%continuation
// ax:ax
// dx:dx
// dx_ax:Sequence dx:ax
// return address size: 0
";
            #endregion
            RunTest(sExpected, f);
        }

        [Test]
        [Ignore("")]
		public void FrBindStackParameters()
		{
			var f = new Frame(arch, PrimitiveType.Word16);
			f.ReturnAddressSize = 4;						// far call.
			int stack = 2;
			Identifier loc02 = f.EnsureStackLocal(-stack, PrimitiveType.Word16, "wLoc02");
			stack += loc02.DataType.Size;
			f.EnsureStackLocal(-stack, PrimitiveType.Word16, "wLoc04");

			FunctionType sig = FunctionType.Action(
					new Identifier("arg0", PrimitiveType.Word16, new StackStorage(4, PrimitiveType.Word16)),
					new Identifier("arg1", PrimitiveType.Word16, new StackStorage(6, PrimitiveType.Word16)));

			var cs = new CallSite(f.ReturnAddressSize + 2 * 4, 0);
			var fn = new ProcedureConstant(PrimitiveType.Ptr32, new IntrinsicProcedure("foo", true, sig));
			var ab = arch.CreateFrameApplicationBuilder(f, cs);
            Instruction instr = ab.CreateInstruction(fn, sig, null);
            var sExpected =
            #region Expected
                "@@@";
            #endregion
            RunTest(sExpected, f);
        }

        [Test]
		public void FrBindMixedParameters()
		{
			var f = new Frame(arch, PrimitiveType.Word16);
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier cx = f.EnsureRegister(Registers.cx);
			int stack = PrimitiveType.Word16.Size;
			f.EnsureStackLocal(-stack, PrimitiveType.Word16);

            FunctionType sig = FunctionType.Func(
                ax,
                cx,
                new Identifier("arg0", PrimitiveType.Word16, new StackStorage(0, PrimitiveType.Word16)));
			var cs = new CallSite(stack, 0);
			var fn = new ProcedureConstant(PrimitiveType.Ptr32, new IntrinsicProcedure("bar", true, sig));
			var ab = new FrameApplicationBuilder(arch, f, cs);
            Instruction instr = ab.CreateInstruction(fn, sig, null);
            var sExpected =
            #region Expected
                @"
// Mem0:Mem
// fp:fp
// %continuation:%continuation
// ax:ax
// cx:cx
// wLoc02:Stack -0002
// sp:sp
// ss:ss
// return address size: 0
";
            #endregion
            RunTest(sExpected, f);
        }

        [Test]
		public void FrFpuStack()
		{
			var f = new Frame(arch, PrimitiveType.Word16);
			f.EnsureFpuStackVariable(-1, PrimitiveType.Real64);
			f.EnsureFpuStackVariable(-2, PrimitiveType.Real64);
			f.EnsureFpuStackVariable(0, PrimitiveType.Real64);

            var sExpected =
            #region Expected
                @"
// Mem0:Mem
// fp:fp
// %continuation:%continuation
// rLoc1:FPU -1
// rLoc2:FPU -2
// rArg0:FPU +0
// return address size: 0
";
            #endregion
            RunTest(sExpected, f);
        }

        [Test]
		public void FrEnsureRegister()
		{
			var f = new Frame(arch, PrimitiveType.Word32);
			f.EnsureRegister(RegisterStorage.Reg32("eax", 0));
			Assert.AreEqual("eax", f.Identifiers[3].Name);
			Assert.AreSame(PrimitiveType.Word32, f.Identifiers[3].DataType);
		}

		[Test]
		public void EnsureOutRegister()
		{
			var f = new Frame(arch, PrimitiveType.Word32);
			Identifier r = f.EnsureRegister(RegisterStorage.Reg32("r1", 1));
			Identifier arg = f.EnsureOutArgument(r, PrimitiveType.Ptr32);
			Assert.AreEqual("r1Out", arg.Name);
			Assert.AreSame(PrimitiveType.Ptr32, arg.DataType);
		}
	}
}
