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

using Reko.Core.Expressions;
using System.Collections.Generic;

namespace Reko.Analysis
{
    /// <summary>
    /// Finds all used identifiers in an expression.
    /// </summary>
    public class ExpressionIdentifierUseFinder : ExpressionVisitorBase
    {
        private readonly List<Identifier> identifiers;

        /// <summary>
        /// Constructs a new instance of <see cref="ExpressionIdentifierUseFinder"/>.
        /// </summary>
        public ExpressionIdentifierUseFinder()
        {
            this.identifiers = [];
        }

        /// <summary>
        /// Given an expression, <paramref name="exp"/>, finds all identifiers used in it.
        /// </summary>
        /// <param name="exp">Expression to analyze.</param>
        /// <returns>A list of the <see cref="Identifier"/>s found in <paramref name="exp"/>.
        /// </returns>
        public static List<Identifier> Find(Expression exp)
        {
            var inst = new ExpressionIdentifierUseFinder();
            exp.Accept(inst);
            return inst.identifiers;
        }

        /// <inheritdoc/>
        public override void VisitIdentifier(Identifier id)
        {
            identifiers.Add(id);
        }

        /// <inheritdoc/>
        public override void VisitOutArgument(OutArgument outArg)
        {
            if (outArg.Expression is Identifier)
                return;
            outArg.Expression.Accept(this);
        }
    }
}
