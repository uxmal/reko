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
using System.Collections;

namespace Reko.UnitTests.Core
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
		public void Reld_AddressNotInDictionary()
		{
			RelocationDictionary rd = new RelocationDictionary();
			rd.AddPointerReference(0x020, 0x12312312);
			Assert.IsNull(rd[0x3243232]);
			Assert.IsFalse(rd.Contains(0x2341231));
		}

        [Test]
        public void Reld_Overlaps()
        {
            var rd = new RelocationDictionary();
            rd.AddPointerReference(0x2000, 0x12312312);
            Assert.IsFalse(rd.Overlaps(Address.Ptr32(0x1FFC), 4));
            Assert.IsFalse(rd.Overlaps(Address.Ptr32(0x2004), 1));
            Assert.IsTrue(rd.Overlaps(Address.Ptr32(0x2003), 1));
            Assert.IsTrue(rd.Overlaps(Address.Ptr32(0x1FFC), 5));
            Assert.IsFalse(rd.Overlaps(Address.Ptr32(0x1FFF), 5));
            Assert.IsFalse(rd.Overlaps(Address.Ptr32(0x2000), 4));
            Assert.IsFalse(rd.Overlaps(Address.Ptr32(0x2000), 8));
            Assert.IsFalse(rd.Overlaps(Address.Ptr32(0x1FFC), 8));
        }
	}
}
