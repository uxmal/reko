#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// This class is used to find registers that are modified or preserved by
    /// a procedure.
    /// </summary>
    public class TrashedRegisterFinder2 
    {
        private IProcessorArchitecture arch;
        private ProgramDataFlow progFlow;
        private SsaTransform ssa;
        private DecompilerEventListener decompilerEventListener;
        private ProcedureFlow2 flow;
        private ISet<SsaTransform> sccGroup;
        private Dictionary<Procedure, HashSet<Storage>> assumedPreserved;

        public TrashedRegisterFinder2(
            IProcessorArchitecture arch,
            ProgramDataFlow flow,
            IEnumerable<SsaTransform> sccGroup,
            DecompilerEventListener listener)
        {
            this.arch = arch;
            this.progFlow = flow;
            this.sccGroup = sccGroup.ToHashSet();
            this.assumedPreserved = sccGroup.ToDictionary(k => k.Procedure, v => new HashSet<Storage>());
            this.decompilerEventListener = listener;
            this.flow = new ProcedureFlow2();
        }

        public ProcedureFlow2 Compute(SsaTransform ssa)
        {
            foreach (var id in ssa.Procedure.ExitBlock.Statements
                .Select(s => s.Instruction as UseInstruction)
                .Where(u => u != null)
                .Select(u => (Identifier) u.Expression))
            {
                CategorizeIdentifier(ssa.SsaState.Identifiers[id]);
            }
            return flow;
        }

        private void CategorizeIdentifier(SsaIdentifier sid)
        {
            if (sid.DefStatement.Instruction is DefInstruction &&
                sid.DefStatement.Block == ssa.Procedure.EntryBlock)
            {
                // Reaching definition was a DefInstruction;
                flow.Preserved.Add(sid.OriginalIdentifier.Storage);
                return;
            }
            Assignment ass;
            if (sid.DefStatement.Instruction.As(out ass))
            {
                if ((ass.Src == sid.OriginalIdentifier) 
                    ||
                   (sid.OriginalIdentifier.Storage == arch.StackRegister &&
                    ass.Src == ssa.Procedure.Frame.FramePointer))
                {
                    flow.Preserved.Add(sid.OriginalIdentifier.Storage);
                    return;
                }
                Constant c;
                if (ass.Src.As(out c))
                {
                    flow.Constants.Add(sid.OriginalIdentifier.Storage, c);
                    // Fall through to Trashed below --v
                }
            }
            CallInstruction call;
            if (sid.DefStatement.Instruction.As(out call))
            {
                VisitCall(call, sid);
                return;
            }
            flow.Trashed.Add(sid.OriginalIdentifier.Storage);
        }

        public void VisitCall(CallInstruction call, SsaIdentifier sid)
        {
            if (!call.Definitions.Any(d => d.Expression == sid.Identifier))
                return;
            ProcedureConstant callee;
            if (!call.Callee.As(out callee))
            {
                throw new NotImplementedException("Indirect calls not handled yet.");
            }

            // Call trashes this identifier. If it's not in our SCC group we
            // know it to be trashed for sure.
            if (!sccGroup.Any(s => s.Procedure == callee.Procedure ))
            {
                flow.Trashed.Add(sid.OriginalIdentifier.Storage);
                return;
            }

            // Assume that it preserves it.

            throw new NotImplementedException();
            // assumedPreserved[proc].Add(sid.OriginalIdentifier.Storage);
        }
    }
}
