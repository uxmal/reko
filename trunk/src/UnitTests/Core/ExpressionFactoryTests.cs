#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ExpressionFactoryTests
	{
		private TypeFactory typefactory;
		private ExpressionFactory factory;

		[SetUp]
		public void Setup()
		{
			typefactory = new TypeFactory();
			factory = new ExpressionFactory();
		}

		[Test]
		public void CreateId()
		{
			Identifier id = factory.Identifier(PrimitiveType.Word32, 0, "foo", null);
		}

		[Test]
		public void CreateConst()
		{
			Constant c = factory.Int32(PrimitiveType.Word32, 3);
			Assert.AreEqual("0x00000003", c.ToString());
		}

		[Test]
		public void BoolConsts()
		{
			Assert.AreEqual("true", factory.True.ToString());
			Assert.AreEqual("false", factory.False.ToString());
		}
	}
}
