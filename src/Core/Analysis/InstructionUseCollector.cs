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

using Reko.Core;
using Reko.Core.Expressions;
using System.Collections.Generic;

namespace Reko.Core.Analysis
{
    /// <summary>
    /// This class visits a <see cref="Statement"/> and all of its contained 
    /// <see cref="Expression"/>s, counting the occurrences of each <see cref="Identifier"/>.
    /// </summary>
    public class InstructionUseCollector : InstructionUseVisitorBase
    {
        private readonly IDictionary<Identifier, int> idMap;

        /// <summary>
        /// Constructs an instance of this class.
        /// </summary>
        public InstructionUseCollector()
        {
            this.idMap = new Dictionary<Identifier, int>();
        }

        /// <summary>
        /// For a given <see cref="Statement"/>, returns all the <see cref="Identifier"/>s
        /// referenced within, as well the number of times each identifier occurred.
        /// </summary>
        /// <param name="stm">Statement to analyze.</param>
        /// <returns>A dictionary associating each <see cref="Identifier"/> with the 
        /// number of times it occurs in the statement.
        /// </returns>
        public IDictionary<Identifier, int> CollectUses(Statement stm)
        {
            idMap.Clear();
            stm.Instruction.Accept(this);
            return idMap;
        }

        /// <summary>
        /// Increments the count of the given identifier in the <see cref="idMap"/>.
        /// </summary>
        /// <param name="id"></param>
        protected override void UseIdentifier(Identifier id)
        {
            if (idMap.ContainsKey(id))
                idMap[id]++;
            else
                idMap.Add(id, 1);
        }
    }
}
