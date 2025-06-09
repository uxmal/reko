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
	/// Traverses all types and replaces references to type variables with 
	/// references to their equivalence class representatives.
	/// </summary>
	public class TypeVariableReplacer : DataTypeTransformer
	{
		private readonly TypeStore store;
        private readonly HashSet<DataType> visitedTypes;

		public TypeVariableReplacer(TypeStore store)
		{
			this.store = store;
            this.visitedTypes = new HashSet<DataType>();
		}

		/// <summary>
		/// Replaces each type variable occurrence T with its equivalence class eq(T). 
		/// </summary>
		public void ReplaceTypeVariables()
		{
			var visited = new HashSet<EquivalenceClass>();
			foreach (TypeVariable tv in store.TypeVariables)
			{
				EquivalenceClass eq = tv.Class;
				if (!visited.Contains(eq))
				{
					visited.Add(eq);
					if (eq.DataType is not null)
					{
						eq.DataType = eq.DataType.Accept(this);
					} 
				}
			}

			foreach (TypeVariable tv in store.TypeVariables)
			{
				tv.DataType = tv.Class;
			}
		}

		public override DataType VisitTypeVariable(TypeVariable tv)
		{
			return tv.Class;
		}

        public override DataType VisitStructure(StructureType str)
        {
            if (visitedTypes.Contains(str))
                return str;
            visitedTypes.Add(str);
            return base.VisitStructure(str);
        }

        public override DataType VisitUnion(UnionType ut)
        {
            if (visitedTypes.Contains(ut))
                return ut;
            visitedTypes.Add(ut);
            return base.VisitUnion(ut);
        }
    }
}
