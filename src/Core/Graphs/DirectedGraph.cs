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

using System.Collections.Generic;

namespace Reko.Core.Graphs;

/// <summary>
/// Represents a directed graph between nodes of <typeparamref name="T"/>.
/// </summary>
public interface DirectedGraph<T>
{
    /// <summary>
    /// Gets the list of the predecessors of <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The nodes whose predecessors to get.</param>
    /// <returns>The predecessor nodes.</returns>
    ICollection<T> Predecessors(T node);

    /// <summary>
    /// Gets the list of the successors of <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The nodes whose successors to get.</param>
    /// <returns>The successor nodes.</returns>
    ICollection<T> Successors(T node);

    /// <summary>
    /// The nodes of the graph.
    /// </summary>
    ICollection<T> Nodes { get; }

    /// <summary>
    /// Adds a graph edge between <paramref name="nodeFrom"/> to 
    /// <paramref name="nodeTo"/>.
    /// </summary>
    /// <param name="nodeFrom">The source of the edge.</param>
    /// <param name="nodeTo">The destination of the edge.</param>
    void AddEdge(T nodeFrom, T nodeTo);

    /// <summary>
    /// Removes the first edge found between <paramref name="nodeFrom"/>
    /// to <paramref name="nodeTo"/>.
    /// </summary>
    /// <param name="nodeFrom">The source of the edge.</param>
    /// <param name="nodeTo">The destination of the edge.</param>
    void RemoveEdge(T nodeFrom, T nodeTo);

    /// <summary>
    /// Determines whether the graph contains an edge between 
    /// <paramref name="nodeFrom"/> and <paramref name="nodeTo"/>.
    /// </summary>
    /// <param name="nodeFrom">The source of the edge.</param>
    /// <param name="nodeTo">The destination of the edge.</param>
    /// <returns>True if an edge exists between <paramref name="nodeFrom"/>
    /// and <paramref name="nodeTo"/>.
    /// </returns>
    bool ContainsEdge(T nodeFrom, T nodeTo);
}

/// <summary>
/// Represents a directed graph between nodes of <typeparamref name="T"/>
/// with edges labeled with <typeparamref name="E"/>.
/// </summary>
public interface DirectedGraph<T, E>
{
    /// <summary>
    /// Gets the list of the predecessors of <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The nodes whose predecessors to get.</param>
    /// <returns>A collection of the predecessor nodes and the labels for each edge.</returns>
    ICollection<(T, E)> Predecessors(T node);

    /// <summary>
    /// Gets the list of the successors of <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The nodes whose successors to get.</param>
    /// <returns>A collection of the successor nodes and the labels for each edge.</returns>
    ICollection<(T, E)> Successors(T node);

    /// <summary>
    /// The nodes of the graph.
    /// </summary>
    ICollection<T> Nodes { get; }

    /// <summary>
    /// Adds a labeled graph edge between <paramref name="nodeFrom"/> to 
    /// <paramref name="nodeTo"/>.
    /// </summary>
    /// <param name="nodeFrom">The source of the edge.</param>
    /// <param name="nodeTo">The destination of the edge.</param>
    /// <param name="edgeLabel">The edge label.</param>
    void AddEdge(T nodeFrom, T nodeTo, E edgeLabel);

    /// <summary>
    /// Removes the first edge found between <paramref name="nodeFrom"/>
    /// to <paramref name="nodeTo"/>.
    /// </summary>
    /// <param name="nodeFrom">The source of the edge.</param>
    /// <param name="nodeTo">The destination of the edge.</param>
    void RemoveEdge(T nodeFrom, T nodeTo);

    /// <summary>
    /// Determines whether the graph contains an edge between 
    /// <paramref name="nodeFrom"/> and <paramref name="nodeTo"/>.
    /// </summary>
    /// <param name="nodeFrom">The source of the edge.</param>
    /// <param name="nodeTo">The destination of the edge.</param>
    /// <returns>True if an edge exists between <paramref name="nodeFrom"/>
    /// and <paramref name="nodeTo"/>.
    /// </returns>
    bool ContainsEdge(T nodeFrom, T nodeTo);

    /// <summary>
    /// Removes the first edge found between <paramref name="nodeFrom"/>
    /// to <paramref name="nodeTo"/>.
    /// </summary>
    /// <param name="nodeFrom">The source of the edge.</param>
    /// <param name="nodeTo">The destination of the edge.</param>
    /// <param name="edgeLabel">The edge label.</param>
    void RemoveEdge(T nodeFrom, T nodeTo, E edgeLabel);

    /// <summary>
    /// Determines whether the graph contains an edge between 
    /// <paramref name="nodeFrom"/> and <paramref name="nodeTo"/>.
    /// </summary>
    /// <param name="nodeFrom">The source of the edge.</param>
    /// <param name="nodeTo">The destination of the edge.</param>
    /// <param name="edgeLabel">The edge label.</param>
    /// <returns>True if an edge exists between <paramref name="nodeFrom"/>
    /// and <paramref name="nodeTo"/>.
    /// </returns>
    bool ContainsEdge(T nodeFrom, T nodeTo, E edgeLabel);
}
