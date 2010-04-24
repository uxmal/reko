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
using System.IO;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class FrameTests
	{
		[Test]
		public void RegisterTest()
		{
			IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
			Frame f = new Frame(PrimitiveType.Word16);
			Identifier id0 = f.EnsureRegister(Registers.ax);
			Identifier id1 = f.EnsureRegister(Registers.bx);
			Identifier id2 = f.EnsureRegister(Registers.ax);
			Assert.AreEqual(id0, id2);
		}

		[Test]
		public void SequenceTest()
		{
			IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
			Frame f = new Frame(PrimitiveType.Word16);
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier dx = f.EnsureRegister(Registers.dx);
			Identifier dxax = f.EnsureSequence(dx, ax, PrimitiveType.Word32);

			using (FileUnitTester fut = new FileUnitTester("Core/SequenceTest.txt"))
			{
				f.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}

			Identifier dxax2 = f.EnsureSequence(dx,ax, PrimitiveType.Word32);
			Assert.IsTrue(dxax2 == dxax);
		}

		[Test]
		public void FrGrfTest()
		{
			IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
			Frame f = new Frame(PrimitiveType.Word16);
			uint iSz = (uint) (FlagM.ZF|FlagM.SF);
			Identifier grfSz = f.EnsureFlagGroup(iSz, arch.GrfToString(iSz), PrimitiveType.Byte);
			using (FileUnitTester fut = new FileUnitTester("Core/FrGrfTest.txt"))
			{
				f.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}


		[Test]
		public void FrLocals()
		{
			Frame f = new Frame(PrimitiveType.Word16);
			f.EnsureStackLocal(2, PrimitiveType.Word16);
			f.EnsureStackLocal(4, PrimitiveType.Word32);
			using (FileUnitTester fut = new FileUnitTester("Core/FrLocals.txt"))
			{
				f.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
			Assert.IsNotNull((StackLocalStorage) f.Identifiers[2].Storage);
		}

		[Test]
		public void FrSequenceAccess()
		{
			IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
			Frame f = new Frame(PrimitiveType.Word16);
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier dx = f.EnsureRegister(Registers.dx);
			Identifier dx_ax = f.EnsureSequence(dx, ax, PrimitiveType.Word32);
			SequenceStorage vDx_ax = (SequenceStorage) dx_ax.Storage;
			using (FileUnitTester fut = new FileUnitTester("Core/FrSequenceAccess.txt"))
			{
				f.Write(fut.TextWriter);
				fut.TextWriter.WriteLine("Head({0}) = {1}", dx_ax.Name, vDx_ax.Head.Name);
				fut.TextWriter.WriteLine("Tail({0}) = {1}", dx_ax.Name, vDx_ax.Tail.Name);

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void FrBindStackParameters()
		{
			Frame f = new Frame(PrimitiveType.Word16);
			f.ReturnAddressSize = 4;						// far call.
			int stack = 2;
			Identifier loc02 = f.EnsureStackLocal(-stack, PrimitiveType.Word16, "wLoc02");
			stack += loc02.DataType.Size;
			Identifier loc04 = f.EnsureStackLocal(-stack, PrimitiveType.Word16, "wLoc04");

			ProcedureSignature sig = new ProcedureSignature(
				null, new Identifier[] {
					new Identifier("arg0", 0, PrimitiveType.Word16, new StackArgumentStorage(4, PrimitiveType.Word16)),
					new Identifier("arg1", 1, PrimitiveType.Word16, new StackArgumentStorage(6, PrimitiveType.Word16)) });

			CallSite cs = new CallSite(stack + f.ReturnAddressSize, 0);
			ProcedureConstant fn = new ProcedureConstant(PrimitiveType.Pointer32, new PseudoProcedure("foo", sig));
			ApplicationBuilder ab = new ApplicationBuilder(f, cs, fn, sig);
            Instruction instr = ab.CreateInstruction(); 
			using (FileUnitTester fut = new FileUnitTester("Core/FrBindStackParameters.txt"))
			{
				f.Write(fut.TextWriter);
				fut.TextWriter.WriteLine(instr.ToString());
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void FrBindMixedParameters()
		{
			IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
			Frame f = new Frame(PrimitiveType.Word16);
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier cx = f.EnsureRegister(Registers.cx);
			int stack = PrimitiveType.Word16.Size;
			Identifier arg1 = f.EnsureStackLocal(-stack, PrimitiveType.Word16);

			ProcedureSignature sig = new ProcedureSignature(
				ax,
			    cx,
			    new Identifier("arg0", 0, PrimitiveType.Word16, new StackArgumentStorage(0, PrimitiveType.Word16)));
			
			CallSite cs = new CallSite(stack, 0);
			ProcedureConstant fn = new ProcedureConstant(PrimitiveType.Pointer32, new PseudoProcedure("bar", sig));
			ApplicationBuilder ab = new ApplicationBuilder(f, cs, fn, sig);
            Instruction instr = ab.CreateInstruction();
			using (FileUnitTester fut = new FileUnitTester("Core/FrBindMixedParameters.txt"))
			{
				f.Write(fut.TextWriter);
				fut.TextWriter.WriteLine(instr.ToString());
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void FrFpuStack()
		{
			Frame f = new Frame(PrimitiveType.Word16);
			f.EnsureFpuStackVariable(-1, PrimitiveType.Real64);
			f.EnsureFpuStackVariable(-2, PrimitiveType.Real64);
			f.EnsureFpuStackVariable(0, PrimitiveType.Real64);
			
			using (FileUnitTester fut = new FileUnitTester("Core/FrFpuStack.txt"))
			{
				f.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void FrEnsureRegister()
		{
			Frame f = new Frame(PrimitiveType.Word32);
			f.EnsureRegister(new Mocks.MockMachineRegister("eax", 0, PrimitiveType.Word32));
			Assert.AreEqual("eax", f.Identifiers[2].Name);
			Assert.AreSame(PrimitiveType.Word32, f.Identifiers[2].DataType);
		}

		[Test]
		public void EnsureOutRegister()
		{
			Frame f = new Frame(PrimitiveType.Word32);
			Identifier r = f.EnsureRegister(new Mocks.MockMachineRegister("r1", 1, PrimitiveType.Word32));
			Identifier arg = f.EnsureOutArgument(r, PrimitiveType.Pointer32);
			Assert.AreEqual("r1Out", arg.Name);
			Assert.AreSame(PrimitiveType.Pointer32, arg.DataType);
		}

		[Test]
		public void BindStackArgumentToCallerFrame()
		{
			Procedure callee = new Procedure("callee", new Frame(PrimitiveType.Word16));
			callee.Frame.ReturnAddressSize = 2;				// "near" call.
			Identifier arg02 = callee.Frame.EnsureStackArgument(2, PrimitiveType.Word16, "wArg02");
			Identifier arg04 = callee.Frame.EnsureStackArgument(4, PrimitiveType.Word32, "dwArg04");
			Identifier arg08 = callee.Frame.EnsureStackArgument(8, PrimitiveType.Word16, "wArg08");

			Procedure caller = new Procedure("caller", new Frame(PrimitiveType.Word16));
			caller.Frame.ReturnAddressSize = 2;
			caller.Frame.EnsureStackLocal(-2, PrimitiveType.Word16, "bpSaved");
			caller.Frame.EnsureStackLocal(-4, PrimitiveType.Word16, "bindToArg08");
			caller.Frame.EnsureStackLocal(-8, PrimitiveType.Word32, "bindToArg04");
			caller.Frame.EnsureStackLocal(-10, PrimitiveType.Word16, "bindToArg02");
			CallSite cs = new CallSite(10 + callee.Frame.ReturnAddressSize, 0);
			Identifier id = arg08.Storage.BindFormalArgumentToFrame(caller.Frame, cs);
			Assert.AreEqual("bindToArg08", id.Name);
			id = arg02.Storage.BindFormalArgumentToFrame(caller.Frame, cs);
			Assert.AreEqual("bindToArg02", id.Name);
			id = arg04.Storage.BindFormalArgumentToFrame(caller.Frame, cs);
			caller.Frame.Write(Console.Out);

			Assert.AreEqual("bindToArg04", id.Name);
		}
	}
}
