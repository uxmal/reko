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
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class IdentifierTests
	{
		private Identifier reg;
		private Identifier outReg;

		public IdentifierTests()
		{
			var reg_internal = new RegisterStorage("eax", 0, 0, PrimitiveType.Word32);
			reg = new Identifier("eax", PrimitiveType.Word32, reg_internal);
			outReg = new Identifier("eaxOut", PrimitiveType.Ptr32, new OutArgumentStorage(reg));
		}

		[Test]
		public void RegToString()
		{
			Assert.AreEqual("eax", reg.ToString());
		}

        [Test]
        public void HashCode()
        {
            Assert.AreNotEqual(reg.GetHashCode(), outReg.GetHashCode());
            Assert.IsFalse(object.Equals(reg, outReg));
        }
	}
}