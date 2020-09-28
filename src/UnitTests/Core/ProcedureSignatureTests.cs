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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Arch.X86;
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture] //$TODO: rename to FunctionTypeTests
	public class ProcedureSignatureTests
	{
		[Test]
		public void PsigArguments()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/PsigArguments.txt"))
			{
				IntelArchitecture arch = new X86ArchitectureReal("x86-real-16");
				uint f = (uint)(FlagM.CF|FlagM.ZF);
				Identifier argF = new Identifier(arch.GetFlagGroup("CZ").ToString(), PrimitiveType.Bool, new FlagGroupStorage(Registers.eflags, f, "CZ", PrimitiveType.Byte));
				Identifier argR = new Identifier(Registers.ax.Name, Registers.ax.DataType, Registers.ax);
				
				argF.Write(true, fut.TextWriter);
				fut.TextWriter.WriteLine();
				argR.Write(true, fut.TextWriter);
				fut.TextWriter.WriteLine();

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void PsigArgument()
		{
			Identifier arg = new Identifier(Registers.eax.Name, Registers.eax.DataType, Registers.eax);
			Assert.AreEqual("eax", arg.Name);
			Assert.AreEqual(PrimitiveType.Word32, arg.DataType);
			Assert.AreEqual("eax", arg.Name);
			Assert.AreEqual(PrimitiveType.Word32, arg.DataType);
		}

		[Test]
		public void PsigValidArguments()
		{
			Identifier arg = new Identifier(Registers.eax.Name, Registers.eax.DataType, Registers.eax);
			FunctionType sig = FunctionType.Action(arg);
			Assert.IsTrue(sig.ParametersValid);

			sig = new FunctionType(arg, new Identifier[0]);
			Assert.IsTrue(sig.ParametersValid);

			sig = new FunctionType();
			Assert.IsFalse(sig.ParametersValid);
		}
	}
}
