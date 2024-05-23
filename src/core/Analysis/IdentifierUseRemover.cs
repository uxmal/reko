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

using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using System;

namespace Reko.Core.Analysis
{
    internal class IdentifierUseRemover : InstructionUseVisitorBase
    {
        private readonly Statement stm;
        private readonly SsaIdentifierCollection identifiers;

        public IdentifierUseRemover(Statement stm, SsaIdentifierCollection identifiers)
        {
            this.stm = stm;
            this.identifiers = identifiers;
        }

        public static void Remove(Statement stm, SsaIdentifierCollection identifiers)
        {
            var iur = new IdentifierUseRemover(stm, identifiers);
            stm.Instruction.Accept(iur);
        }

        protected override void UseIdentifier(Identifier id)
        {
            if (identifiers.TryGetValue(id, out SsaIdentifier? sid))
                sid.Uses.RemoveAll(u => u == this.stm);
        }
    }
}