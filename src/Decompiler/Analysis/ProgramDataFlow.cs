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
using Reko.Evaluation;
using System;
using System.Collections.Generic;

namespace Reko.Analysis
{
	/// <summary>
	/// Contains dataflow information for each procedure and each block of 
    /// the program.
	/// </summary>
	public class ProgramDataFlow
	{
		private Dictionary<Procedure,ProcedureFlow> procFlow;

		public ProgramDataFlow()
		{
			procFlow = new Dictionary<Procedure,ProcedureFlow>();
		}

		public ProgramDataFlow(Program program) : this()
		{
            CreateFlowsFor(program.Procedures.Values);
		}

        public void CreateFlowsFor(IEnumerable<Procedure> procs)
        {
            foreach (Procedure proc in procs)
            {
                procFlow[proc] = new ProcedureFlow(proc);
            }
        }

		public ProcedureFlow this[Procedure proc]
		{
			get { return procFlow[proc]; }
			set { procFlow[proc] = value; }
		}

        public IDictionary<Procedure, ProcedureFlow> ProcedureFlows
        {
            get { return procFlow; }
        }
	}
}
