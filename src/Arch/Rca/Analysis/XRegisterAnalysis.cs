#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using System.Linq;

namespace Reko.Arch.Rca.Analysis;

/// <summary>
/// By convention, the X register is 2 on entry to a procedure, which is the stack
/// register R2. This analysis will track the X register and ensure that it is properly
/// set to R2 on entry to a procedure.
/// </summary>
public class XRegisterAnalysis : IAnalysis<SsaState>
{
    private readonly SsaState ssa;
    private readonly AnalysisContext context;

    public XRegisterAnalysis(SsaState ssa, AnalysisContext context)
    {
        this.ssa = ssa;
        this.context = context;
    }

    public string Id => "XRegisterAnalysis";

    public string Description => "Analyzes the X register in the RCA 1802 architecture.";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        return (ssa, false);
    }
}
