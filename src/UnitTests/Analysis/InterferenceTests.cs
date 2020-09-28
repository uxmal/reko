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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class IntereferenceTests
	{
		[Test]
		public void InterfSymmetry()
		{
			var id1 = new Identifier("id1", PrimitiveType.Word32, null);
			var id2 = new Identifier("id2", PrimitiveType.Word32, null);

			var intf1 = new Interference(id1, id2);
			var intf2 = new Interference(id1, id2);
			var intf3 = new Interference(id2, id1);
			Assert.AreEqual(intf1.GetHashCode(), intf2.GetHashCode());
			Assert.AreEqual(intf1.GetHashCode(), intf3.GetHashCode());
			Assert.AreEqual(intf2.GetHashCode(), intf3.GetHashCode());

			Assert.AreEqual(intf1, intf2, "1 compared 2");
			Assert.AreEqual(intf1, intf3, "1 compared 3");
			Assert.AreEqual(intf2, intf3, "2 compared 3");
		}
	}
}
