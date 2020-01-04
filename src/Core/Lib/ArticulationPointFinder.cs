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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Lib
{
    // Impleented after pseudocode on https://en.wikipedia.org/wiki/Biconnected_component
    public class ArticulationPointFinder<T>
    {
        DirectedGraph<T> graph;
        private HashSet<T> aps;
        private Dictionary<T, int> depth;
        private Dictionary<T, int> low;
        private Dictionary<T, T> parent;
        private HashSet<T> visited;

        private void GetArticulationPoints(T i, int d)
        {
            visited.Add(i);
            depth[i] = d;
            low[i] = d;
            int childCount = 0;
            bool isArticulation = false;
            foreach (var ni in graph.Successors(i).Concat(graph.Predecessors(i)))
            {
                if (!visited.Contains(ni))
                {
                    parent[ni] = i;
                    GetArticulationPoints(ni, d + 1);
                    ++childCount;
                    if (low[ni] >= depth[i])
                        isArticulation = true;
                    low[i] = Math.Min(low[i], low[ni]);
                }
                else if (!ni.Equals(parent[i]))
                {
                    low[i] = Math.Min(low[i], depth[ni]);
                }
            }
            if (!parent.ContainsKey(i))
            {
                if (childCount > 1)
                    aps.Add(i);
            }
            else if (isArticulation)
            {
                aps.Add(i);
            }
        }

        public HashSet<T> FindArticulationPoints(DirectedGraph<T> graph, IEnumerable<T> nodes)
        {
            this.graph = graph;
            this.visited = new HashSet<T>();
            this.depth = new Dictionary<T, int>();
            this.low = new Dictionary<T, int>();
            this.parent = new Dictionary<T, T>();
            this.aps = new HashSet<T>();

            // Call the recursive helper function to find articulation
            // points in DFS tree rooted with vertex 'i'
            foreach (var i in nodes)
            {
                if (!visited.Contains(i))
                {
                    GetArticulationPoints(i, 0);
                }
            }
            return aps;
        }
    }
}
