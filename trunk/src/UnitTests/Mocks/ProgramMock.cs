/* 
 * Copyright (C) 1999-2008 John Källén.
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
using Decompiler.Core.Code;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// A mock that builds a progam.
	/// </summary>
	public class ProgramMock 
	{
		private Program prog;
		private uint procCount;
		private Hashtable nameToProcedure = new Hashtable();
		private ArrayList unresolvedProcedures = new ArrayList();

		public ProgramMock()
		{
			prog = new Program();
		}

		public void Add(ProcedureMock mock)
		{
			++procCount;
			mock.ProgramMock = this;
			Procedure proc = mock.Procedure;
			prog.Procedures[new Address(procCount)] = proc;
			nameToProcedure.Add(mock.Procedure.Name, proc);
			Console.WriteLine(mock.GetType().Name);
			unresolvedProcedures.AddRange(mock.UnresolvedProcedures);
		}

		public void BuildCallgraph()
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				foreach (Block block in proc.RpoBlocks)
				{
					foreach (Statement stm in block.Statements)
					{
						CallInstruction call = stm.Instruction as CallInstruction;
						if (call == null)
							continue;
						prog.CallGraph.AddEdge(stm, call.Callee);
					}
				}
			}
		}

		public Program BuildProgram()
		{
			ResolveUnresolved();
			BuildCallgraph();
			prog.Architecture = new ArchitectureMock();
			return prog;
		}


		public void ResolveUnresolved()
		{
			foreach (ProcedureConstantUpdater pcu in unresolvedProcedures)
			{
				Procedure proc = (Procedure) nameToProcedure[pcu.Name];
				if (proc == null)
					throw new ApplicationException("Unresolved procedure name: " + pcu.Name);
				pcu.Update(proc);
			}
		}
	}

	public class ProcedureConstantUpdater 
	{
		private string name;
		private CallInstruction ci;

		public ProcedureConstantUpdater(string name, CallInstruction ci)
		{
			this.name = name;
			this.ci = ci;
		}

		public void Update(Procedure proc)
		{
			ci.Callee = proc;
		}

		public string Name
		{
			get { return name; }
		}
	}

}

