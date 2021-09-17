#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Evaluation
{
    public class SliceSequence
    {
        private EvaluationContext ctx;
        private Expression? result;
        private Expression? eOld;

        public SliceSequence(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(Slice slice)
        {
            var e = slice.Expression;
            if (IsSequence(e, out var seq))
            {
                var bitsUsed = slice.DataType.BitSize;
                int bitoffset = 0;
                for (int i = seq.Expressions.Length - 1; i >= 0; --i)
                {
                    var elem = seq.Expressions[i];
                    var bitsElem = elem.DataType.BitSize;
                    var offset = slice.Offset - bitoffset;
                    if (0 <= offset && offset + bitsUsed <= bitsElem)
                    {
                        this.eOld = e;
                        this.result = offset == 0 && bitsUsed == bitsElem
                            ? elem
                            : new Slice(slice.DataType, elem, offset);
                        return true;
                    }
                    bitoffset += bitsElem;
                }
            }
            return false;
        }

        public Expression Transform()
        {
            ctx.RemoveExpressionUse(eOld!);
            ctx.UseExpression(result!);
            return result!;
        }

        private bool IsSequence(Expression e, out MkSequence sequence)
        {
            MkSequence? s;
            if (e is Identifier id)
            {
                s = ctx.GetDefiningExpression(id) as MkSequence;
            }
            else
            {
                s = e as MkSequence;
            }
            sequence = s!;
            return s != null;
        }
    }
}
