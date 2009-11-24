/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.Core
{
	/// <summary>
	/// Describes the structure of the graph: what nodes call what others.
	/// </summary>
	public class CallGraph
	{
		private List<Procedure> entryPoints = new List<Procedure>();	
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

		public void AddEntryPoint(Procedure proc)
		{
			AddProcedure(proc);
			if (!entryPoints.Contains(proc))
			{
				entryPoints.Add(proc);
			}
		}

		public void AddProcedure(Procedure proc)
		{
			graphProcs.AddNode(proc);
			graphStms.AddNode(proc);
		}

		public void AddStatement(Statement stm)
		{
			graphStms.AddNode(stm);
		}

		public IEnumerable<object> Callees(Statement stm)
		{
            return graphStms.Successors(stm);
		}

		public IEnumerable<Procedure> Callees(Procedure proc)
		{
			return graphProcs.Successors(proc);
		}

        public IEnumerable<Procedure> CallerProcedures(Procedure proc)
		{
			return graphProcs.Predecessors(proc);
		}

        public IEnumerable<object> CallerStatements(Procedure proc)
		{
			return graphStms.Predecessors(proc);
		}

		public void Emit(TextWriter wri)
		{
            SortedList<Procedure, Procedure> sl = new SortedList<Procedure, Procedure>(new ProcedureComparer());
			foreach (Procedure proc in graphProcs.Nodes)
			{
				sl.Add(proc, proc);
			}
			foreach (Procedure proc in sl.Values)
			{
				wri.WriteLine("Procedure {0} calls:", proc.Name);
				foreach (Procedure p in graphProcs.Successors(proc))
				{
					wri.WriteLine("\t{0}", p.Name);
				}
			}
		}

		private class ProcedureComparer : IComparer<Procedure>
		{
			#region IComparer Members

			public int Compare(Procedure x, Procedure y)
			{
				return string.Compare(x.Name, y.Name);
			}

			#endregion
		}


		public List<Procedure> EntryPoints
		{
			get { return entryPoints; }
		}
	}
}
