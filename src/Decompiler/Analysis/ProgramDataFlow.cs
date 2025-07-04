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

using Reko.Core;
using System.Collections.Generic;

namespace Reko.Analysis
{
	/// <summary>
	/// Contains dataflow information for each procedure of
    /// the program.
	/// </summary>
	public class ProgramDataFlow
	{
		private readonly Dictionary<Procedure,ProcedureFlow> procFlow;

        /// <summary>
        /// Constructs an empty instance of <see cref="ProgramDataFlow"/>.
        /// </summary>
		public ProgramDataFlow()
		{
			procFlow = new Dictionary<Procedure,ProcedureFlow>();
		}

        /// <summary>
        /// Constructs an instance of <see cref="ProgramDataFlow"/> for the given program.
        /// </summary>
        /// <param name="program">Program used to populate the instance.
        /// </param>
		public ProgramDataFlow(Program program) : this()
		{
            CreateFlowsFor(program.Procedures.Values);
		}

        /// <summary>
        /// Given a collection of procedures, registers a <see cref="ProcedureFlow"/> 
        /// for each procedure.
        /// </summary>
        /// <param name="procs">Collection of <see cref="Procedure"/>s.</param>
        public void CreateFlowsFor(IEnumerable<Procedure> procs)
        {
            foreach (Procedure proc in procs)
            {
                procFlow[proc] = new ProcedureFlow(proc);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ProcedureFlow"/> for the given procedure.
        /// </summary>
        /// <param name="proc">Procedure whose flow is of interest.</param>
		public ProcedureFlow this[Procedure proc]
		{
			get { return procFlow[proc]; }
			set { procFlow[proc] = value; }
		}

        /// <summary>
        /// Retrieves the collection of procedure flows for this program.
        /// </summary>
        public Dictionary<Procedure, ProcedureFlow> ProcedureFlows
        {
            get { return procFlow; }
        }
	}
}
