/* 
 * Copyright (C) 1999-2009 John Källén.
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

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class UseInstructionTests
	{
		[Test]
		public void UseCreate()
		{
			Identifier id1 = new Identifier("foo", 1, PrimitiveType.Word32, new TemporaryStorage());
			UseInstruction use = new UseInstruction(id1);
			Assert.AreSame(id1, use.Expression);
			Assert.IsNull(use.OutArgument);
		}

		[Test]
		public void UseCreateWithArg()
		{
			Identifier id2 = new Identifier("bar", -1, PrimitiveType.Word32, new TemporaryStorage());
			Identifier r = new Identifier(Registers.edx.Name, 2, Registers.edx.DataType, new RegisterStorage(Registers.edx));
			Identifier arg = new Identifier("barOut", 3, PrimitiveType.Pointer32, new OutArgumentStorage(r));
			UseInstruction use2 = new UseInstruction(id2, arg);
			Assert.AreSame(id2, use2.Expression);
			Assert.AreEqual("barOut", use2.OutArgument.Name);
		}

		[Test]
		public void UseToString()
		{
			Identifier id1 = new Identifier("foo", 1, PrimitiveType.Word32, null);
			UseInstruction use = new UseInstruction(id1);
			Assert.AreEqual("use foo", use.ToString());

			Identifier r = new Identifier(Registers.edx.Name, 3, Registers.edx.DataType, new RegisterStorage(Registers.edx));
			Identifier arg = new Identifier("edxOut", 4, PrimitiveType.Pointer32, new OutArgumentStorage(r));
			use = new UseInstruction(id1, arg);
			Assert.AreEqual("use foo (=> edxOut)" , use.ToString());
		}
	}
}