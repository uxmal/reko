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

using Reko.Core.Code;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Typing
{
	/// <summary>
	/// Given a type variable T, maps to the traits associated with that type variable.
	/// </summary>
	public class TraitMapping : IComparer<TypeVariable>
	{
		private SortedList<TypeVariable, List<Trait>> items;
        private TypeStore store;

		public TraitMapping(TypeStore store)
		{
			items = new SortedList<TypeVariable,List<Trait>>(this);
            this.store = store;
		}

		public List<Trait> this[TypeVariable t]
		{
			get { return GetTypeTraits(t); }
		}

		public DataType AddTrait(TypeVariable t, Trait tr)
		{
            GetTypeTraits(t).Add(tr);
            return null;
		}

		public void AddTypeVar(TypeVariable t)
		{
			GetTypeTraits(t);
		}

		/// <summary>
		/// Returns a list of the traits for type t.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private List<Trait> GetTypeTraits(TypeVariable t)
		{
            List<Trait> typeTraits;
            if (!items.TryGetValue(t, out typeTraits))
			{
				typeTraits = new List<Trait>();
                items.Add(t, typeTraits);
			}
			return typeTraits;
		}

		public void Write(TextWriter tw)
		{
            var formatter = new TextFormatter(tw);
			foreach (KeyValuePair<TypeVariable,List<Trait>> de in items)
			{
                tw.Write(de.Key);
                store.WriteExpressionOf(de.Key, formatter);
                tw.WriteLine();
				foreach (Trait tr in de.Value)
				{
					tw.WriteLine("\t{0}", tr);
				}
			}
		}
		#region IComparer Members

		public int Compare(TypeVariable a, TypeVariable b)
		{
			return a.Number - b.Number;
		}

		#endregion
	}
}
