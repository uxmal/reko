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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Lib
{
    public class Dijkstra<T>
    {
        public Dictionary<T, double> dist;
        public Dictionary<T, T> prev;

        /// <summary>
        /// Implementation of the Dijkstra's shortest-path algorithm. Given a <paramref name="graph"/>
        /// it computes the shortest paths that can be reached starting at <paramref name="source"/>.
        /// </summary>
        /// <param name="Graph"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static Dijkstra<T> ShortestPath(DirectedGraph<T> Graph, T source, Func<T, T, double> weight)
        {
            var Q = new FibonacciHeap<double, T>();
            var q = new HashSet<T>(Graph.Nodes);
            var self = new Dijkstra<T>();
            self.dist = Graph.Nodes.ToDictionary(K => K, V => double.PositiveInfinity);
            self.prev = new Dictionary<T, T>();

            self.dist[source] = 0;                        // Distance from source to source


            foreach (var v in Graph.Nodes)
            {
                Q.insert(v, self.dist[v]);
            }

            while (!Q.isEmpty())
            {
                var u = Q.removeMin();                            // Remove and return best vertex
                q.Remove(u);
                foreach (var v in Graph.Successors(u))
                {
                    // only v that is still in Q
                    var alt = self.dist[u] + weight(u, v);
                    if (alt < self.dist[v])
                    {
                        self.dist[v] = alt;
                        self.prev[v] = u;
                        Q.decreaseKey(v, alt);
                    }
                }
            }
            return self;
        }

        public List<T> GetPath(T destination)
        {
            var path = new List<T>();
            T p;
            while (prev.TryGetValue(destination, out p))
            {
                path.Add(p);
                destination = p;
            }
            path.Reverse();
            return path;
        }
    }
}
