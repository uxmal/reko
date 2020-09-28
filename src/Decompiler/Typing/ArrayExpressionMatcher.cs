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
using Reko.Core.Types;
using System;

namespace Reko.Typing
{
	/// <summary>
	/// Matches and picks apart component of an array access expression.
	/// </summary>
	public class ArrayExpressionMatcher
	{
		private Constant elemSize;
        private PrimitiveType dtPointer;

		public ArrayExpressionMatcher(PrimitiveType dtPointer)
		{
            this.dtPointer = dtPointer;
		}

		public Expression ArrayPointer { get; set; }

        public Expression Index { get; set; }

			
		public Constant ElementSize
		{
			get { return elemSize; }
		}

		public bool MatchMul(BinaryExpression b)
		{
			if (b.Operator == Operator.SMul || b.Operator == Operator.UMul || b.Operator == Operator.IMul)
			{
                Expression e = b.Right;
                if (!(b.Left is Constant c))
                {
                    c = b.Right as Constant;
                    e = b.Left;
                }
                if (c != null)
				{
					elemSize = c;
					Index = e;
					return true;
				}
			}
			if (b.Operator == Operator.Shl)
			{
                if (b.Right is Constant c)
                {
                    elemSize = b.Operator.ApplyConstants(Constant.Create(b.Left.DataType, 1), c);
                    Index = b.Left;
                    return true;
                }
            }
			return false;
		}

		public bool Match(Expression e)
		{
			elemSize = null;
			Index = null;
			ArrayPointer = null;

            if (!(e is BinaryExpression b))
                return false;
            if (MatchMul(b))
            {
                ArrayPointer = Constant.Zero(b.DataType);
                return true;
            }

			// (+ x y)
			if (b.Operator == Operator.IAdd)
			{
                if (b.Left is BinaryExpression bInner)
                {
                    if (MatchMul(bInner))
                    {
                        // (+ (* i c) ptr)
                        ArrayPointer = b.Right;
                        return true;
                    }
                }
                bInner = b.Right as BinaryExpression;
				if (bInner != null)
				{
					if (MatchMul(bInner))
					{
						// (+ ptr (* i c))
						ArrayPointer = b.Left;
						return true;
					}
					if (bInner.Operator == Operator.IAdd)
					{
						// (+ x (+ a b)) 
						var bbInner = bInner.Left as BinaryExpression;
                        if (bbInner != null && MatchMul(bbInner))
                        {
                            // (+ x (+ (* i c) y)) rearranges to become
                            // (+ (* i c) (+ x y))

                            this.ArrayPointer = new BinaryExpression(
                                Operator.IAdd,
                                PrimitiveType.Create(Domain.Pointer, b.Left.DataType.BitSize),
                                b.Left,
                                bInner.Right);
                            return true;
                        }
					}
				}
			}
			return false;
		}

		public Expression Transform(Expression baseptr, DataType dtAccess)
		{
            if (baseptr != null)
            {
                ArrayPointer = new MkSequence(dtPointer, baseptr, ArrayPointer);
            }
			return new ArrayAccess(
                dtAccess, 
                ArrayPointer,
                new BinaryExpression(
                    Operator.IMul, 
                    Index.DataType,
                    Index,
                    ElementSize));
		}
	}
}
