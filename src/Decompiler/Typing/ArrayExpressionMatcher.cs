#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
        private readonly PrimitiveType dtPointer;
        private readonly ExpressionEmitter m;

        /// <summary>
        /// Constructs a new <see cref="ArrayExpressionMatcher"/>.
        /// </summary>
        /// <param name="dtPointer">Size of a pointer.</param>
		public ArrayExpressionMatcher(PrimitiveType dtPointer)
		{
            this.dtPointer = dtPointer;
            this.m = new ExpressionEmitter();
		}

        /// <summary>
        /// Matches the array part of an array access.
        /// </summary>
		public Expression? ArrayPointer { get; set; }

        /// <summary>
        /// Matches the index part of an array access.
        /// </summary>
        public Expression? Index { get; set; }

        /// <summary>
        /// Discovered element size of the array.
        /// </summary>
		public Constant? ElementSize { get; private set; }

        /// <summary>
        /// Matches the multiplication part of an array access.
        /// </summary>
        /// <param name="b">Binary expression to match.</param>
        /// <returns>True if the expression matches.</returns>
		public bool MatchMul(BinaryExpression b)
		{
			switch (b.Operator.Type)
            {
            case OperatorType.SMul:
            case OperatorType.UMul:
            case OperatorType.IMul:
                Expression e = b.Right;
                if (b.Left is not Constant c)
                {
                    c = (b.Right as Constant)!;
                    e = b.Left;
                }
                if (c is not null)
				{
					ElementSize = c;
					Index = e;
					return true;
				}
                break;
            case OperatorType.Shl:
                if (b.Right is Constant cShl)
                {
                    ElementSize = b.Operator.ApplyConstants(b.Left.DataType, m.Const(b.Left.DataType, 1), cShl);
                    Index = b.Left;
                    return true;
                }
                break;
            }
			return false;
		}

        /// <summary>
        /// Check if an expression is an array access expression.
        /// </summary>
        /// <param name="e">Expression to test.</param>
        /// <returns>True if <paramref name="e"/> looks like an array access expression.</returns>
		public bool Match(Expression e)
		{
			ElementSize = null;
			Index = null;
			ArrayPointer = null;

            if (e is not BinaryExpression b)
                return false;
            if (MatchMul(b))
            {
                ArrayPointer = Constant.Zero(b.DataType);
                return true;
            }

			// (+ x y)
			if (b.Operator.Type == OperatorType.IAdd)
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
                if (b.Left is Conversion conv && conv.Expression is BinaryExpression bConverted)
                {
                    if (MatchMul(bConverted))
                    {
                        // (+ (CONVERT (* i c) ...) ptr)
                        ArrayPointer = b.Right;
                        return true;
                    }
                }
                if (b.Right is BinaryExpression bInner2)
                {
                    if (MatchMul(bInner2))
                    {
                        // (+ ptr (* i c))
                        ArrayPointer = b.Left;
                        return true;
                    }
                    if (bInner2.Operator.Type == OperatorType.IAdd)
                    {
                        // (+ x (+ a b)) 
                        var bbInner = bInner2.Left as BinaryExpression;
                        if (bbInner is not null && MatchMul(bbInner))
                        {
                            // (+ x (+ (* i c) y)) rearranges to become
                            // (+ (* i c) (+ x y))

                            this.ArrayPointer = m.Bin(
                                Operator.IAdd,
                                PrimitiveType.Create(Domain.Pointer, b.Left.DataType.BitSize),
                                b.Left,
                                bInner2.Right);
                            return true;
                        }
                    }
                }
            }
			return false;
		}

        /// <summary>
        /// Transform the array access expression into an <see cref="ArrayAccess"/> expression.
        /// </summary>
        /// <param name="baseptr">Optional </param>
        /// <param name="dtAccess"></param>
        /// <returns></returns>
		public Expression Transform(Expression? baseptr, DataType dtAccess)
		{
            if (baseptr is not null)
            {
                ArrayPointer = m.SegPtr(dtPointer, baseptr, ArrayPointer!);
            }
			return m.ARef(
                dtAccess, 
                ArrayPointer!,
                m.IMul(Index!, ElementSize!));
		}
	}
}
