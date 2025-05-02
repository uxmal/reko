#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using NUnit.Framework;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class OperatorTests
	{
		[Test]
		public void AddApplyConstants()
		{
			RunApply(1, 2, Operator.IAdd, 3);
		}

        [Test]
        public void AddApplyByteOffsetToPtr()
        {
            var c1 = Constant.Create(PrimitiveType.Ptr16, 0x402);
            var c2 = Constant.Create(PrimitiveType.Byte, 0xFE);
            Constant c3 = Operator.IAdd.ApplyConstants(c1.DataType, c1, c2);
            Assert.AreEqual(0x500, (int) c3.ToInt64());
        }

        [Test]
		public void AndApplyConstants()
		{
			Constant c1 = Constant.Word32(1);
			Constant c2 = Constant.Word32(2);
			BinaryOperator op = Operator.And;
			Constant c3 = op.ApplyConstants(c1.DataType, c1, c2);
			Assert.AreEqual(0, c3.ToInt32());
		}

		[Test]
		public void OrApplyConstants()
		{
			Constant c1 = Constant.Word32(1);
			Constant c2 = Constant.Word32(2);
			BinaryOperator op = Operator.Or;
			Constant c3 = op.ApplyConstants(c1.DataType, c1, c2);
			Assert.AreEqual(3, c3.ToInt32());
		}

		[Test]
		public void XorApplyConstants()
		{
			RunApply(0xC, 0xA, Operator.Xor, 6);
		}

		[Test]
		public void SarApplyConstants()
		{
			RunApply(0x30, 2, Operator.Sar, 12);
		}

		[Test]
		public void ShrApplyConstants()
		{
			RunApply(0x30, 2, Operator.Shr, 12);
		}

		[Test]
		public void ShlApplyConstants()
		{
			RunApply(0xC, 2, Operator.Shl, 48);
		}

		[Test]
		public void SubApplyConstants()
		{
			RunApply(0x1, 0x2, Operator.ISub, -1);
		}

		[Test]
		public void MulsApplyConstants()
		{
			RunApply(0x1, 0x2, Operator.SMul, 2);
		}

		[Test]
		public void DivuApplyConstants()
		{
			RunApply(10, 3, Operator.UDiv, 3);
		}

		[Test]
		public void ModApplyConstants()
		{
			RunApply(0x5, 0x3, Operator.IMod, 2);
		}

		[Test]
		public void CandCreate()
		{
			Assert.IsNotNull(Operator.Cand);
		}

		[Test]
		public void CorCreate()
		{
			Assert.IsNotNull(Operator.Cor);
		}

		[Test]
		public void ltCreate()
		{
			Assert.IsNotNull(Operator.Lt);
		}
		
		[Test]
		public void gtCreate()
		{
			Assert.IsNotNull(Operator.Gt);
		}
		
		[Test]
		public void leCreate()
		{
			Assert.IsNotNull(Operator.Le);
		}
		
		[Test]
		public void geCreate()
		{
			Assert.IsNotNull(Operator.Ge);
		}
		
		[Test]
		public void ultCreate()
		{
			Assert.IsNotNull(Operator.Ult);
		}
		
		[Test]
		public void ugtCreate()
		{
			Assert.IsNotNull(Operator.Ugt);
		}
		
		[Test]
		public void uleCreate()
		{
			Assert.IsNotNull(Operator.Ule);
		}
		
		[Test]
		public void ugeCreate()
		{
			Assert.IsNotNull(Operator.Uge);
		}
		
		[Test]
		public void eqCreate()
		{
			Assert.IsNotNull(Operator.Eq);
		}
		
		[Test]
		public void neCreate()
		{
			Assert.IsNotNull(Operator.Ne);
		}
		
		[Test]
		public void NotCreate()
		{
			Assert.IsNotNull(Operator.Not);
		}

		[Test]
		public void NegApply()
		{
			RunApply(3, Operator.Neg, -3);
		}

		[Test]
		public void ComplementApply()
		{
			RunApply(3, Operator.Comp, -4);
		}

		[Test]
		public void AddressOfCreate()
		{
			Assert.IsNotNull(Operator.AddrOf);
		}

        [Test]
        public void LtApplyConstants1()
        {
            RunApply(5, 3, Operator.Lt, 0);
        }

        [Test]
        public void LtApplyConstants2()
        {
            RunApply(4, 5, Operator.Lt, 1);
        }

        [Test]
        public void LtApplyConstants3()
        {
            RunApply(6, 6, Operator.Lt, 0);
        }

        [Test]
        public void GtApplyConstants1()
        {
            RunApply(5, 3, Operator.Gt, 1);
        }

        [Test]
        public void GtApplyConstants2()
        {
            RunApply(4, 5, Operator.Gt, 0);
        }

        [Test]
        public void GtApplyConstants3()
        {
            RunApply(6, 6, Operator.Gt, 0);
        }

        [Test]
        public void LeApplyConstants1()
        {
            RunApply(5, 3, Operator.Le, 0);
        }

        [Test]
        public void LeApplyConstants2()
        {
            RunApply(4, 5, Operator.Le, 1);
        }

        [Test]
        public void LeApplyConstants3()
        {
            RunApply(6, 6, Operator.Le, 1);
        }

        [Test]
        public void GeApplyConstants1()
        {
            RunApply(5, 3, Operator.Ge, 1);
        }

        [Test]
        public void GeApplyConstants2()
        {
            RunApply(4, 5, Operator.Ge, 0);
        }

        [Test]
        public void GeApplyConstants3()
        {
            RunApply(6, 6, Operator.Ge, 1);
        }



        private void RunApply(int a, int b, BinaryOperator op, int expected)
		{
			Constant c1 = Constant.Word32(a);
			Constant c2 = Constant.Word32(b);
			Constant c3 = op.ApplyConstants(c1.DataType, c1, c2);
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
