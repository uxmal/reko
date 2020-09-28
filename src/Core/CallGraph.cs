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

using Reko.Core;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core
{
	/// <summary>
	/// Describes the call structure of the program: what nodes call what others.
	/// </summary>
	public class CallGraph
	{
		private DirectedGraphImpl<Procedure> graphProcs = new DirectedGraphImpl<Procedure>();
		private DirectedGraphImpl<object> graphStms = new DirectedGraphImpl<object>();

		public void AddEdge(Statement stmCaller, Procedure callee)
		{
			graphProcs.AddNode(stmCaller.Block.Procedure);
			graphProcs.AddNode(callee);
			graphProcs.AddEdge(stmCaller.Block.Procedure, callee);

			graphStms.AddNode(stmCaller);
			graphStms.AddNode(callee);
			graphStms.AddEdge(stmCaller, callee);
		}

        public List<Procedure> EntryPoints { get; } = new List<Procedure>();

        public void AddEntryPoint(Procedure proc)
		{
			AddProcedure(proc);
			if (!EntryPoints.Contains(proc))
			{
				EntryPoints.Add(proc);
			}
		}

		public void AddProcedure(Procedure proc)
		{
			graphProcs.AddNode(proc);
			graphStms.AddNode(proc);
		}

        public void RemoveCaller(Statement stm)
        {
            if (!graphStms.Nodes.Contains(stm))
                return;
            var callees = this.Callees(stm).ToArray();
            foreach (var callee in callees)
            {
                graphStms.RemoveEdge(stm, callee);
            }
        }

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

        public IEnumerable<Procedure> CallerProcedures(Procedure proc)
		{
			return graphProcs.Predecessors(proc);
		}

        /// <summary>
        /// Given a procedure, find all the statements that call it.
        /// </summary>
        public IEnumerable<Statement> CallerStatements(Procedure proc)
		{
            if (!graphStms.Nodes.Contains(proc))
                return Array.Empty<Statement>();
            return graphStms.Predecessors(proc).OfType<Statement>()
                .Where(s => s.Block.Procedure != null);
		}

		public void Write(TextWriter wri)
		{
            var sl = graphProcs.Nodes.OrderBy(n => n.Name);
			foreach (Procedure proc in sl)
			{
				wri.WriteLine("Procedure {0} calls:", proc.Name);
				foreach (Procedure p in graphProcs.Successors(proc))
				{
					wri.WriteLine("\t{0}", p.Name);
				}
			}

            var st = graphStms.Nodes.OfType<Statement>().OrderBy(n => n.LinearAddress);
            foreach (var stm in st)
            {
                wri.WriteLine("Statement {0:X8} {1} calls:", stm.LinearAddress, stm.Instruction);
                foreach (Procedure p in graphStms.Successors(stm).OfType<Procedure>())
                {
                    wri.WriteLine("\t{0}", p.Name);
                }
            }
		}

    }
}
