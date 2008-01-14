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
using System.IO;

namespace Decompiler.UnitTests.Typing
{
	/// <summary>
	/// Given a type variable T, maps to the traits associated with that type variable.
	/// </summary>
	public class TraitMapping : IComparer, IEnumerable
	{
		private SortedList items;
		
		public TraitMapping()
		{
			items = new SortedList(this);
		}

		public ArrayList this[TypeVariable t]
		{
			get { return GetTypeTraits(t); }
		}

		public void AddTrait(TypeVariable t, Trait tr)
		{
			ArrayList traits = GetTypeTraits(t);
			traits.Add(tr);
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
		private ArrayList GetTypeTraits(TypeVariable t)
		{
			ArrayList typeTraits = (ArrayList) items[t];
			if (typeTraits == null)
			{
				typeTraits = new ArrayList();
				items[t] = typeTraits;
			}
			return typeTraits;
		}

		public void Write(TextWriter tw)
		{
			foreach (DictionaryEntry de in items)
			{
				TypeVariable t = (TypeVariable) de.Key;
				tw.WriteLine("{0} (in {1})", t, t.Expression);
				
				foreach (Trait tr in GetTypeTraits(t))
				{
					tw.WriteLine("\t{0}", tr);
				}
			}
		}
		#region IComparer Members

		public int Compare(object x, object y)
		{
			TypeVariable a = (TypeVariable) x;
			TypeVariable b = (TypeVariable) y;
			return a.Number - b.Number;
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return items.GetEnumerator();
		}

		#endregion
	}
}
