#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Hll.Pascal;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Environments.MacOS.Classic
{
    /// <summary>
    /// Evaluates a Pascal expression to a Reko IR constant.
    /// </summary>
    public class ConstantEvaluator : IPascalSyntaxVisitor<Expression>
    {
        private IDictionary<string, Exp> constDefinitions;
        private Dictionary<string, Expression> evaluated;

        public ConstantEvaluator(IDictionary<string, Exp> constDefinitions, Dictionary<string, Expression> evaluated)
        {
            this.constDefinitions = constDefinitions;
            this.evaluated = evaluated;
        }

        public Expression VisitArrayType(Core.Hll.Pascal.Array array)
        {
            throw new NotImplementedException();
        }

        public Expression VisitBinExp(BinExp binExp)
        {
            var eLeft = binExp.Left.Accept(this);
            if (eLeft is not Constant cLeft)    //$TODO: string concatenation?
                return InvalidConstant.Create(eLeft.DataType);
            if (!cLeft.IsValid)
                return cLeft;
            var eRight = binExp.Right.Accept(this);
            if (eRight is not Constant cRight)
                return InvalidConstant.Create(eLeft.DataType);  // eLeft is intentional to handle shifts
            if (!cRight.IsValid)
                return InvalidConstant.Create(eLeft.DataType);  // eLeft is intentional to handle shifts
            return binExp.Op switch
            {
                TokenType.Plus => Operator.ISub.ApplyConstants(cLeft.DataType, cLeft, cRight),
                TokenType.Minus => Operator.ISub.ApplyConstants(cLeft.DataType, cLeft, cRight),
                TokenType.Star => Operator.IMul.ApplyConstants(cLeft.DataType, cLeft, cRight),
                TokenType.Slash => Operator.FDiv.ApplyConstants(cLeft.DataType, cLeft, cRight),
                _ => throw new NotImplementedException(),
            };
        }

        public Expression VisitBooleanLiteral(BooleanLiteral boolLiteral)
        {
            return Constant.Bool(boolLiteral.Value);
        }

        public Expression VisitCallableDeclaration(CallableDeclaration cd)
        {
            throw new NotImplementedException();
        }

        public Expression VisitCallableType(CallableType ct)
        {
            throw new NotImplementedException();
        }

        public Expression VisitConstantDeclaration(ConstantDeclaration cd)
        {
            throw new NotImplementedException();
        }

        public Expression VisitEnumType(Core.Hll.Pascal.EnumType enumType)
        {
            throw new NotImplementedException();
        }

        public Expression VisitFile(Core.Hll.Pascal.File file)
        {
            throw new NotImplementedException();
        }

        public Expression VisitIdentifier(Id id)
        {
            if (!this.evaluated.TryGetValue(id.Name, out Expression? e))
            {
                this.evaluated.Add(id.Name, InvalidConstant.Create(PrimitiveType.Word32));
                if (constDefinitions.TryGetValue(id.Name, out Exp? def))
                {
                    e = def.Accept(this);
                    this.evaluated[id.Name] = e;
                    return e;
                }
                else
                {
                    throw new InvalidOperationException($"Undefined constant '{id.Name}'.");
                }
            }
            else
            {
                if (e is Constant c && c.IsValid)
                    return e;
                else if (e is StringConstant)
                    return e;
                else
                    throw new InvalidOperationException($"Recursive constant definition of {id.Name}.");
            }
            throw new NotImplementedException();
        }

        public Expression VisitInlineMachineCode(InlineMachineCode code)
        {
            throw new NotImplementedException();
        }

        public Expression VisitNumericLiteral(NumericLiteral number)
        {
            return Constant.Create(PrimitiveType.Int64, number.Value);
        }
        public Expression VisitPointerType(Core.Hll.Pascal.Pointer pointer)
        {
            throw new NotImplementedException();
        }

        public Expression VisitObject(Core.Hll.Pascal.ObjectType pointer)
        {
            throw new NotImplementedException();
        }

        public Expression VisitPrimitiveType(Primitive primitive)
        {
            throw new NotImplementedException();
        }

        public Expression VisitRangeType(RangeType rangeType)
        {
            throw new NotImplementedException();
        }

        public Expression VisitRealLiteral(RealLiteral realLiteral)
        {
            return Constant.Real64(realLiteral.Value);
        }

        public Expression VisitRecord(Record record)
        {
            throw new NotImplementedException();
        }

        public Expression VisitSetType(SetType setType)
        {
            throw new NotImplementedException();
        }

        public Expression VisitStringLiteral(StringLiteral str)
        {
            return Constant.String(str.String, Core.Types.StringType.LengthPrefixedStringType(PrimitiveType.Char, PrimitiveType.Byte));
        }

        public Expression VisitStringType(Core.Hll.Pascal.StringType strType)
        {
            throw new NotImplementedException();
        }

        public Expression VisitTypeDeclaration(TypeDeclaration td)
        {
            throw new NotImplementedException();
        }

        public Expression VisitTypeReference(Core.Hll.Pascal.TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public Expression VisitUnaryExp(UnaryExp unaryExp)
        {
            var e = unaryExp.exp.Accept(this);
            if (e is not Constant c)
                return InvalidConstant.Create(e.DataType);
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
