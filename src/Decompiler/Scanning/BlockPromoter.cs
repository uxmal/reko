#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Lib;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Promotes a block to a procedure by moving all of the blocks that are reachable from the block into a new
    /// procedure.
    /// </summary>
    public class BlockPromoter
    {
        private Program program;
        private Block blockToPromote;
        private Procedure procOld;
        private Procedure procNew;

        public BlockPromoter(Program program)
        {
            this.program = program;
        }

        /// <summary>
        /// Promotes a block to being the entry of a procedure.
        /// </summary>
        /// <param name="block">Block to promote</param>
        /// <param name="addrStart">Address at which the block starts</param>
        /// <param name="proc">The procedure from which the block is called.</param>
        /// <returns></returns>
        public Block PromoteBlock(Block block, Address addrStart)
        {
            this.blockToPromote = block ;
            this.procOld = blockToPromote.Procedure;
            if (!program.Procedures.TryGetValue(addrStart, out procNew))
            {
                procNew = Procedure.Create(addrStart, program.Architecture.CreateFrame());
                procNew.Frame.ReturnAddressSize = procOld.Frame.ReturnAddressSize;
                procNew.Characteristics = new ProcedureCharacteristics(procOld.Characteristics);
                program.Procedures.Add(addrStart, procNew);
                program.CallGraph.AddProcedure(procNew);
            }
            return block;
        }


        private Block CreateCallRetThunk(Block block, Procedure procOld, Procedure procNew)
        {
            var callRetThunkBlock = procOld.AddBlock(block.Name + Scanner.CallRetThunkSuffix);
                callRetThunkBlock.Statements.Add(0, new CallInstruction(
                        new ProcedureConstant(program.Architecture.PointerType, procNew),
                        new CallSite(procNew.Signature.ReturnAddressOnStack, 0)));
                    program.CallGraph.AddEdge(callRetThunkBlock.Statements.Last, procNew);
                    callRetThunkBlock.Statements.Add(0, new ReturnInstruction());
            return callRetThunkBlock;
        }

        /*
            if (BlockInDifferentProcedure(block, proc))
            {
                if (!BlockIsEntryBlock(block))
                {
                    Debug.Print("Block {0} (proc {1}) is not entry block", block, block.Procedure);
                    if (IsLinearReturning(block))
                    {
                        Debug.Print("Cloning {0} to {1}", block.Name, proc);
                        block = new BlockCloner(block, proc, program.CallGraph).Execute();
                    }
                    else
                    {
                        block = PromoteBlock(block, addrStart, proc);
                    }
                }
            }
         
         */

        public Block CallRetThunkBlock { get; private set; }
    }
}
