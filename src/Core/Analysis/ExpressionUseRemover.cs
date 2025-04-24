#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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

namespace Reko.Core.Analysis
{
    /// <summary>
    /// This class visits an <see cref="Expression"/> tree, and when
    /// encountering <see cref="Identifier"/>, removes their use from
    /// the SSA state.
    /// </summary>
    public class ExpressionUseRemover : ExpressionVisitorBase
    {
        private readonly Statement user;
        private readonly SsaIdentifierCollection ssaIds;

        /// <summary>
        /// Creates a new <see cref="ExpressionUseRemover"/> instance.
        /// </summary>
        /// <param name="user">Statement from which the expressions are being removed.</param>
        /// <param name="ssaIds"><see cref="SsaIdentifierCollection"/> to use when 
        /// adjusting <see cref="SsaIdentifier"/> usages.
        /// </param>
        public ExpressionUseRemover(Statement user, SsaIdentifierCollection ssaIds)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.ssaIds = ssaIds;
        }

        /// <summary>
        /// Removes a single use of the identifier <paramref name="id"/> from the
        /// SSA state.
        /// </summary>
        /// <param name="id">Identifier for the SSA id, one of whose references
        /// is to be removed.</param>
        public override void VisitIdentifier(Identifier id)
        {
            ssaIds[id].Uses.Remove(user);
        }
    }
}
