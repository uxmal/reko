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

#pragma warning disable IDE1006

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Graphs
{
    /// <summary>
    /// Interface describing an undirected graph.
    /// </summary>
    /// <typeparam name="T">Node type.</typeparam>
    public interface UndirectedGraph<T>
    {
        /// <summary>
        /// Returns the neighbors of the given node. 
        /// </summary>
        /// <param name="node">Node whose neighbors are to be retrieved.</param>
        /// <returns>A collection of the neighbors of <paramref name="node"/>.</returns>
        ICollection<T> Neighbors(T node);

        /// <summary>
        /// Adds an edge between two nodes.
        /// </summary>
        /// <param name="nodeFrom">One node.</param>
        /// <param name="nodeTo">Other node.</param>
        void AddEdge(T nodeFrom, T nodeTo);

        /// <summary>
        /// Removes an edge between two nodes.
        /// </summary>
        /// <param name="nodeFrom">One node.</param>
        /// <param name="nodeTo">Other node.</param>
        void RemoveEdge(T nodeFrom, T nodeTo);

        /// <summary>
        /// Determines whether there is a node between two nodes.
        /// </summary>
        /// <param name="nodeFrom">One node.</param>
        /// <param name="nodeTo">Other node.</param>
        /// <returns>Returns true if an edge exists in the graph between the
        /// two nodes; otherwise false.
        /// </returns>
        bool ContainsEdge(T nodeFrom, T nodeTo);
    }
}
