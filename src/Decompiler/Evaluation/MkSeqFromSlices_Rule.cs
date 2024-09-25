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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core;

namespace Reko.Evaluation
{
    public class MkSeqFromSlices_Rule
    {
        public MkSeqFromSlices_Rule()
        {
        }

        public Expression? Match(MkSequence seq, EvaluationContext ctx)
        {
            if (seq.Expressions.Length != 2)
                return null;   //$TODO: handle sequences of any length?
            if (seq.Expressions[0] is not Identifier idHi ||
                seq.Expressions[1] is not Identifier idLo)
                return null;
            if (ctx.GetDefiningExpression(idHi) is not Slice defHi ||
                ctx.GetDefiningExpression(idLo) is not Slice defLo)
                return null;

            if (defHi.Expression != defLo.Expression)
                return null;

            var idOrig = defHi.Expression as Identifier;
            if (idOrig is null)
                return null;

            return idOrig;
        }
    }
}
