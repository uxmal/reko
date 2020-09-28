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
using Reko.Core.Pascal;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.MacOS.Classic
{
    public class ConstantEvaluator : IPascalSyntaxVisitor<Constant>
    {
        private IDictionary<string, Exp> constDefinitions;
        private Dictionary<string, Constant> evaluated;

        public ConstantEvaluator(IDictionary<string, Exp> constDefinitions, Dictionary<string, Constant> evaluated)
        {
            this.constDefinitions = constDefinitions;
            this.evaluated = evaluated;
        }

        public Constant VisitArrayType(Core.Pascal.Array array)
        {
            throw new NotImplementedException();
        }

        public Constant VisitBinExp(BinExp binExp)
        {
            var cLeft = binExp.Left.Accept(this);
            if (!cLeft.IsValid)
                return cLeft;
            var cRight = binExp.Right.Accept(this);
            if (!cRight.IsValid)
                return cRight;
            if (binExp.Op == TokenType.Minus)
            {
                return Operator.ISub.ApplyConstants(cLeft, cRight);
            }
            throw new NotImplementedException();
        }

        public Constant VisitBooleanLiteral(BooleanLiteral boolLiteral)
        {
            return Constant.Bool(boolLiteral.Value);
        }

        public Constant VisitCallableDeclaration(CallableDeclaration cd)
        {
            throw new NotImplementedException();
        }

        public Constant VisitConstantDeclaration(ConstantDeclaration cd)
        {
            throw new NotImplementedException();
        }

        public Constant VisitEnumType(Core.Pascal.EnumType enumType)
        {
            throw new NotImplementedException();
        }

        public Constant VisitFile(Core.Pascal.File file)
        {
            throw new NotImplementedException();
        }

        public Constant VisitIdentifier(Id id)
        {
            if (!this.evaluated.TryGetValue(id.Name, out Constant c))
            {
                this.evaluated.Add(id.Name, Constant.Invalid);
                if (constDefinitions.TryGetValue(id.Name, out Exp def))
                {
                    c = def.Accept(this);
                    this.evaluated[id.Name] = c;
                    return c;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Undefined constant '{0}'.", id.Name));
                }
            }
            else
            {
                if (c.IsValid)
                    return c;
                else
                    throw new InvalidOperationException(string.Format("Recursive constant definition of {0}.", id.Name));
            }
            throw new NotImplementedException();
        }

        public Constant VisitInlineMachineCode(InlineMachineCode code)
        {
            throw new NotImplementedException();
        }

        public Constant VisitNumericLiteral(NumericLiteral number)
        {
            return Constant.Create(PrimitiveType.Int64, number.Value);
        }

        public Constant VisitPointerType(Core.Pascal.Pointer pointer)
        {
            throw new NotImplementedException();
        }

        public Constant VisitPrimitiveType(Primitive primitive)
        {
            throw new NotImplementedException();
        }

        public Constant VisitRangeType(RangeType rangeType)
        {
            throw new NotImplementedException();
        }

        public Constant VisitRealLiteral(RealLiteral realLiteral)
        {
            return Constant.Real64(realLiteral.Value);
        }

        public Constant VisitRecord(Record record)
        {
            throw new NotImplementedException();
        }

        public Constant VisitSetType(SetType setType)
        {
            throw new NotImplementedException();
        }

        public Constant VisitStringLiteral(StringLiteral str)
        {
            return Constant.String(str.String, Core.Types.StringType.LengthPrefixedStringType(PrimitiveType.Char, PrimitiveType.Byte));
        }

        public Constant VisitStringType(Core.Pascal.StringType strType)
        {
            throw new NotImplementedException();
        }

        public Constant VisitTypeDeclaration(TypeDeclaration td)
        {
            throw new NotImplementedException();
        }

        public Constant VisitTypeReference(Core.Pascal.TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public Constant VisitUnaryExp(UnaryExp unaryExp)
        {
            var c = unaryExp.exp.Accept(this);
            if (!c.IsValid)
                return c;
            if (unaryExp.op == TokenType.Minus)
            {
                return Operator.Neg.ApplyConstant(c);
            }
            throw new NotImplementedException();
        }
    }
}
