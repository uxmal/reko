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
using System;

namespace Reko.Core.Operators
{
	public class Operator
	{
		public static readonly BinaryOperator IAdd = new IAddOperator();
        public static readonly BinaryOperator ISub = new ISubOperator();
        public static readonly BinaryOperator USub = new USubOperator();
        public static readonly BinaryOperator IMul = new IMulOperator();
        public static readonly BinaryOperator SMul = new SMulOperator();
        public static readonly BinaryOperator UMul = new UMulOperator();
        public static readonly BinaryOperator SDiv = new SDivOperator();
        public static readonly BinaryOperator UDiv = new UDivOperator();


        public static readonly BinaryOperator IMod = new IModOperator();

        public static readonly BinaryOperator FAdd = new FAddOperator();
        public static readonly BinaryOperator FSub = new FSubOperator();
        public static readonly BinaryOperator FMul = new FMulOperator();
        public static readonly BinaryOperator FDiv = new FDivOperator();
        public static readonly UnaryOperator FNeg = new FNegOperator();

        public static readonly BinaryOperator And = new AndOperator();
		public static readonly BinaryOperator Or = new OrOperator();
		public static readonly BinaryOperator Shr = new ShrOperator();
		public static readonly BinaryOperator Sar = new SarOperator();

		public static readonly BinaryOperator Shl = new ShlOperator();
		public static readonly BinaryOperator Xor = new XorOperator();

		public static readonly BinaryOperator Cand = new CandOperator();
		public static readonly BinaryOperator Cor = new CorOperator();

		public static readonly ConditionalOperator Lt = new LtOperator();
		public static readonly ConditionalOperator Gt = new GtOperator();
		public static readonly ConditionalOperator Le = new LeOperator();
		public static readonly ConditionalOperator Ge = new GeOperator();

        public static readonly ConditionalOperator Feq = new ReqOperator();
        public static readonly ConditionalOperator Fne = new RneOperator();
        public static readonly ConditionalOperator Flt = new RltOperator();
		public static readonly ConditionalOperator Fgt = new RgtOperator();
		public static readonly ConditionalOperator Fle = new RleOperator();
		public static readonly ConditionalOperator Fge = new RgeOperator();

		public static readonly ConditionalOperator Ult = new UltOperator();
		public static readonly ConditionalOperator Ugt = new UgtOperator();
		public static readonly ConditionalOperator Ule = new UleOperator();
		public static readonly ConditionalOperator Uge = new UgeOperator();

		public static readonly ConditionalOperator Eq = new EqOperator();
		public static readonly ConditionalOperator Ne = new NeOperator();

		public static readonly UnaryOperator Not = new NotOperator();
		public static readonly UnaryOperator Neg = new NegateOperator();
		public static readonly UnaryOperator Comp = new ComplementOperator();
		public static readonly UnaryOperator AddrOf = new AddressOfOperator();

        public static readonly BinaryOperator Comma = new CommaOperator();

        public virtual Constant ApplyConstants(Constant c1, Constant c2)
        {
            throw new NotSupportedException();
        }

        public virtual string AsCompound()
        {
            throw new NotSupportedException($"The {this} operator can't be used in a compound assignment.");
        }

        public virtual Operator Invert()
		{
			throw new NotImplementedException();
		}
    }
}
