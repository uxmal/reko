/* 
 * Copyright (C) 1999-2008 John Källén.
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
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class WorkListTests
	{
		[Test]
		public void WlAdd()
		{
			WorkList w = new WorkList();
			w.Add(3);
			Assert.IsFalse(w.IsEmpty);
			Assert.AreEqual(3, w.GetWorkItem());
			Assert.IsTrue(w.IsEmpty);
		}

		[Test]
		public void WlRemove()
		{
			WorkList w = new WorkList();
			w.Add(3);
			w.Add(2);
			Assert.IsFalse(w.IsEmpty);
			w.Remove(3);
			Assert.IsFalse(w.IsEmpty);
			w.Remove(2);
			Assert.IsTrue(w.IsEmpty);
			Assert.IsNull(w.GetWorkItem());
		}
	}
}
