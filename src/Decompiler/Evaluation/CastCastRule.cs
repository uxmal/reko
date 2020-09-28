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
using Reko.Core.Types;

namespace Reko.Evaluation
{
    public class CastCastRule
    {
        private Cast c;
        private EvaluationContext ctx;
        private Expression origExp;
        private PrimitiveType ptC;
        private PrimitiveType ptCc;
        private PrimitiveType ptExp;

        public CastCastRule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(Cast c)
        {
            this.c = c;
            var cc = c.Expression as Cast;
            if (cc == null)
                return false;
            this.origExp = cc.Expression;

            this.ptC = c.DataType as PrimitiveType;
            this.ptCc = cc.DataType as PrimitiveType;
            this.ptExp = origExp.DataType as PrimitiveType;
            if (ptC == null || ptCc == null || ptExp == null)
                return false;

            // If the cast is identical, we don't have to do it twice.
            if (ptC == ptCc)
            {
                this.origExp = cc;
                return true;
            }
            // Only match widening / narrowing. 
            //$TODO: the Cast() class should really not appear
            // until after type analysis. It should be replaced
            // by a Convert(dtFrom, dtTo) expression which more
            // accurately models what is going on.
            if (ptC.Domain != ptCc.Domain || ptC.Domain != ptExp.Domain)
                return false;
            //$TODO: for now, only eliminate the casts if the 
            // original size == new size.
            return ptC.Size == ptExp.Size && ptC.Size <= ptCc.Size;
        }

        public Expression Transform()
        {
            return origExp;
        }
    }
}