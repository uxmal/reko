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
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class TypeFactoryTests
	{
		private TypeFactory factory;

		[SetUp]
		public void Setup()
		{
			factory = new TypeFactory();
		}
		
		[Test]
		public void TfacCreation()
		{
			PrimitiveType it = factory.CreatePrimitiveType(Domain.UnsignedInt, 32);
			Assert.AreEqual(4, it.Size);

			StructureType str = factory.CreateStructureType("niz", 30);
			Assert.AreEqual(30, str.Size);
			Assert.AreEqual("niz", str.Name);

			str = factory.CreateStructureType("foo", 30);
			Assert.AreEqual(30, str.Size);
			Assert.AreEqual("foo", str.Name);
		}


		[Test]
		public void TfacIdenticalPrimitives()
		{
			object o1 = factory.CreatePrimitiveType(Domain.SignedInt, 16);
			object o2 = factory.CreatePrimitiveType(Domain.SignedInt, 16);
			Assert.AreSame(o1, o2);
		}
	}
}
