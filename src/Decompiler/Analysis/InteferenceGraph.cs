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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Analysis
{
    /// <summary>
    /// Keeps track of what identifier interfere with each other.
    /// Because the graph is sparse, it's implemented as a set of 
    /// edges, rather than a bit matrix.
    /// </summary>
	public class InterferenceGraph
	{
        private readonly Dictionary<Interference, Interference> intf;

        /// <summary>
        /// Creates an empty interference graph.
        /// </summary>
		public InterferenceGraph()
		{
            intf = [];
		}

        /// <summary>
        /// Adds an interference edge between two identifiers.
        /// </summary>
        /// <param name="id1">The first identifier.</param>
        /// <param name="id2">The second identifier.</param>
		public void Add(Identifier id1, Identifier id2)
		{
			if (id1 != id2)
			{
				var i = new Interference(id1, id2);
				if (!intf.ContainsKey(i))
					intf.Add(i, i);
			}
		}

        /// <summary>
        /// Determine whether two identifiers interfere.
        /// </summary>
		public bool Interfere(Identifier id1, Identifier id2)
		{
			var i = new Interference(id1, id2);
			return intf.ContainsKey(i);
		}

        /// <summary>
        /// Writes a human-readable representation of the interference graph.
        /// </summary>
        /// <param name="writer"></param>
        public void Write(TextWriter writer)
		{
			var sl = new SortedList<Interference,Interference>(intf);
			foreach (Interference i in sl.Values)
			{
				writer.WriteLine("{0} interferes with {1}", i.Identifier1, i.Identifier2);
			}
		}
	}
}
