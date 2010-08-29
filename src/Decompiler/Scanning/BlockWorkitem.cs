#region License
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
#endregion

using Decompiler.Core;
using Decompiler.Core.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Scanner work item for processing (extended) basic blocks: linear sequences of code.
    /// </summary>
    public class BlockWorkitem2 : Scanner2.WorkItem2, InstructionVisitor
    {
        private Program prog;
        private Scanner2 scanner;
        private Address addrStart;
        private Block blockCur;

        private bool processNextInstruction;

        public void BlockWorkItem2(Scanner2 scanner, Program prog, Address addr)
        {
            this.prog = prog;
            this.addrStart = addr;
        }

        public override void Process()
        {
            Block block = FindBlock(addrStart);
            if (block != null)
            {
                blockCur = SplitBlock(block, addrStart);
                ResyncBlocks(block, addrStart);
            }
            else
            {
                blockCur = AddBlock(block);
                var rw = prog.Architecture.CreateRewriter2(addrStart);
                processNextInstruction = true;
                foreach (var ri in rw)
                {
                    if (!processNextInstruction)
                        break;
                    ri.Instruction.Accept(this);
                }
            }
        }

        /// <summary>
        /// Rewrites instructions until the current address is exactly on an instruction boundary.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="blockCur"></param>
        /// <param name="addrStart"></param>
        private void ResyncBlocks(Block oldBlock, Address addrStart)
        {
            uint linAddr = (uint) addrStart.Linear;
        }

        private Block AddBlock(Block block)
        {
            throw new NotImplementedException();
        }

        private Block FindBlock(Address addrStart)
        {
            throw new NotImplementedException();
        }

        private void TerminateBlock()
        {
            throw new NotImplementedException();
        }

        private Block SplitBlock(Block block, Address addrStart)
        {
            throw new NotImplementedException();
        }

        #region InstructionVisitor Members

        public void VisitAssignment(Assignment a)
        {
        }

        public void VisitBranch(Branch b)
        {
            scanner.EnqueueJumpTarget(b.Target);
        }

        public void VisitCallInstruction(CallInstruction ci)
        {
            throw new NotImplementedException();
        }

        public void VisitDeclaration(Declaration decl)
        {
        }

        public void VisitDefInstruction(DefInstruction def)
        {
        }

        public void VisitGotoInstruction(GotoInstruction g)
        {
            TerminateBlock();
            this.processNextInstruction = false;
        }

        public void VisitPhiAssignment(PhiAssignment phi)
        {
        }

        public void VisitIndirectCall(IndirectCall ic)
        {
            throw new NotImplementedException();
        }

        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            TerminateBlock();
            this.processNextInstruction = false;
        }

        public void VisitSideEffect(SideEffect side)
        {
        }

        public void VisitStore(Store store)
        {
        }

        public void VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public void VisitUseInstruction(UseInstruction u)
        {
        }

        #endregion
    }
}
