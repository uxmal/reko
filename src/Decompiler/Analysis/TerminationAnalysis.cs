/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.Core.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Analysis
{
    public class TerminationAnalysis : InstructionVisitorBase
    {
        private HashSet<Block> terminatingBlocks;
        private HashSet<Procedure> terminatingProcedures;
        private Block curBlock;

        public TerminationAnalysis()
        {
            this.terminatingBlocks = new HashSet<Block>();
            this.terminatingProcedures = new HashSet<Procedure>();
        }


        public void Analyze(Block b)
        {
            curBlock = b;
            foreach (var stm in b.Statements)
            {
                if (terminatingBlocks.Contains(b))
                    return;
                stm.Instruction.Accept(this);
            }
        }

        public override void VisitCallInstruction(CallInstruction ci)
        {
            base.VisitCallInstruction(ci);
            if (ci.Callee.Characteristics.Terminates)
            {
                terminatingBlocks.Add(curBlock);
            }
            else
            {
                var callee = ci.Callee as Procedure;
                if (callee != null && Terminates(callee))
                    terminatingBlocks.Add(curBlock);
            }
        }

        public bool Terminates(Block b)
        {
            return terminatingBlocks.Contains(b);
        }

        public void Analyze(Procedure procedure)
        {
            Debug.WriteLine("Analyzing: " + procedure.Name);
            if (!CanReachEntryFromExit(procedure))
                terminatingProcedures.Add(procedure);
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
                Analyze(b);
                if (Terminates(b))
                    continue;
                foreach (var p in b.Pred)
                    stack.Push(p);
            }
            return false;
        }

        public bool Terminates(Procedure proc)
        {
            return terminatingProcedures.Contains(proc);
        }

        public void Analyze(Program program)
        {
            DirectedGraph<Procedure> gr = new ProgramGraph(program);
            foreach (var proc in new DfsIterator<Procedure>(gr).PostOrder())
            {
                Analyze(proc);
            }
        }


        private class ProgramGraph : DirectedGraph<Procedure>
        {
            private CallGraph cg;
            private ICollection<Procedure> procs;

            public ProgramGraph(Program prog)
            {
                this.cg = prog.CallGraph;
                this.procs = prog.Procedures.Values;
            }

            public ICollection<Procedure> Predecessors(Procedure node)
            {
                throw new NotSupportedException();
            }

            public ICollection<Procedure> Successors(Procedure node)
            {
                var succs = new List<Procedure>(cg.Callees(node));
                return succs;
            }

            public ICollection<Procedure> Nodes
            {
                get { return procs; } 
            }

            public void AddEdge(Procedure nodeFrom, Procedure nodeTo)
            {
                throw new NotSupportedException();
            }

            public void RemoveEdge(Procedure nodeFrom, Procedure nodeTo)
            {
                throw new NotSupportedException();
            }

            public bool ContainsEdge(Procedure nodeFrom, Procedure nodeTo)
            {
                throw new NotSupportedException();
            }
        }
    }
}
