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

namespace Reko.Core.Analysis
{
    /// <summary>
    /// Classes implementing this interface are responsible for creating objects
    /// implementing the <see cref="IAnalysis{T}"/> interface.
    /// </summary>
    public interface IAnalysisFactory : IExtension
    {
        /// <summary>
        /// Creates an analysis that processes and potentially mutates the provided 
        /// <see cref="SsaState"/>.
        /// </summary>
        /// <param name="stage">The stage at which the master analysis is in.</param>
        /// <param name="ssa">The <see cref="SsaState"/> to be analyzed and potentially
        /// mutated.</param>
        /// <param name="context">Any global context the analysis may need.</param>
        /// <returns></returns>
        IAnalysis<SsaState>[] CreateSsaAnalyses(AnalysisStage stage, SsaState ssa, AnalysisContext context);
    }
}
