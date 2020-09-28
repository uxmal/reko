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

using Reko.Core.Expressions;
using Reko.Core.Operators;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class OperatorTests
	{
		[Test]
		public void AddApplyConstants()
		{
			RunApply(1, 2, new IAddOperator(), 3);
		}

		[Test]
		public void AndApplyConstants()
		{
			Constant c1 = Constant.Word32(1);
			Constant c2 = Constant.Word32(2);
			BinaryOperator op = new AndOperator();
			Constant c3 = op.ApplyConstants(c1, c2);
			Assert.AreEqual(0, c3.ToInt32());
		}

		[Test]
		public void OrApplyConstants()
		{
			Constant c1 = Constant.Word32(1);
			Constant c2 = Constant.Word32(2);
			BinaryOperator op = new OrOperator();
			Constant c3 = op.ApplyConstants(c1, c2);
			Assert.AreEqual(3, c3.ToInt32());
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
			RunApply(0x1, 0x2, new ISubOperator(), -1);
		}

		[Test]
		public void MulsApplyConstants()
		{
			RunApply(0x1, 0x2, new SMulOperator(), 2);
		}

		[Test]
		public void DivuApplyConstants()
		{
			RunApply(10, 3, new UDivOperator(), 3);
		}

		[Test]
		public void ModApplyConstants()
		{
			RunApply(0x5, 0x3, new IModOperator(), 2);
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

        [Test]
        public void LtApplyConstants1()
        {
            RunApply(5, 3, new LtOperator(), 0);
        }

        [Test]
        public void LtApplyConstants2()
        {
            RunApply(4, 5, new LtOperator(), 1);
        }

        [Test]
        public void LtApplyConstants3()
        {
            RunApply(6, 6, new LtOperator(), 0);
        }

        [Test]
        public void GtApplyConstants1()
        {
            RunApply(5, 3, new GtOperator(), 1);
        }

        [Test]
        public void GtApplyConstants2()
        {
            RunApply(4, 5, new GtOperator(), 0);
        }

        [Test]
        public void GtApplyConstants3()
        {
            RunApply(6, 6, new GtOperator(), 0);
        }

        [Test]
        public void LeApplyConstants1()
        {
            RunApply(5, 3, new LeOperator(), 0);
        }

        [Test]
        public void LeApplyConstants2()
        {
            RunApply(4, 5, new LeOperator(), 1);
        }

        [Test]
        public void LeApplyConstants3()
        {
            RunApply(6, 6, new LeOperator(), 1);
        }

        [Test]
        public void GeApplyConstants1()
        {
            RunApply(5, 3, new GeOperator(), 1);
        }

        [Test]
        public void GeApplyConstants2()
        {
            RunApply(4, 5, new GeOperator(), 0);
        }

        [Test]
        public void GeApplyConstants3()
        {
            RunApply(6, 6, new GeOperator(), 1);
        }



        private void RunApply(int a, int b, BinaryOperator op, int expected)
		{
			Constant c1 = Constant.Word32(a);
			Constant c2 = Constant.Word32(b);
			Constant c3 = op.ApplyConstants(c1, c2);
			Assert.AreEqual(expected, (int) c3.ToInt64());
		}

		private void RunApply(int a, UnaryOperator op, int expected)
		{
			Constant c1 = Constant.Word32(a);
			Constant c2 = op.ApplyConstant(c1);
			Assert.AreEqual(expected, (int) c2.ToInt64());
		}
	}
}
