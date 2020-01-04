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
        private Expression result;
        private Expression eOld;

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
                int bitoffset = seq.DataType.BitSize;
                foreach (var elem in seq.Expressions)
                {
                    var bitsElem = elem.DataType.BitSize;
                    bitoffset -= bitsElem;
                    if (slice.Offset == bitoffset && bitsElem == bitsUsed)
                    {
                        this.eOld = e;
                        this.result = elem;
                        return true;
                    }
                }
            }
            return false;
        }

        public Expression Transform()
        {
            ctx.RemoveExpressionUse(eOld);
            ctx.UseExpression(result);
            return result;
        }

        private bool IsSequence(Expression e, out MkSequence sequence)
        {
            if (e is Identifier id)
            {
                sequence = ctx.GetDefiningExpression(id) as MkSequence;
            }
            else
            {
                sequence = e as MkSequence;
            }
            return sequence != null;
        }
    }
}
