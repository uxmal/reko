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

using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class SetTests
	{
		public SetTests()
		{
		}

		[Test]
		public void CreateTest()
		{
			Set s = new Set();
			Assert.IsTrue(s.Count == 0);
		}

		[Test]
		public void AddSomeTest()
		{
			Set s = new Set();
			s[3] = true;
			s[1] = false;
			s[2] = true;
			Assert.IsTrue(Dump(s) == "2 3 ");
			s[2] = false;
			Assert.IsTrue(Dump(s) == "3 ");
		}

		private string Dump(Set s)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (object o in s)
			{
				sb.Append(o.ToString());
				sb.Append(" ");
			}
			return sb.ToString();
		}
	}
}
