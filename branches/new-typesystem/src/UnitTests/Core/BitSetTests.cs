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

using Decompiler.Core.Lib;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Core
{
	/// <summary>
	/// NUnit tests for BitSet class.
	/// </summary>
	[TestFixture]
	public class BitSetTests
	{
		[Test]
		public void CreateBitSet()
		{
			BitSet b = new BitSet(32);
			b[0] = true;
			b[1] = false;
			Assert.IsTrue(b[0]);
			Assert.IsTrue(!b[1]);
		}

		[Test]
		public void BitSetToString()
		{
			BitSet b = new BitSet(8);
			b[0] = true;
			b[1] = false;
			b[2] = true;
			b[7] = true;
			b[31] = true;
			string sExp = "10100001";
			string s = b.ToString();
			Assert.AreEqual(sExp, s);
		}

		[Test]
		public void BitSetOperators()
		{
			BitSet a = new BitSet(8);
			a[0] = true;
			a[1] = true;
			a[2] = false;
			a[3] = false;
			a[4] = true;
			BitSet b = new BitSet(8);
			b[0] = true;
			b[1] = false;
			b[2] = false;
			b[3] = true;
			b[4] = true;
			string sExp = "10001000";
			string s = (a & b).ToString();
			Assert.AreEqual(sExp, s);
			sExp = "11011000";
			s = (a | b).ToString();
			Assert.AreEqual(sExp, s);
			sExp = "01000000";
			s = (a - b).ToString();
			Assert.AreEqual(sExp, s);
		}

		[Test]
		public void BitSetEmpty()
		{
			BitSet a = new BitSet(8);
			Assert.IsTrue(a.IsEmpty);
			BitSet b = new BitSet(8);
			b[7] = true;
			Assert.IsTrue(!b.IsEmpty);
			b[7] = false;
			Assert.IsTrue(b.IsEmpty);
		}

		[Test]
		public void BitSetAll()
		{
			BitSet a = new BitSet(8);
			a.SetAll(true);
			string sExp = "11111111";
			string s = a.ToString();
			Assert.AreEqual(sExp, s);
			a.SetAll(false);
			sExp = "00000000";
			s = a.ToString();
			Assert.AreEqual(sExp, s);
		}

		[Test]
		public void BitSetClone()
		{
			BitSet a = new BitSet(8);
			a[0] = true;
			a[2] = true;
			BitSet b = new BitSet(a);
			string sExp = "10100000";
			string s = b.ToString();
			Assert.AreEqual(sExp, s);
			BitSet c = (BitSet) b.Clone();
			s = c.ToString();
			Assert.AreEqual(sExp, s);
		}

		private void EnumerateBitset(BitSet bitset, TextWriter sb)
		{
			sb.Write("{ ");
			foreach (int b in bitset)
			{
				sb.Write("{0} ", b);
			}
			sb.WriteLine("}");
		}

		private void EnumerateBitsetBackwards(BitSet bitset, TextWriter sb)
		{
			sb.Write("{ ");
			foreach (int b in bitset.Reverse)
			{
				sb.Write("{0} ", b);
			}
			sb.WriteLine("}");
		}

		[Test]
		public void BitSetEnumerate()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/BitSetEnumerate.txt"))
			{
				BitSet a = new BitSet(8);
				a[0] = true;
				a[2] = true;
				EnumerateBitset(a, fut.TextWriter);
				a = new BitSet(8);		// test empty set.
				EnumerateBitset(a, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void BitSetAllEnumerate()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/BitSetAll.txt"))
			{
				BitSet a = new BitSet(8);
				a.SetAll(true);
				EnumerateBitsetBackwards(a, fut.TextWriter);
				fut.AssertFilesEqual();

			}
		}

		[Test]
		public void BitSetReverseEnumerate()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/BitSetReverseEnumerate.txt"))
			{
				BitSet b = new BitSet(49);
				b[42] = true;
				b[3] = true;

				EnumerateBitsetBackwards(b, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}
