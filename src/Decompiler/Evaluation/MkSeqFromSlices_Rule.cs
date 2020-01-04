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
using Reko.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core;

namespace Reko.Evaluation
{
    public class MkSeqFromSlices_Rule
    {
        private EvaluationContext ctx;
        private Identifier idHi;
        private Identifier idLo;
        private Identifier idOrig;

        public MkSeqFromSlices_Rule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(MkSequence seq)
        {
            if (seq.Expressions.Length != 2)
                return false;   //$TODO: handle sequences of any length?
            this.idHi = seq.Expressions[0] as Identifier;
            this.idLo = seq.Expressions[1] as Identifier;
            if (idHi == null || idLo == null)
                return false;
            var defHi = ctx.GetDefiningExpression(idHi) as Slice;
            var defLo = ctx.GetDefiningExpression(idLo) as Cast;
            if (defHi == null || defLo == null)
                return false;

            if (defHi.Expression != defLo.Expression)
                return false;

            this.idOrig = defHi.Expression as Identifier;
            return idOrig != null;
        }

        public Expression Transform()
        {
            ctx.RemoveIdentifierUse(this.idHi);
            ctx.RemoveIdentifierUse(this.idLo);
            ctx.UseExpression(idOrig);
            return idOrig;
        }
    }
}
