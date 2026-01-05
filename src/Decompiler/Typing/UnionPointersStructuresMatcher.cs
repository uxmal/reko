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

using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Typing
{
    /// <summary>
    /// Matches the pattern: <c>(union (ptr T_1) (ptr T_2) ... (ptr T_n))</c> where <c>T_1.class .. T_n.class</c> are all structures.
    /// </summary>
    public class UnionPointersStructuresMatcher
	{
		private readonly List<EquivalenceClass> eqClasses;
		private readonly List<StructureType> strs;
        private int pointerBitSize;

        /// <summary>
        /// Constructs an instance of the <see cref="UnionPointersStructuresMatcher"/> class.
        /// </summary>
        public UnionPointersStructuresMatcher()
		{
			this.eqClasses = [];
			this.strs = [];
		}

        /// <summary>
        /// Tests whether the given <see cref="UnionType"/> matches the pattern of a union of pointers to structures.
        /// </summary>
        /// <param name="ut"></param>
        /// <returns></returns>
		public bool Match(UnionType ut)
		{
            int structureSize = 0;
			foreach (UnionAlternative a in ut.Alternatives.Values)
			{
                if (a.DataType is not Pointer p)
                    return false;

                if (pointerBitSize == 0)
                    pointerBitSize = p.BitSize;
                else if (pointerBitSize != p.BitSize)
                    return false;

                if (p.Pointee is not EquivalenceClass eq)
                    return false;

                if (eq.DataType is not StructureType s)
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

        /// <summary>
        /// The bit size of the pointers in the union.
        /// </summary>
        public int PointerBitSize
        {
            get { return pointerBitSize; }
        }

        /// <summary>
        /// The various structures that were matched in the union.
        /// </summary>
		public ICollection<StructureType> Structures 
		{
			get { return strs; }
		}

        /// <summary>
        /// The equivalence classes that were matched in the union.
        /// </summary>
		public ICollection<EquivalenceClass> EquivalenceClasses
		{
			get { return eqClasses; }
		}
	}
}
