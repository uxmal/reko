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
    public class SelfDpbRule
    {
        private EvaluationContext ctx;
        private Identifier e;

        public SelfDpbRule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(DepositBits dpb)
        {
            if (dpb.BitPosition != 0)
                return false;
            this.e = dpb.Source as Identifier;
            if (e == null)
                return false;
            var c = dpb.InsertedBits as Cast;
            if (c == null)
                return false;
            return e == c.Expression;
        }

        public Expression Transform()
        {
            ctx.RemoveIdentifierUse(e);
            return e;
        }
    }
}
