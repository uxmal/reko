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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.CLanguage
{
    public class CConstantEvaluator : CExpressionVisitor<object>
    {
        private IPlatform platform;
        private Dictionary<string, int> constants;

        public CConstantEvaluator(IPlatform platform, Dictionary<string, int> constants)
        {
            this.platform = platform;
            this.constants = constants;
        }
    
        public object VisitConstant(ConstExp constExp)
        {
            return constExp.Const;
        }

        public object VisitIdentifier(CIdentifier id)
        {
            return constants[id.Name];
        }

        public object VisitApplication(Application application)
        {
            throw new NotImplementedException();
        }

        public object VisitMember(MemberExpression memberExpression)
        {
            throw new NotImplementedException();
        }

        public object VisitUnary(CUnaryExpression unary)
        {
            switch (unary.Operation)
            {
            case CTokenType.Ampersand:
                return 0;
            default: throw new NotImplementedException();
            }
        }

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
            }
        }

        public object VisitAssign(AssignExpression assignExpression)
        {
            throw new NotImplementedException();
        }

        public object VisitCast(CastExpression castExpression)
        {
            return castExpression.Expression.Accept(this);
        }

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

        public object VisitIncremeent(IncrementExpression incrementExpression)
        {
            throw new NotImplementedException();
        }

        public object VisitSizeof(SizeofExpression sizeOf)
        {
            return platform.GetByteSizeFromCBasicType(CBasicType.Int);
        }
    }
}
