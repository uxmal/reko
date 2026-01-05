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

using Reko.Core;
using Reko.Core.Graphs;
using System;
using System.Collections.Generic;

namespace Reko.Analysis
{
    /// <summary>
    /// This class is a wrapper for the <see cref="CallGraph"/> of a program.
    /// The edges go from calling procedure to called procedure.
    /// </summary>
    public class ProcedureGraph : DirectedGraph<Procedure>
    {
        private readonly CallGraph cg;

        /// <summary>
        /// Constructs an instance of <see cref="ProcedureGraph"/>.
        /// </summary>
        /// <param name="program">The program supplying the call graph.</param>
        public ProcedureGraph(Program program)
        {
            this.cg = program.CallGraph;
            this.Nodes = program.Procedures.Values;
        }

        /// <inheritdoc/>
        public ICollection<Procedure> Nodes { get; }

        /// <inheritdoc/>
        public ICollection<Procedure> Predecessors(Procedure node)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public ICollection<Procedure> Successors(Procedure node)
        {
            var succs = new List<Procedure>(cg.Callees(node));
            return succs;
        }

        /// <inheritdoc/>
        public void AddEdge(Procedure nodeFrom, Procedure nodeTo)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void RemoveEdge(Procedure nodeFrom, Procedure nodeTo)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool ContainsEdge(Procedure nodeFrom, Procedure nodeTo)
        {
            throw new NotSupportedException();
        }
    }
}
