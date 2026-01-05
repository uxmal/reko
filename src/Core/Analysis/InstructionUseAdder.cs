#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
    /// This class visits an expression, and for each encountered <see cref="Identifier"/>, adds 
    /// a use to the corresponding <see cref="SsaIdentifier"/>.
    /// </summary>
    public class InstructionUseAdder : InstructionUseVisitorBase
    {
        private readonly Statement user;
        private readonly SsaIdentifierCollection ssaIds;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="user"><see cref="Statement"/> that will recorded as using 
        /// the identifiers in the expression.</param>
        /// <param name="ssaIds">The <see cref="SsaIdentifierCollection"/> affected
        /// by these changes.</param>
        public InstructionUseAdder(Statement user, SsaIdentifierCollection ssaIds)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.ssaIds = ssaIds;
        }

        /// <summary>
        /// When an <see cref="Identifier"/> is encountered, add a use to its
        /// corresponding <see cref="SsaIdentifier"/>.
        /// </summary>
        /// <param name="id">Identified that was visited.</param>
        protected override void UseIdentifier(Identifier id)
        {
            ssaIds[id].Uses.Add(user);
        }
    }
}
