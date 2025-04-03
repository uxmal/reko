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

using Reko.Core.Services;
using System;
using System.Collections.Generic;

namespace Reko.Core.Analysis
{
    /// <summary>
    /// A collection of objects that classes that implement <see cref="IAnalysis{T}"/> typically need.
    /// </summary>
    public class AnalysisContext
    {
        /// <summary>
        /// The <see cref="Program"/> currently being analyzed.
        /// </summary>
        public IReadOnlyProgram Program { get; }

        /// <summary>
        /// The strongly connected component (SCC) of procedures that are 
        /// being analyzed.
        /// </summary>
        /// <remarks>
        /// Normally, analysis proceeds one procedure at a time, but
        /// when mutually recursive procedures are encountered, they
        /// have to be processed together.
        /// </remarks>
        public IReadOnlySet<Procedure> SccProcedures { get; }

        /// <summary>
        /// A <see cref="IDynamicLinker"/> instance that can be used to resolve
        /// imported symbols.
        /// </summary>
        public IDynamicLinker DynamicLinker { get; }

        /// <summary>
        /// A <see cref="IServiceProvider"/> instance that can be used to obtain
        /// references to published services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// An instance of the <see cref="IEventListener"/> interface.
        /// This can be considered the "user interface" of the analysis engine.
        /// </summary>
        public IEventListener EventListener { get; }

        /// <summary>
        /// Convenience constructor that creates a new <see cref="AnalysisContext"/> instance for a single
        /// procedure.
        /// </summary>
        /// <param name="program"><see cref="Program"/> being analyzed.</param>
        /// <param name="proc">A single <see cref="Procedure"/>. </param>
        /// <param name="dynamicLinker">An instance of <see cref="IDynamicLinker"/> used to resolve
        /// references to imported symbols.</param>
        /// <param name="services">An instance of <see cref="IServiceProvider"/>.</param>
        /// <param name="eventListener">An instance of <see cref="IEventListener"/>.</param>
        public AnalysisContext(
            IReadOnlyProgram program,
            Procedure proc,
            IDynamicLinker dynamicLinker,
            IServiceProvider services,
            IEventListener eventListener)
            : this(
                  program,
                  new HashSet<Procedure> { proc },
                  dynamicLinker,
                  services,
                  eventListener)
        {
        }

        /// <summary>
        /// Convenience constructor that creates a new <see cref="AnalysisContext"/> instance for a single
        /// procedure.
        /// </summary>
        /// <param name="program"><see cref="Program"/> being analyzed.</param>
        /// <param name="sccprocs">A strongly connected component (SCC) of mutually 
        /// recursive procedures to analyze.</param>
        /// <param name="dynamicLinker">An instance of <see cref="IDynamicLinker"/> used to resolve
        /// references to imported symbols.</param>
        /// <param name="services">An instance of <see cref="IServiceProvider"/>.</param>
        /// <param name="eventListener">An instance of <see cref="IEventListener"/>.</param>
        public AnalysisContext(
            IReadOnlyProgram program,
            IReadOnlySet<Procedure> sccprocs,
            IDynamicLinker dynamicLinker,
            IServiceProvider services,
            IEventListener eventListener)
        {
            this.Program = program;
            this.SccProcedures = sccprocs;
            this.DynamicLinker = dynamicLinker;
            this.Services = services;
            this.EventListener = eventListener;
        }
    }


    /// <summary>
    /// The stage of analysis that is currently being performed. A
    /// custom analysis can be injected at any of these stages.
    /// </summary>
    public enum AnalysisStage
    {
        /// <summary>
        /// The analysis has not yet performed SSA translation.
        /// </summary>
        PreSsa = 0,

        /// <summary>
        /// The analysis has performed SSA translation on regiters,
        /// but not stack variables.
        /// </summary>
        AfterRegisterSsa = 1000,

        /// <summary>
        /// The analysis has performed SSA translation on stack variables.
        /// </summary>
        AfterStackSsa = 2000,

        /// <summary>
        /// The analysis has built (deep) expression trees.
        /// </summary>
        AfterExpressionCoalescing = 4000,
    }
}
