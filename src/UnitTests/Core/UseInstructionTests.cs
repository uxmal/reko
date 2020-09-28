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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class UseInstructionTests
	{
		[Test]
		public void UseCreate()
		{
			var id1 = new Identifier("foo", PrimitiveType.Word32, new TemporaryStorage("foo", 1, PrimitiveType.Word32));
			var use = new UseInstruction(id1);
			Assert.AreSame(id1, use.Expression);
			Assert.IsNull(use.OutArgument);
		}

		[Test]
		public void UseCreateWithArg()
		{
			var reg_edx = new RegisterStorage("edx", 1, 0, PrimitiveType.Word32);
			var id2 = new Identifier("bar", PrimitiveType.Word32, new TemporaryStorage("bar", -1, PrimitiveType.Word32));
			var r = new Identifier(reg_edx.Name, reg_edx.DataType, reg_edx);
			var arg = new Identifier("barOut", PrimitiveType.Ptr32, new OutArgumentStorage(r));
			var use2 = new UseInstruction(id2, arg);
			Assert.AreSame(id2, use2.Expression);
			Assert.AreEqual("barOut", use2.OutArgument.Name);
		}

		[Test]
		public void UseToString()
		{
			var reg_edx = new RegisterStorage("edx",1, 0,PrimitiveType.Word32);
			var id1 = new Identifier("foo", PrimitiveType.Word32, null);
			var use = new UseInstruction(id1);
			Assert.AreEqual("use foo", use.ToString());

			var r = new Identifier(reg_edx.Name, reg_edx.DataType, reg_edx);
			var arg = new Identifier("edxOut", PrimitiveType.Ptr32, new OutArgumentStorage(r));
			use = new UseInstruction(id1, arg);
			Assert.AreEqual("use foo (=> edxOut)" , use.ToString());
		}
	}
}