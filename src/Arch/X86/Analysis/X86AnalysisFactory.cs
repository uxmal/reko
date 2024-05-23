#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Analysis;
using Reko.Core.Analysis;
using System;

namespace Reko.Arch.X86.Analysis
{
    /// <summary>
    /// This class is responsible for creating instances of analyses that are
    /// specific to the x86 architecture.
    /// </summary>
    public class X86AnalysisFactory : IAnalysisFactory
    {
        public IAnalysis<SsaState>[] CreateSsaAnalyses(AnalysisStage stage, SsaState ssa, AnalysisContext ctx)
        {
            if (stage == AnalysisStage.AfterRegisterSsa)
            {
                return new[] { new FstswAnalysis(ctx.Program, ctx.EventListener) };
            }
            if (stage == AnalysisStage.AfterExpressionCoalescing)
            {
                return new[] { new StrcpyChainRewriter(ctx) };
            }
            return Array.Empty<IAnalysis<SsaState>>();
        }
    }
}
