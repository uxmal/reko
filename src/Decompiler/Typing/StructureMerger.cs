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

using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Typing
{
	/// <summary>
	/// Given an array of structures and their corresponding TypeVars, merges
    /// them and adjusts equivalence classes accordingly.
	/// </summary>
	public class StructureMerger
	{
        private readonly ICollection<StructureType> structures;
        private readonly ICollection<EquivalenceClass> eqClasses;
		private readonly EquivalenceClass? eqMin;

        public StructureMerger(ICollection<StructureType> structures, ICollection<EquivalenceClass> eqClasses)
		{
			this.structures = structures;
			this.eqClasses = eqClasses;
			foreach (EquivalenceClass eq in eqClasses)
			{
				if (eqMin is null)
					eqMin = eq;
				else if (eq.Number < eqMin.Number)
					eqMin = eq;
			}
		}

        public EquivalenceClass MergedClass
        {
            get { return eqMin!; }
        }

		// T_1 --> C_1 --> S_1
		// T_2 --> C_2 --> S_2
		// T_3 --> C_3 --> S_3

		// T_1 --> C_1 --> S_New
		// T_2 --> C_1
		// T_3 --> C_1
        public void Merge()
        {
            Unifier un = new Unifier(new TypeFactory());
            DataType? dt = null;
            foreach (StructureType str in structures)
            {
                dt = un.Unify(dt, str);
            }
            StructureType strNew = (StructureType) dt!;
            eqMin!.DataType = strNew;
        }
	}
}
