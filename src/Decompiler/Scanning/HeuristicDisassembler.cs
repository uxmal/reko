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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    public class HeuristicDisassembler
    {
        private Program program;
        private HeuristicProcedure proc;
        private IRewriterHost host;
        private Dictionary<Address, HeuristicBlock> blockMap;

        public HeuristicDisassembler(
            Program program,
            HeuristicProcedure proc,
            IRewriterHost host)
        {
            this.program = program;
            this.proc = proc;
            this.host = host;
            blockMap = new Dictionary<Address, HeuristicBlock>();
        }

        /// <summary>
        /// Recursively disassembles the range of addresses.
        /// <paramref name="proc"/>.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="proc"></param>
        /// <returns></returns>
        public HeuristicBlock Disassemble(Address addr)
        {
            var current = new HeuristicBlock(addr, string.Format("l{0:X}", addr));
            var dasm = program.CreateDisassembler(addr);
            foreach (var instr in dasm.TakeWhile(r => r.Address < proc.EndAddress))
            {
                HeuristicBlock block;
                if (blockMap.TryGetValue(instr.Address, out block))
                {
                    // This instruction was already disassembled before.
                    if (instr.Address.ToLinear() != block.Address.ToLinear())
                    {
                        block = SplitBlock(block, instr.Address);
                    }
                    if (current.Instructions.Count == 0)
                    {
                        // Coincides exactly, return the old block.
                        return block;
                    }
                    else
                    {
                        // Fell into 'block' while disassembling
                        // 'current'. Create a fall-though edge
                        if (!proc.Cfg.Nodes.Contains(current))
                        {
                            AddNode(current);
                        }
                        AddEdge(current, block);
                        return current;
                    }
                }
                else
                {
                    // Fresh instruction
                    AddNode(current);
                    current.Instructions.Add(instr);
                    blockMap.Add(instr.Address, current);
                    var op0 = instr.GetOperand(0);
                    var addrOp= op0 as AddressOperand;
                    switch (instr.InstructionClass)
                    {
                    case InstructionClass.Invalid:
                        current.IsValid = false;
                        return current;
                    case InstructionClass.Transfer | InstructionClass.Call:
                        return current;
                    case InstructionClass.Transfer:
                        if (addrOp != null &&
                            proc.BeginAddress <= addrOp.Address && addrOp.Address < proc.EndAddress)
                        {
                            block = Disassemble(addrOp.Address);
                            AddEdge(current, block);
                            return current;
                        }
                        return current;
                    case InstructionClass.Transfer | InstructionClass.Conditional:
                        if (addrOp != null && program.SegmentMap.IsValidAddress(addrOp.Address))
                        {
                            block = Disassemble(addrOp.Address);
                            Debug.Assert(proc.Cfg.Nodes.Contains(block));
                            AddEdge(current, block);
                        }
                        block = Disassemble(instr.Address + instr.Length);
                        AddEdge(current, block);
                        return current;
                    }
                }
            }
            AddNode(current);
            return current;
        }

        private HeuristicBlock SplitBlock(HeuristicBlock block, Address addr)
        {
            var newBlock = new HeuristicBlock(addr, string.Format("l{0:X}", addr));
            proc.Cfg.Nodes.Add(newBlock);
            newBlock.Instructions.AddRange(
                block.Instructions.Where(r => r.Address >= addr).OrderBy(r => r.Address));
            foreach (var de in blockMap.Where(d => d.Key >= addr && d.Value == block).ToList())
            {
                blockMap[de.Key] = newBlock;
            }
            block.Instructions.RemoveAll(r => r.Address >= addr);
            var succs = proc.Cfg.Successors(block).ToArray();
            foreach (var s in succs)
            {
                AddEdge(newBlock, s);
                RemoveEdge(block, s);
            }
            AddEdge(block, newBlock);
            return newBlock;
        }

        private void AddEdge(HeuristicBlock from, HeuristicBlock to)
        {
            proc.Cfg.AddEdge(from, to);
        }

        private void AddNode(HeuristicBlock block)
        {
            if (!proc.Cfg.Nodes.Contains(block))
                proc.Cfg.Nodes.Add(block);
        }

        private void RemoveEdge(HeuristicBlock from, HeuristicBlock to)
        {
            proc.Cfg.RemoveEdge(from, to);
        }
    }
}
