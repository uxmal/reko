#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using IrInstruction = Reko.Core.Code.Instruction;

namespace Reko.ImageLoaders.LLVM
{
    public class ProcedureBuilder : CodeEmitter
    {
        private Procedure proc;
        private Block block;
        private Dictionary<string, Block> labelMap;
        private Block lastBlock;
        private Block branchBlock;
        private int tmpCounter;
        private Dictionary<string, Identifier> tmpToIdentifier;
        private ulong linearAddress;

        public ProcedureBuilder(Procedure proc)
        {
            this.proc = proc;
            this.labelMap = new Dictionary<string, Block>();
            this.tmpToIdentifier = new Dictionary<string, Identifier>();
        }

        public override Frame Frame
        {
            get { return proc.Frame; }
        }

        public Procedure Procedure { get { return proc; } }

        public override Statement Emit(IrInstruction instr)
        {
            EnsureBlock(null);
            block.Statements.Add(linearAddress++, instr);
            return block.Statements.Last;
        }

        public override Identifier Register(int i)
        {
            throw new NotImplementedException();
        }

        public override void Return()
        {
            base.Return();
            Procedure.ControlGraph.AddEdge(block, Procedure.ExitBlock);
            TerminateBlock();
            lastBlock = null;
        }

        public Block Label(string name)
        {
            TerminateBlock();
            return EnsureBlock(name);
        }

        private Block BlockOf(string label)
        {
            Block b;
            if (!labelMap.TryGetValue(label, out b))
            {
                b = proc.AddBlock("l" + label);
                labelMap.Add(label, b);
            }
            return b;
        }

        private Block EnsureBlock(string name)
        {
            if (block != null)
                return block;

            if (name == null)
            {
                name = NextTemp();
            }
            block = BlockOf(name);
            if (proc.EntryBlock.Succ.Count == 0)
            {
                proc.ControlGraph.AddEdge(proc.EntryBlock, block);
            }

            if (lastBlock != null)
            {
                if (branchBlock != null)
                {
                    proc.ControlGraph.AddEdge(lastBlock, block);
                    proc.ControlGraph.AddEdge(lastBlock, branchBlock);
                    branchBlock = null;
                }
                else
                {
                    proc.ControlGraph.AddEdge(lastBlock, block);
                }
                lastBlock = null;
            }
            return block;
        }

        public string NextTemp()
        {
            var name = tmpCounter.ToString();
            ++tmpCounter;
            return name;
        }

        private void TerminateBlock()
        {
            if (this.block != null)
            {
                lastBlock = this.block;
                this.block = null;
            }
        }
    }
}
