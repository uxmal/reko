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
		private List<EquivalenceClass> eqClasses;
		private List<StructureType> strs;
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
				Pointer p = a.DataType as Pointer;
				if (p == null)
					return false;

                if (pointerBitSize == 0)
                    pointerBitSize = p.BitSize;
                else if (pointerBitSize != p.BitSize)
                    return false;

				EquivalenceClass eq = p.Pointee as EquivalenceClass;
				if (eq == null)
					return false;

				StructureType s = eq.DataType as StructureType;
				if (s == null)
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
