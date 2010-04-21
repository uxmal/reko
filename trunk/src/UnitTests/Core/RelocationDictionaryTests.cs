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
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class RelocationDictionaryTests
	{
		[Test]
		public void AddSegmentRelocation()
		{
			RelocationDictionary rd = new RelocationDictionary();
			rd.AddSegmentReference(0xD234, 0x0C00);
			Assert.AreEqual(1, rd.Count);
			Constant c = rd[0xD234];
			Assert.AreEqual("selector", c.DataType.ToString());
		}

		[Test]
		public void AddPointerRelocation()
		{
			RelocationDictionary rd = new RelocationDictionary();
			rd.AddPointerReference(0x100400, 0x100500);
			Assert.AreEqual(1, rd.Count);
			Constant c = rd[0x0100400];
			Assert.AreEqual("ptr32", c.DataType.ToString());
		}

		[Test]
		public void AddSegmentRelocationByLinearAddress()
		{
			RelocationDictionary rd = new RelocationDictionary();
			rd.AddSegmentReference(0x0C010, 0x0C00);
			Assert.AreEqual(1, rd.Count);
			Constant c = rd[0x0C010];
			Assert.AreEqual("selector", c.DataType.ToString());
			c = rd[0x0C010];
			Assert.AreEqual("selector", c.DataType.ToString());
		}

		[Test]
		public void AddressNotInDictionary()
		{
			RelocationDictionary rd = new RelocationDictionary();
			rd.AddPointerReference(0x020, 0x12312312);
			Assert.IsNull(rd[0x3243232]);
			Assert.IsFalse(rd.Contains(0x2341231));
		}
	}
}
