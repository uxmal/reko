#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
	/// Matches the pattern: (union (ptr T_1) (ptr T_2) ... (ptr T_n)) where T_1.class .. T_n.class are all structures.
	/// </summary>
	public class UnionPointersStructuresMatcher
	{
		private readonly List<EquivalenceClass> eqClasses;
		private readonly List<StructureType> strs;
        private int pointerBitSize;


		public UnionPointersStructuresMatcher()
		{
			this.eqClasses = new List<EquivalenceClass>();
			this.strs = new List<StructureType>();
		}

		public bool Match(UnionType ut)
		{
            int structureSize = 0;
			foreach (UnionAlternative a in ut.Alternatives.Values)
			{
                if (!(a.DataType is Pointer p))
                    return false;

                if (pointerBitSize == 0)
                    pointerBitSize = p.BitSize;
                else if (pointerBitSize != p.BitSize)
                    return false;

                if (!(p.Pointee is EquivalenceClass eq))
                    return false;

                if (!(eq.DataType is StructureType s))
                    return false;

                if (structureSize == 0)
                    structureSize = s.Size;
                else if (structureSize != s.Size)
                    return false;

				eqClasses.Add(eq);
				strs.Add(s);
			}
			return true;
		}

        public int PointerBitSize
        {
            get { return pointerBitSize; }
        }

		public ICollection<StructureType> Structures 
		{
			get { return strs; }
		}

		public ICollection<EquivalenceClass> EquivalenceClasses
		{
			get { return eqClasses; }
		}
	}
}
