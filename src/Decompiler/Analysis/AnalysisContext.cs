#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    public class AnalysisContext
    {
        public IReadOnlyProgram Program { get; }
        public IReadOnlySet<Procedure> SccProcedures { get; }
        public IDynamicLinker DynamicLinker { get; }
        public IServiceProvider Services { get; }
        public IDecompilerEventListener EventListener { get; }

        public AnalysisContext(
            IReadOnlyProgram program,
            IReadOnlySet<Procedure> sccprocs,
            IDynamicLinker dynamicLinker,
            IServiceProvider services,
            IDecompilerEventListener eventListener)
        {
            this.Program = program;
            this.SccProcedures = sccprocs;
            this.DynamicLinker = dynamicLinker;
            this.Services = services;
            this.EventListener = eventListener;
        }

    }

    public enum AnalysisStage
    {
        PreSsa = 0,
        AfterRegisterSsa = 1000,
        AfterStackSsa = 2000,
        AfterExpressionCoalescing = 4000,
    }
}
