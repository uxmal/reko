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

using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class OperatorTests
	{
		[Test]
		public void AddApplyConstants()
		{
			RunApply(1, 2, new AddOperator(), 3);
		}

		[Test]
		public void AndApplyConstants()
		{
			Constant c1 = Constant.Word32(1);
			Constant c2 = Constant.Word32(2);
			BinaryOperator op = new AndOperator();
			Constant c3 = op.ApplyConstants(c1, c2);
			Assert.AreEqual(0, Convert.ToInt32(c3.Value));
		}

		[Test]
		public void OrApplyConstants()
		{
			Constant c1 = Constant.Word32(1);
			Constant c2 = Constant.Word32(2);
			BinaryOperator op = new OrOperator();
			Constant c3 = op.ApplyConstants(c1, c2);
			Assert.AreEqual(3, Convert.ToInt32(c3.Value));
		}

		[Test]
		public void XorApplyConstants()
		{
			RunApply(0xC, 0xA, new XorOperator(), 6);
		}

		[Test]
		public void SarApplyConstants()
		{
			RunApply(0x30, 2, new SarOperator(), 12);
		}

		[Test]
		public void ShrApplyConstants()
		{
			RunApply(0x30, 2, new ShrOperator(), 12);
		}

		[Test]
		public void ShlApplyConstants()
		{
			RunApply(0xC, 2, new ShlOperator(), 48);
		}

		[Test]
		public void SubApplyConstants()
		{
			RunApply(0x1, 0x2, new SubOperator(), -1);
		}

		[Test]
		public void MulsApplyConstants()
		{
			RunApply(0x1, 0x2, new MulsOperator(), 2);
		}

		[Test]
		public void DivuApplyConstants()
		{
			RunApply(10, 3, new DivuOperator(), 3);
		}

		[Test]
		public void ModApplyConstants()
		{
			RunApply(0x5, 0x3, new ModOperator(), 2);
		}

		[Test]
		public void CandCreate()
		{
			Assert.IsNotNull(new CandOperator());
		}

		[Test]
		public void CorCreate()
		{
			Assert.IsNotNull(new CorOperator());
		}

		[Test]
		public void ltCreate()
		{
			Assert.IsNotNull(new LtOperator());
		}
		
		[Test]
		public void gtCreate()
		{
			Assert.IsNotNull(new GtOperator());
		}
		
		[Test]
		public void leCreate()
		{
			Assert.IsNotNull(new LeOperator());
		}
		
		[Test]
		public void geCreate()
		{
			Assert.IsNotNull(new GeOperator());
		}
		
		[Test]
		public void ultCreate()
		{
			Assert.IsNotNull(new UltOperator());
		}
		
		[Test]
		public void ugtCreate()
		{
			Assert.IsNotNull(new UgtOperator());
		}
		
		[Test]
		public void uleCreate()
		{
			Assert.IsNotNull(new UleOperator());
		}
		
		[Test]
		public void ugeCreate()
		{
			Assert.IsNotNull(new UgeOperator());
		}
		
		[Test]
		public void eqCreate()
		{
			Assert.IsNotNull(new EqOperator());
		}
		
		[Test]
		public void neCreate()
		{
			Assert.IsNotNull(new NeOperator());
		}
		
		[Test]
		public void NotCreate()
		{
			Assert.IsNotNull(new NotOperator());
		}

		[Test]
		public void NegApply()
		{
			RunApply(3, new NegateOperator(), -3);
		}

		[Test]
		public void ComplementApply()
		{
			RunApply(3, new ComplementOperator(), -4);
		}

		[Test]
		public void AddressOfCreate()
		{
			Assert.IsNotNull(new AddressOfOperator());
		}



		private void RunApply(int a, int b, BinaryOperator op, int expected)
		{
			Constant c1 = Constant.Word32(a);
			Constant c2 = Constant.Word32(b);
			Constant c3 = op.ApplyConstants(c1, c2);
			Assert.AreEqual(expected, (int) Convert.ToInt64(c3.Value));
		}

		private void RunApply(int a, UnaryOperator op, int expected)
		{
			Constant c1 = Constant.Word32(a);
			Constant c2 = op.ApplyConstant(c1);
			Assert.AreEqual(expected, (int) Convert.ToInt64(c2.Value));
		}
	}
}
