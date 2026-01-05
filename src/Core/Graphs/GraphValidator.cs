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

using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Graphs;

/// <summary>
/// Validates a directed graph, ensuring that successor and predecessor edges
/// are consistent.
/// </summary>
public class GraphValidator
{
    /// <summary>
    /// Checks if the directed graph is consistent.
    /// </summary>
    /// <param name="graph">Graph to validate.</param>
    /// <returns>True if the graph's edges are consisten; otherwise false.
    /// </returns>
    public static bool IsConsistent<T>(DirectedGraph<T> graph)
    {
        var nodes = graph.Nodes.ToHashSet();
        var outEdges = new List<(T, T)>();
        foreach (var from in nodes)
        {
            foreach (var to in graph.Successors(from))
            {
                outEdges.Add((from, to));
            }
        }
        foreach (var to in nodes)
        {
            foreach (var from in graph.Predecessors(to))
            {
                if (!outEdges.Contains((from, to)))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
