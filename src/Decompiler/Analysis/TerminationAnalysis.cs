#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Code;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Reko.Core.Services;

namespace Reko.Analysis
{
    public class TerminationAnalysis : InstructionVisitorBase
    {
        private Block curBlock;
        private ProgramDataFlow flow;
        private DecompilerEventListener eventListener;

        public TerminationAnalysis(ProgramDataFlow flow, DecompilerEventListener eventListener)
        {
            this.flow = flow;
            this.eventListener = eventListener;
        }

        public void Analyze(Block b)
        {
            curBlock = b;
            foreach (var stm in b.Statements)
            {
                if (flow[b].TerminatesProcess)
                    return;
                stm.Instruction.Accept(this);
            }
        }

        public override void VisitApplication(Application appl)
        {
            base.VisitApplication(appl);
            var pc = appl.Procedure as ProcedureConstant;
            if (pc == null)
                return;
            if (ProcedureTerminates(pc.Procedure))
            {
                flow[curBlock].TerminatesProcess = true;
            }
        }

        private bool ProcedureTerminates(ProcedureBase proc)
        {
            if (proc.Characteristics != null && proc.Characteristics.Terminates)
                return true;
            var callee = proc as Procedure;
            if (callee == null)
                return false;

            return (callee != null && flow[callee].TerminatesProcess);
        }

        public override void VisitCallInstruction(CallInstruction ci)
        {
            base.VisitCallInstruction(ci);
            var pc = ci.Callee as ProcedureConstant;
            if (pc != null && ProcedureTerminates(pc.Procedure))
            {
                flow[curBlock].TerminatesProcess = true;
            }
        }

        public void Analyze(Procedure procedure)
        {
            if (!CanReachEntryFromExit(procedure))
                flow[procedure].TerminatesProcess = true;
        }

        private bool CanReachEntryFromExit(Procedure procedure)
        {
            var visited = new HashSet<Block>();
            var stack = new Stack<Block>();
            stack.Push(procedure.ExitBlock);
            while (stack.Count > 0)
            {
                var b = stack.Pop();
                if (b == procedure.EntryBlock)
                    return true;
                if (visited.Contains(b))
                    continue;
                visited.Add(b);
                Analyze(b);
                if (!flow[b].TerminatesProcess)
                {
                    foreach (var p in b.Pred)
                        stack.Push(p);
                }
            }
            return false;
        }

        public void Analyze(Program program)
        {
            var gr = new ProcedureGraph(program);
            foreach (var proc in new DfsIterator<Procedure>(gr).PostOrder())
            {
                if (this.eventListener.IsCanceled())
                    break;
                Analyze(proc);
            }
        }
    }
}
