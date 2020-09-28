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
using Reko.Core.Code;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Evaluation
{
    public class DpbDpbRule
    {
        private EvaluationContext ctx;
        private DepositBits dpbUse;
        private Identifier idDef;
        private Identifier idSrc;

        public DpbDpbRule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(DepositBits dpb)
        {
            this.dpbUse = dpb;
            if (!(dpb.Source is Identifier idDef))
                return false;
            var stms = ctx.GetDefiningStatementClosure(idDef);
            if (stms.Count == 0)
                return false;
            var items = stms.Select(GetDpbDetails).ToList();
            var first = items[0].idSrc;
            if (items.All(i => i.idSrc != null && i.idSrc == first && i.dpbDef.BitPosition == dpbUse.BitPosition))
            {
                this.idDef = idDef;
                this.idSrc = items[0].idSrc;
                return true;
            }
            else
            {
                return false;
            }
        }

        private (DepositBits dpbDef, Identifier idSrc) GetDpbDetails(Statement stm)
        {
            if (stm.Instruction is Assignment ass &&
                ass.Src is DepositBits dpbDef &&
                dpbDef.Source is Identifier idSrc)
            {
                return (dpbDef, idSrc);
            }
            else
            {
                return (null, null);
            }
        }

        public DepositBits Transform()
        {
            ctx.RemoveIdentifierUse(idDef);
            ctx.UseExpression(idSrc);
            return new DepositBits(idSrc, dpbUse.InsertedBits, dpbUse.BitPosition);
        }
    }
}
