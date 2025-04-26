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

using System;
using System.Collections.Generic;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Evaluate a C expression to a constant value.
    /// </summary>
    public class CConstantEvaluator : CExpressionVisitor<object>
    {
        private IPlatform platform;
        private Dictionary<string, int> constants;

        /// <summary>
        /// Constructs an instance of the <see cref="CConstantEvaluator"/> class.
        /// </summary>
        /// <param name="platform">Operating environment in which to evaluate the 
        /// expression.</param>
        /// <param name="constants">Map from the names of known constants to their values.
        /// </param>
        public CConstantEvaluator(IPlatform platform, Dictionary<string, int> constants)
        {
            this.platform = platform;
            this.constants = constants;
        }
    
        /// <summary>
        /// Evaluates a C constant.
        /// </summary>
        /// <param name="constExp"></param>
        /// <returns>The constant's value.</returns>
        public object VisitConstant(ConstExp constExp)
        {
            return constExp.Const;
        }

        /// <summary>
        /// Evaluates a C identifier.
        /// </summary>
        /// <param name="id">Identifier whose value is to be evaluated.</param>
        /// <returns>The current value of that identifier.</returns>
        public object VisitIdentifier(CIdentifier id)
        {
            return constants[id.Name];
        }

        /// <summary>
        /// Evaluates a C function application.
        /// </summary>
        /// <param name="application"></param>
        /// <returns>The result of evaluating the function.</returns>
        public object VisitApplication(Application application)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object VisitArrayAccess(CArrayAccess aref)
        {
            throw new NotFiniteNumberException();
        }

        /// <inheritdoc/>
        public object VisitMember(MemberExpression memberExpression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates a C unary expression.
        /// </summary>
        /// <param name="unary">C unary expression to evaluate.</param>
        /// <returns>The evaluated expression.</returns>
        public object VisitUnary(CUnaryExpression unary)
        {
            switch (unary.Operation)
            {
            case CTokenType.Ampersand:
                return 0;
            case CTokenType.Tilde:
                return ~(int) unary.Expression.Accept(this);
            default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Evaluates a C binary expression.
        /// </summary>
        /// <param name="bin">The C binary expression to evaluate.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object VisitBinary(CBinaryExpression bin)
        {
            var left = bin.Left.Accept(this);
            var right = bin.Right.Accept(this);
            switch (bin.Operation)
            {
            default:
                throw new NotImplementedException("Operation " + bin.Operation);
            case CTokenType.Ampersand:
                return (int) left & (int) right;
            case CTokenType.Eq:
                return (int) left == (int) right;
            case CTokenType.Minus:
                return (int) left - (int) right;
            case CTokenType.Pipe:
                return (int) left | (int) right;
            case CTokenType.Plus:
                return (int) left + (int) right;
            case CTokenType.Shl:
                return (int) left << (int) right;
            case CTokenType.Shr:
                return (int) left >> (int) right;
            case CTokenType.Star:
                return (int) left * (int) right;
            case CTokenType.Slash:
                return (int) left / (int) right;
            }
        }

        /// <inheritdoc/>
        public object VisitAssign(AssignExpression assignExpression)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object VisitCast(CastExpression castExpression)
        {
            return castExpression.Expression.Accept(this);
        }

        /// <inheritdoc/>
        public object VisitConditional(ConditionalExpression cond)
        {
            if (Convert.ToBoolean(cond.Condition.Accept(this)))
            {
                return cond.Consequent.Accept(this);
            }
            else
            {
                return cond.Alternative.Accept(this);
            }
        }

        /// <inheritdoc/>
        public object VisitIncrement(IncrementExpression incrementExpression)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object VisitSizeof(SizeofExpression sizeOf)
        {
            var bits = platform.GetBitSizeFromCBasicType(CBasicType.Int);
            var granularity = platform.Architecture.MemoryGranularity;
            return (bits + (granularity - 1)) / granularity;
        }
    }
}
