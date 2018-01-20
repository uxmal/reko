#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// Use SSA to determine register preservation / trash status.
    /// Does not modify the procedures, but collects the trash 
    /// information in the DataFlow collection.
    /// </summary>
    public class RegisterPreservation
    {
        private Dictionary<Procedure, SsaState> scc;
        private DataFlow2 dataFlow;
        private ExpressionValueComparer cmp;
        private Identifier idFinal;

        public RegisterPreservation(Dictionary<Procedure, SsaState> scc, DataFlow2 dataFlow)
        {
            this.scc = scc;
            this.dataFlow = dataFlow;
            this.cmp = new ExpressionValueComparer();
        }

        public void Compute()
        {
            foreach (var procedure in scc.Keys)
            {
                Compute(procedure);
            }
        }

        /// <summary>
        /// Compute the preserved and modified registers of this procedure.
        /// </summary>
        /// <remarks>
        /// The strategy is to start at the Exit block, where SSA 
        /// transformation should have created UseInstructions for all
        /// registers used in the program. for each used identifier, we follow
        /// the definition chain backwards. If we hit a phi function, we 
        /// enqueue more chains. Eventually we hit either a def statement,
        /// or a load statement. </remarks>
        /// <param name="proc"></param>
        public void Compute(Procedure proc)
        {
            var procFlow = EnsureProcedureFlow(proc);
            foreach (var use in proc.ExitBlock.Statements.Select(s => (UseInstruction)s.Instruction))
            {
                this.idFinal = (Identifier)use.Expression;
                var worklist = new Queue<Identifier>();
                worklist.Enqueue(idFinal);
                while (worklist.Count > 0)
                {
                    var id = worklist.Dequeue();
                    var sid = scc[proc].Identifiers[id];
                    //Debug.Print("id: {0} stm: {1}", id.Name, sid.DefStatement.Instruction);
                    if (sid.DefStatement.Instruction is DefInstruction)
                    {
                        if (id.Storage != idFinal.Storage)
                        {
                            MarkTrashed(idFinal, procFlow);
                            break;
                        }
                        else
                        {
                            MarkPreserved(idFinal, procFlow);
                        }
                    }
                    else if (sid.DefStatement.Instruction is PhiAssignment)
                    {
                        ProcessPhi((PhiAssignment)sid.DefStatement.Instruction, worklist);
                    }
                    else if (sid.DefStatement.Instruction is Assignment)
                    {
                        ProcessAssignment((Assignment)sid.DefStatement.Instruction, procFlow, worklist);
                    }
                    else
                    {
                        MarkTrashed(id, procFlow);
                    }
                }
            }
        }

        private void ProcessAssignment(Assignment ass, ProcedureFlow2 procFlow, Queue<Identifier> worklist)
        {
            if (ass.Src is Constant)
            {
                SetConstant(ass.Dst, (Constant)ass.Src, procFlow);
            }
            else if (ass.Src is Identifier)
            {
                worklist.Enqueue((Identifier)ass.Src);
            }
            else
            {
                MarkTrashed(idFinal, procFlow);
            }
        }

        private void ProcessPhi(PhiAssignment phi, Queue<Identifier> worklist)
        {
            foreach (Identifier id in phi.Src.Arguments)
            {
                worklist.Enqueue(id);
            }
        }

        private void MarkTrashed(Identifier id, ProcedureFlow2 procFlow)
        {
            procFlow.Preserved.Remove(id.Storage);
            procFlow.Trashed.Add(id.Storage);
            procFlow.Constants.Remove(id.Storage);
        }

        private void SetConstant(Identifier id, Constant c, ProcedureFlow2 procFlow)
        {
            if (procFlow.Trashed.Contains(id.Storage))
            {
                Constant c2;
                if (!procFlow.Constants.TryGetValue(id.Storage, out c2))
                    return;
                if (!cmp.Equals(c, c2))
                    procFlow.Constants.Remove(id.Storage);
            }
            else
            {
                procFlow.Preserved.Remove(id.Storage);
                procFlow.Trashed.Add(id.Storage);
                procFlow.Constants.Add(id.Storage, c);
            }
        }

        private static void MarkPreserved(Identifier id, ProcedureFlow2 procFlow)
        {
            if (procFlow.Trashed.Contains(id.Storage))
                return;
            procFlow.Preserved.Add(id.Storage);
        }

        private ProcedureFlow2 EnsureProcedureFlow(Procedure proc)
        {
            ProcedureFlow2 procFlow;
            if (!dataFlow.ProcedureFlows.TryGetValue(proc, out procFlow))
            {
                procFlow = new ProcedureFlow2();
                dataFlow.ProcedureFlows.Add(proc, procFlow);
            }

            return procFlow;
        }
    }

    public class DataFlow2
    {
        public Dictionary<Procedure, ProcedureFlow2> ProcedureFlows { get; private set; }

        public DataFlow2()
        {
            this.ProcedureFlows = new Dictionary<Procedure, ProcedureFlow2>();
        }
    }
}

