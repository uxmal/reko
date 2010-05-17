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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Arch.Intel;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ProcedureSignatureTests
	{
		[Test]
		public void PsigArguments()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/PsigArguments.txt"))
			{
				IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
				uint f = (uint)(FlagM.CF|FlagM.ZF);
				Identifier argF = new Identifier(arch.GrfToString(f), 0, PrimitiveType.Bool, new FlagGroupStorage(f, "CZ"));
				Identifier argR = new Identifier(Registers.ax.Name, 1, Registers.ax.DataType, new RegisterStorage(Registers.ax));
				
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
			Identifier arg = new Identifier(Registers.eax.Name, 0, Registers.eax.DataType, new RegisterStorage(Registers.eax));
			Assert.AreEqual("eax", arg.Name);
			Assert.AreEqual(PrimitiveType.Word32, arg.DataType);
			Assert.AreEqual("eax", arg.Name);
			Assert.AreEqual(PrimitiveType.Word32, arg.DataType);

			Identifier arg2 = new Identifier(Registers.eax.Name, 0, Registers.eax.DataType, new RegisterStorage(Registers.eax));
		}

		[Test]
		public void PsigValidArguments()
		{
			Identifier arg = new Identifier(Registers.eax.Name, 0, Registers.eax.DataType, new RegisterStorage(Registers.eax));
			ProcedureSignature sig = new ProcedureSignature(null, new Identifier[] { arg });
			Assert.IsTrue(sig.ArgumentsValid);

			sig = new ProcedureSignature(arg, null);
			Assert.IsTrue(sig.ArgumentsValid);

			sig = new ProcedureSignature();
			Assert.IsFalse(sig.ArgumentsValid);
		}
	}
}
