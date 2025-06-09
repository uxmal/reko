#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Diagnostics;
using IrInstruction = Reko.Core.Code.Instruction;
using Branch = Reko.Core.Code.Branch;
using Reko.Core.Code;

namespace Reko.ImageLoaders.LLVM
{
    public class ProcedureBuilder : CodeEmitter
    {
        private Procedure proc;
        private Block? block;
        private Dictionary<string, Block> labelMap;
        private Dictionary<string, Block> deferredBlocks;
        private Block? lastBlock;
        private int tmpCounter;
        private Dictionary<string, Identifier> llvmNametoId;
        private Address address;
        private int stackOffset;

        public ProcedureBuilder(Procedure proc)
        {
            this.proc = proc;
            this.labelMap = new Dictionary<string, Block>();
            this.deferredBlocks = new Dictionary<string, Block>();
            this.llvmNametoId = new Dictionary<string, Identifier>();
            this.address = proc.EntryAddress;
        }

        public override Frame Frame
        {
            get { return proc.Frame; }
        }

        public Procedure Procedure { get { return proc; } }

        public override Statement Emit(IrInstruction instr)
        {
            EnsureBlock(null);
            var stm = block!.Statements.Add(address, instr);
            this.address += 1;
            return stm;
        }

        public override void Return()
        {
            base.Return();
            Procedure.ControlGraph.AddEdge(block!, Procedure.ExitBlock);
            TerminateBlock();
            lastBlock = null;
        }

        public override void Return(Expression exp)
        {
            base.Return(exp);
            Procedure.ControlGraph.AddEdge(block!, Procedure.ExitBlock);
            TerminateBlock();
            lastBlock = null;
        }

        public Block Label(string name)
        {
            TerminateBlock();
            return EnsureBlock(name);
        }

        public void Goto(string name)
        {
            EnsureBlock(null);
            Block blockTo = BlockOf(name, true);
            Procedure.ControlGraph.AddEdge(this.block!, blockTo);
            TerminateBlock();
            EnsureBlock(null);
        }

        public void Branch(Expression expr, string labelTrue, string labelFalse)
        {
            Block b = EnsureBlock(null);
            TerminateBlock();
            EnsureBlock(null);
            var trueBlock = BlockOf(labelTrue, true);
            var falseBlock = BlockOf(labelFalse, true);
            var stm = new Statement(address, new Branch(expr, trueBlock), b);
            b.Statements.Add(stm);
            address += 1;
            proc.ControlGraph.AddEdge(b, falseBlock);
            proc.ControlGraph.AddEdge(b, trueBlock);
        }

        public Block BlockOf(string label, bool defer)
        {
            if (deferredBlocks.TryGetValue(label, out Block? b))
            {
                if (!defer)
                {
                    deferredBlocks.Remove(label);
                    labelMap.Add(label, b);
                }
                return b;
            }
            if (!labelMap.TryGetValue(label, out b))
            {
                b = proc.AddBlock(default, "l" + label);
                if (defer)
                    deferredBlocks.Add(label, b);
                else
                    labelMap.Add(label, b);
            }
            return b;
        }

        public Block EnsureBlock(string? name)
        {
            if (block is not null)
                return block;

            if (name is null)
            {
                name = NextTemp();
            }
            block = BlockOf(name, false);
            if (proc.EntryBlock.Succ.Count == 0)
            {
                proc.ControlGraph.AddEdge(proc.EntryBlock, block);
            }
            return block;
        }

        public Identifier CreateLocalId(string prefix, DataType pt)
        {
            var llvmLocalName = NextTemp();
            var id = Procedure.Frame.CreateTemporary(prefix + llvmLocalName, pt);
            llvmNametoId.Add(llvmLocalName, id);
            return id;
        }

        public Identifier GetLocalId(string llvLocalName)
        {
            return llvmNametoId[llvLocalName];
        }
    
        public string NextTemp()
        {
            var name = tmpCounter.ToString();
            ++tmpCounter;
            return name;
        }

        public void TerminateBlock()
        {
            if (this.block is not null)
            {
                lastBlock = this.block;
                this.block = null;
            }
        }

        public Identifier AllocateStackVariable(DataType type, int count)
        {
            var bytes = type.Size * count;
            Debug.Assert(bytes > 0);
            this.stackOffset -= bytes;
            var stk = Frame.EnsureStackLocal(stackOffset, type);
            return stk;
        }
    }
}
