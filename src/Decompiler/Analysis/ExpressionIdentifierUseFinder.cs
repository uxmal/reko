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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// Finds all used identifiers in an expression.
    /// </summary>
    public class ExpressionIdentifierUseFinder : ExpressionVisitorBase
    {
        private SsaIdentifierCollection ssaIds;
        private List<Identifier> identifiers;

        public ExpressionIdentifierUseFinder(SsaIdentifierCollection ssaIds)
        {
            this.ssaIds = ssaIds;
            this.identifiers = new List<Identifier>();
        }

        public static List<Identifier> Find(SsaIdentifierCollection ssaIds, Expression exp)
        {
            var inst = new ExpressionIdentifierUseFinder(ssaIds);
            exp.Accept(inst);
            return inst.identifiers;
        }

        public override void VisitIdentifier(Identifier id)
        {
            identifiers.Add(id);
        }

        public override void VisitOutArgument(OutArgument outArg)
        {
            if (outArg.Expression is Identifier)
                return;
            outArg.Expression.Accept(this);
        }
    }
}
