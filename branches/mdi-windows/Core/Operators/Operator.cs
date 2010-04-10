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

using System;

namespace Decompiler.Core.Operators
{
	public class Operator
	{
		public static readonly BinaryOperator Add = new AddOperator();
		public static readonly BinaryOperator And = new AndOperator();
		public static readonly BinaryOperator Divs = new DivsOperator();
		public static readonly BinaryOperator Divu = new DivuOperator();
		public static readonly BinaryOperator Mod = new ModOperator();
		public static readonly BinaryOperator Mul = new MulOperator();
		public static readonly BinaryOperator Muls = new MulsOperator();
		public static readonly BinaryOperator Mulu = new MuluOperator();

		public static readonly BinaryOperator Or = new OrOperator();
		public static readonly BinaryOperator Shr = new ShrOperator();
		public static readonly BinaryOperator Sar = new SarOperator();

		public static readonly BinaryOperator Shl = new ShlOperator();
		public static readonly BinaryOperator Sub = new SubOperator();
		public static readonly BinaryOperator Xor = new XorOperator();

		public static readonly BinaryOperator Cand = new CandOperator();
		public static readonly BinaryOperator Cor = new CorOperator();

		public static readonly BinaryOperator Lt = new LtOperator();
		public static readonly BinaryOperator Gt = new GtOperator();
		public static readonly BinaryOperator Le = new LeOperator();
		public static readonly BinaryOperator Ge = new GeOperator();

		public static readonly BinaryOperator Rlt = new RltOperator();
		public static readonly BinaryOperator Rgt = new RgtOperator();
		public static readonly BinaryOperator Rle = new RleOperator();
		public static readonly BinaryOperator Rge = new RgeOperator();

		public static readonly BinaryOperator Ult = new UltOperator();
		public static readonly BinaryOperator Ugt = new UgtOperator();
		public static readonly BinaryOperator Ule = new UleOperator();
		public static readonly BinaryOperator Uge = new UgeOperator();

		public static readonly BinaryOperator Eq = new EqOperator();
		public static readonly BinaryOperator Ne = new NeOperator();

		public static readonly UnaryOperator Not = new NotOperator();
		public static readonly UnaryOperator Neg = new NegateOperator();
		public static readonly UnaryOperator Comp = new ComplementOperator();
		public static readonly UnaryOperator AddrOf = new AddressOfOperator();

		public virtual Operator Invert()
		{
			throw new NotImplementedException();
		}
	}
}
