#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Serialization;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Classifies blocks in a program as being either:
    /// * nothing special
    /// * blocks that need to be moved to another, possibly new, procedure.
    /// * blocks that need to be cloned into other procedures.
    /// </summary>
    public class CrossProcedureAnalyzer : InstructionVisitorBase
    {
        private Program prog;
        private Procedure procOld;
        private Block block;

        public CrossProcedureAnalyzer(Program prog)
        {
            this.prog = prog;
        }

        public void Analyze(Procedure proc)
        {
            this.procOld = proc;
            foreach (var block in new DfsIterator<Block>(proc.ControlGraph).PreOrder())
            {
                if (block.Statements.Count == 0)
                    continue;
                this.block = block;
                Instruction instr = block.Statements.Last.Instruction;
                instr.Accept(this);
            }
        }

        public override void VisitBranch(Branch b)
        {
            Debug.Assert(block.Succ.Count == 2);
            if (b.Target.Procedure != procOld)
            {
                Debug.Assert(b.Target == block.ThenBlock);
                var procNew = PromoteBlockToProcedureEntry(b.Target);
                var crBlock = CreateCallRetThunk(block, procOld, procNew);


            }
        }

        /// <summary>
        /// Promotes a block to being the entry of a procedure.
        /// </summary>
        /// <param name="block">Block to promote</param>
        /// <param name="addrStart">Address at which the block starts</param>
        /// <param name="proc">The procedure from which the block is called.</param>
        /// <returns></returns>
        public Procedure PromoteBlockToProcedureEntry(Block block)
        {
            Address addrStart = prog.ImageMap.MapLinearAddressToAddress(block.Statements[0].LinearAddress);

            Procedure procNew;
            if (!prog.Procedures.TryGetValue(addrStart, out procNew))
            {
                procNew = Procedure.Create(addrStart, prog.Architecture.CreateFrame());
                procNew.Frame.ReturnAddressSize = procOld.Frame.ReturnAddressSize;
                procNew.Characteristics = new ProcedureCharacteristics(procOld.Characteristics);
                prog.Procedures.Add(addrStart, procNew);
                prog.CallGraph.AddProcedure(procNew);
            }
            return procNew;
        }

        private Block CreateCallRetThunk(Block block, Procedure procOld, Procedure procNew)
        {
            var callRetThunkBlock = procOld.AddBlock(block.Name + Scanner.CallRetThunkSuffix);
            callRetThunkBlock.Statements.Add(0, new CallInstruction(
                    new ProcedureConstant(prog.Architecture.PointerType, procNew),
                    new CallSite(procNew.Signature.ReturnAddressOnStack, 0)));
            prog.CallGraph.AddEdge(callRetThunkBlock.Statements.Last, procNew);
            callRetThunkBlock.Statements.Add(0, new ReturnInstruction());
            return callRetThunkBlock;
        }

    }
}
