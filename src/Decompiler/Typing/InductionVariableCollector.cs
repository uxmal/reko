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

using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Operators;

namespace Reko.Typing
{
    /// <summary>
    /// Collects induction variables, including derived induction variables.
    /// </summary>
    public class InductionVariableCollector : ExpressionVisitorBase<LinearInductionVariable?>
    {
        private readonly Program program;

        public InductionVariableCollector(Program program)
        {
            this.program = program;
        }

        public override LinearInductionVariable? DefaultValue => null;
        
        public override LinearInductionVariable? VisitIdentifier(Identifier id)
        {
            if (!program.InductionVariables.TryGetValue(id, out var iv))
                return null;
            else 
                return iv;
        }

        public override LinearInductionVariable? VisitBinaryExpression(BinaryExpression binExp)
        {
            var ivLeft = binExp.Left.Accept(this);
            var ivRight = binExp.Right.Accept(this);
            if (ivLeft is not null)
            {
                switch (binExp.Operator.Type)
                {
                    case OperatorType.SMul:
                    case OperatorType.UMul:
                    case OperatorType.IMul:
                    case OperatorType.Shl:
                    return MergeInductionVariableConstant(ivLeft, binExp.Operator, binExp.Right as Constant);
                default:
                    return null;
                }
            }
            return null;
        }

        public override LinearInductionVariable? VisitMemoryAccess(MemoryAccess access)
        {
            return null;
        }

        public LinearInductionVariable? MergeInductionVariableConstant(LinearInductionVariable? iv, Operator op, Constant? c)
        {
            if (iv is null || c is null)
                return null;
            Constant delta = op.ApplyConstants(iv.Delta!.DataType, iv.Delta!, c);
            Constant? initial = (iv.Initial is not null) ? op.ApplyConstants(iv.Initial.DataType, iv.Initial, c) : null;
            Constant? final = (iv.Final is not null) ? op.ApplyConstants(iv.Final.DataType, iv.Final, c) : null;
            return new LinearInductionVariable(initial, delta, final, false);
        }
    }
}
