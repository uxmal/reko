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
		public static readonly BinaryOperator add = new AddOperator();
		public static readonly BinaryOperator and = new AndOperator();
		public static readonly BinaryOperator divs = new DivsOperator();
		public static readonly BinaryOperator divu = new DivuOperator();
		public static readonly BinaryOperator mod = new ModOperator();
		public static readonly BinaryOperator mul = new MulOperator();
		public static readonly BinaryOperator muls = new MulsOperator();
		public static readonly BinaryOperator mulu = new MuluOperator();

		public static readonly BinaryOperator or = new OrOperator();
		public static readonly BinaryOperator shr = new ShrOperator();
		public static readonly BinaryOperator sar = new SarOperator();

		public static readonly BinaryOperator shl = new ShlOperator();
		public static readonly BinaryOperator sub = new SubOperator();
		public static readonly BinaryOperator xor = new XorOperator();

		public static readonly BinaryOperator cand = new CandOperator();
		public static readonly BinaryOperator cor = new CorOperator();

		public static readonly BinaryOperator lt = new LtOperator();
		public static readonly BinaryOperator gt = new GtOperator();
		public static readonly BinaryOperator le = new LeOperator();
		public static readonly BinaryOperator ge = new GeOperator();

		public static readonly BinaryOperator rlt = new RltOperator();
		public static readonly BinaryOperator rgt = new RgtOperator();
		public static readonly BinaryOperator rle = new RleOperator();
		public static readonly BinaryOperator rge = new RgeOperator();

		public static readonly BinaryOperator ult = new UltOperator();
		public static readonly BinaryOperator ugt = new UgtOperator();
		public static readonly BinaryOperator ule = new UleOperator();
		public static readonly BinaryOperator uge = new UgeOperator();
		public static readonly BinaryOperator eq = new EqOperator();
		public static readonly BinaryOperator ne = new NeOperator();
		public static readonly UnaryOperator not = new NotOperator();
		public static readonly UnaryOperator neg = new NegateOperator();
		public static readonly UnaryOperator comp = new ComplementOperator();
		public static readonly UnaryOperator addrOf = new AddressOfOperator();

		public virtual Operator Invert()
		{
			throw new NotImplementedException();
		}
	}
}
