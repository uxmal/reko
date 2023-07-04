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

using Reko.Analysis;

namespace Reko.Arch.X86.Analysis
{
    // This class is for illustration purposes only and can be deleted soon.
    internal class DummyAnalysis : IAnalysis<SsaState>
    {
        private readonly SsaState ssa;
        private readonly AnalysisContext ctx;

        public DummyAnalysis(SsaState ssa, AnalysisContext ctx)
        {
            this.ssa = ssa;
            this.ctx = ctx;
        }

        public string Id => "dummy";

        public string Description => "Dummy test of new functionality";

        public (SsaState, bool) Transform(SsaState subject)
        {
            ctx.EventListener.Info("Dummy analysis on {0}", subject.Procedure.Name);
            return (subject, false);
        }
    }
}