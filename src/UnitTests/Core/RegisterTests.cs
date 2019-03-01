#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using NUnit.Framework;

using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class RegisterStorageTests
	{
		[Test]
		public void Create()
		{
			RegisterStorage reg = new RegisterStorage("eax", 0, 0, PrimitiveType.Word32);
			Assert.AreEqual("eax", reg.Name);
			Assert.AreEqual(0, reg.Number);
			Assert.AreEqual("word32", reg.DataType.ToString());
		}
	}
}
