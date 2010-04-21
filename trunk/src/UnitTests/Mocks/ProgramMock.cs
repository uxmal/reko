/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// A mock that builds a progam.
	/// </summary>
	public class ProgramMock 
	{
		private Program prog;
		private uint procCount;
        private Dictionary<string, Procedure> nameToProcedure = new Dictionary<string, Procedure>();
		private List<ProcUpdater> unresolvedProcedures = new List<ProcUpdater>();

		public ProgramMock()
		{
			prog = new Program();
		}

		public void Add(ProcedureMock mock)
		{
			++procCount;
			mock.ProgramMock = this;
			Procedure proc = mock.Procedure;
			proc.RenumberBlocks();
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
			foreach (ProcUpdater pcu in unresolvedProcedures)
			{
				Procedure proc;
                if (!nameToProcedure.TryGetValue(pcu.Name, out proc))
					throw new ApplicationException("Unresolved procedure name: " + pcu.Name);
				pcu.Update(proc);
			}
		}
	}

	public abstract class ProcUpdater
	{
		private string name;
		public ProcUpdater(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public abstract void Update(Procedure proc);
	}

	public class ProcedureConstantUpdater : ProcUpdater
	{
		private CallInstruction ci;

		public ProcedureConstantUpdater(string name, CallInstruction ci) : base(name)
		{
			this.ci = ci;
		}

		public override void Update(Procedure proc)
		{
			ci.Callee = proc;
		}

	}

	public class ApplicationUpdater : ProcUpdater
	{
		private Application appl;

		public ApplicationUpdater(string name, Application appl) : base(name)
		{
			this.appl = appl;
		}

		public override void Update(Procedure proc)
		{
			appl.Procedure = new ProcedureConstant(PrimitiveType.Pointer32, proc);
		}
	}
}

