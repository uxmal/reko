#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Evaluation
{
    public class DpbDpbRule
    {
        private EvaluationContext ctx;
        private DepositBits dpbDef;
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
            if (!dpb.Source.As(out idDef))
                return false;
            var expDef = ctx.GetDefiningExpression(idDef);
            if (expDef == null)
                return false;
            if (!expDef.As(out dpbDef))
                return false;
            if (!dpbDef.Source.As(out idSrc))
                return false;
            return
                dpbDef.BitPosition == dpbUse.BitPosition;
        }

        public DepositBits Transform()
        {
            ctx.RemoveIdentifierUse(idDef);
            ctx.UseExpression(idSrc);
            return new DepositBits(idSrc, dpbUse.InsertedBits, dpbUse.BitPosition);
        }
    }
}
