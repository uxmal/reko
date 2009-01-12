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

using Decompiler.Core;
using Decompiler.Scanning;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Scanning
{
	[TestFixture]
	public class WorkItemTests
	{
		[Test]
		public void WorkItemChain0()
		{
			WorkItem w = new WorkItem(null, BlockType.Procedure, new Address(0x10000000));
			Assert.AreEqual(@"WorkItem @ 10000000
", w.ToString());
		}
	}
}
