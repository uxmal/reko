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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core.Graphs
{
    /// <summary>
    /// Describes the call structure of the program: what nodes call what others.
    /// </summary>
    public class CallGraph : IReadOnlyCallGraph
    {
        //$TODO: implement DirectedBipartiteGraph<Statement, ProcedureBase>
        // and DirectedBipartiteGraph<Procedure, ProcedureBase>
        private DirectedGraphImpl<Procedure> graphProcs = new DirectedGraphImpl<Procedure>();
        private DirectedGraphImpl<ProcedureBase> graphExternals = new DirectedGraphImpl<ProcedureBase>();
        private DirectedGraphImpl<object> graphStms = new DirectedGraphImpl<object>();

        /// <summary>
        /// Adds an edge to the call graph. The edge is directed from the caller 
        /// <see cref="Statement"/> to the callee <see cref="ProcedureBase"/>.
        /// </summary>
        /// <param name="stmCaller">The calling <see cref="Statement"/>.</param>
        /// <param name="callee">The called procedure.</param>
        public void AddEdge(Statement stmCaller, ProcedureBase callee)
        {
            switch (callee)
            {
            case Procedure proc:
                graphProcs.AddNode(stmCaller.Block.Procedure);
                graphProcs.AddNode(proc);
                graphProcs.AddEdge(stmCaller.Block.Procedure, proc);

                graphStms.AddNode(stmCaller);
                graphStms.AddNode(proc);
                graphStms.AddEdge(stmCaller, proc);
                break;
            case ExternalProcedure extProc:
                graphExternals.AddNode(stmCaller.Block.Procedure);
                graphExternals.AddNode(extProc);
                graphExternals.AddEdge(stmCaller.Block.Procedure, extProc);

                graphStms.AddNode(stmCaller);
                graphStms.AddNode(extProc);
                graphStms.AddEdge(stmCaller, extProc);
                break;
            }
        }

        /// <summary>
        /// The entry points of the program.
        /// </summary>
        public List<Procedure> EntryPoints { get; } = [];

        /// <summary>
        /// The procedures of the call graph.
        /// </summary>
        public DirectedGraph<Procedure> Procedures => graphProcs;

        /// <summary>
        /// Adds an entry point procedure to the call graph.
        /// </summary>
        /// <param name="proc">The <see cref="Procedure"/> to add.</param>
        public void AddEntryPoint(Procedure proc)
        {
            AddProcedure(proc);
            if (!EntryPoints.Contains(proc))
            {
                EntryPoints.Add(proc);
            }
        }

        /// <summary>
        /// Adds a procedure to the graph.
        /// </summary>
        /// <param name="proc">Procedure to add.</param>
        public void AddProcedure(Procedure proc)
        {
            graphProcs.AddNode(proc);
            graphStms.AddNode(proc);
        }

        /// <inheritdoc />
        public void RemoveCaller(Statement stm)
        {
            if (!graphStms.Nodes.Contains(stm))
                return;
            var callees = Callees(stm).ToArray();
            foreach (var callee in callees)
            {
                graphStms.RemoveEdge(stm, callee);
            }
        }

        /// <summary>
        /// Gets the of procedures called by the statement <paramref name="stm"/>.
        /// </summary>
        /// <param name="stm">The calling statement.</param>
        /// <returns>An <see cref="IEnumerable{Object}"/> of callees.</returns>
        public IEnumerable<object> Callees(Statement stm)
        {
            return graphStms.Successors(stm);
        }

        /// <summary>
        /// Returns all the procedures that the given procedure calls.
        /// </summary>
		public IEnumerable<Procedure> Callees(Procedure proc)
        {
            return graphProcs.Successors(proc);
        }


        /// <summary>
        /// Returns all the callers of the given callable <paramref name="proc"/>.
        /// </summary>
        /// <param name="proc">The callable whose callers are to be retrieved.
        /// </param>
        /// <returns>Returns all the procedures the given callable calls.
        /// </returns>
        public IEnumerable<Procedure> CallerProcedures(ProcedureBase proc)
        {
            switch (proc)
            {
            case Procedure p:
                return graphProcs.Predecessors(p);
            case ExternalProcedure ep:
                if (graphExternals.Nodes.Contains(ep))
                    return graphExternals.Predecessors(ep).Cast<Procedure>();
                break;
            }
            return Enumerable.Empty<Procedure>();
        }

        /// <summary>
        /// Given a procedure, find all the statements that call it.
        /// </summary>
        public IEnumerable<Statement> FindCallerStatements(ProcedureBase proc)
        {
            if (!graphStms.Nodes.Contains(proc))
                return Array.Empty<Statement>();
            return graphStms.Predecessors(proc).OfType<Statement>()
                .Where(s => s.Block.Procedure is not null);
        }

        /// <summary>
        /// Determines whether the given procedure calls any other 
        /// user procedures. Intrinsics and external procedures are
        /// ignored.
        /// </summary>
        /// <param name="proc">Procedure to test for leaf-ness.</param>
        /// <returns>True if the procedure doesn't call any other
        /// <see cref="Procedure"/>s.
        /// </returns>
        public bool IsLeafProcedure(Procedure proc)
        {
            return graphProcs.Successors(proc).Count == 0;
        }

        /// <summary>
        /// Determine whether the given procedure is called by any
        /// other procedures.
        /// </summary>
        /// <param name="proc">Procedure to test for root-ness.</param>
        /// <returns>True if the procedure isn't called by any
        /// other procedure in this program.</returns>
        public bool IsRootProcedure(Procedure proc)
        {
            return graphProcs.Predecessors(proc).Count == 0;
        }

        /// <summary>
        /// Writes a textual representation of the call graph to the given
        /// <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which the
        /// textual representation is written.
        /// </param>
        public void Write(TextWriter writer)
        {
            var sl = graphProcs.Nodes.OrderBy(n => n.Name);
            foreach (Procedure proc in sl)
            {
                writer.WriteLine("Procedure {0} calls:", proc.Name);
                foreach (Procedure p in graphProcs.Successors(proc))
                {
                    writer.WriteLine("\t{0}", p.Name);
                }
            }

            var st = graphStms.Nodes.OfType<Statement>().OrderBy(n => n.Address);
            foreach (var stm in st)
            {
                writer.WriteLine("Statement {0:X8} {1} calls:", stm.Address, stm.Instruction);
                foreach (Procedure p in graphStms.Successors(stm).OfType<Procedure>())
                {
                    writer.WriteLine("\t{0}", p.Name);
                }
            }
        }

    }
}
