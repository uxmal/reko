/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.Collections;	

namespace Decompiler.Typing
{
	/// <summary>
	/// Matches the pattern: (union (ptr T_1) (ptr T_2) ... (ptr T_n)) where T_1.class .. T_n.class are all structures.
	/// </summary>

	/// </summary>
	public class UnionPointersStructuresMatcher
	{
		private ArrayList eqClasses;
		private ArrayList strs;

		public UnionPointersStructuresMatcher()
		{
			this.eqClasses = new ArrayList();
			this.strs = new ArrayList();
		}

		public bool Match(UnionType ut)
		{
			foreach (UnionAlternative a in ut.Alternatives)
			{
				Pointer p = a.DataType as Pointer;
				if (p == null)
					return false;
				EquivalenceClass eq = p.Pointee as EquivalenceClass;
				if (eq == null)
					return false;

				StructureType s = eq.DataType as StructureType;
				if (s == null)
					return false;

				eqClasses.Add(eq);
				strs.Add(s);
			}
			return true;
		}

		public ICollection Structures 
		{
			get { return strs; }
		}

		public ICollection EquivalenceClasses
		{
			get { return eqClasses; }
		}
	}
}
