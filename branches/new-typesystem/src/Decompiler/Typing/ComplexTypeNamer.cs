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

namespace Decompiler.Typing
{
	/// <summary>
	/// Gives names to complex types.
	/// </summary>
	public class ComplexTypeNamer : DataTypeVisitor
	{
		private EquivalenceClass eq;

		public ComplexTypeNamer()
		{
		}

		public void RenameAllTypes(TypeStore store)
		{
			foreach (EquivalenceClass e in store.UsedEquivalenceClasses)
			{
				eq = e;
				if (eq.DataType != null)
					eq.DataType.Accept(this);
			}

			eq = null;
			foreach (TypeVariable tv in store.TypeVariables)
			{
				tv.DataType.Accept(this);
			}
		}

		public override void VisitStructure(StructureType str)
		{
			if (str.Name == null && eq != null)
				str.Name = eq.Name;
		}

		public override void VisitUnion(UnionType ut)
		{
			if (ut.Name == null && eq != null)
				ut.Name = eq.Name;
			int i = 0; 
			foreach (UnionAlternative a in ut.Alternatives)
			{
				a.Name = a.MakeName(i++);
			}
		}
	}
}
