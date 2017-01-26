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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Reko.Core.Lib;
using Reko.Core.Types;

namespace Reko.Scanning
{
    /// <summary>
    /// Disassembles basic blocks of instructions and places them in 
    /// the provided control flow graph.
    /// </summary>
    public class HeuristicDisassembler
    {
        private Program program;
        private IRewriterHost host;
        private Dictionary<Address, HeuristicBlock> blockMap;
        private ScanResults sr;
        private Func<Address, bool> isAddrValid;
        private bool assumeCallsDiverge;

        public HeuristicDisassembler(
            Program program,
            ScanResults sr,
            Func<Address, bool> isAddrValid,
            bool assumeCallsDiverge,
            IRewriterHost host)
        {
            this.program = program;
            this.sr = sr;
            this.isAddrValid = isAddrValid;
            this.assumeCallsDiverge = assumeCallsDiverge;
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
            foreach (var instr in dasm.TakeWhile(i => isAddrValid(i.Address)))
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
                        if (!sr.ICFG.Nodes.Contains(current))
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
                    var addrOp = DestinationAddress(instr);
                    switch (instr.InstructionClass)
                    {
                    case InstructionClass.Invalid:
                        current.IsValid = false;
                        return current;
                    case InstructionClass.Linear:
                    case InstructionClass.Linear | InstructionClass.Conditional:
                        if (FallthroughToInvalid(instr))
                        {
                            current.IsValid = false;
                            return current;
                        }
                        break;
                    case InstructionClass.Transfer | InstructionClass.Call:
                        if (addrOp != null)
                        {
                            if (program.SegmentMap.IsValidAddress(addrOp))
                            {
                                int c;
                                if (!sr.DirectlyCalledAddresses.TryGetValue(addrOp, out c))
                                    c = 0;
                                sr.DirectlyCalledAddresses[addrOp] = c + 1;
                            }
                            else
                            {
                                current.IsValid = false;
                            }
                        }
                        if (FallthroughToInvalid(instr))
                        {
                            current.IsValid = false;
                            return current;
                        }
                        // If assume calls terminate, stop scanning.
                        if (assumeCallsDiverge)
                        {
                            return current;
                        }
                        block = Disassemble(instr.Address + instr.Length);
                        AddEdge(current, block);
                        return current;
                    case InstructionClass.Transfer:
                        if (addrOp != null && isAddrValid(addrOp))
                        {
                            block = Disassemble(addrOp);
                            AddEdge(current, block);
                            return current;
                        }
                        return current;
                    case InstructionClass.Transfer | InstructionClass.Conditional:
                        FallthroughToInvalid(instr);
                        if (addrOp != null && program.SegmentMap.IsValidAddress(addrOp))
                        {
                            block = Disassemble(addrOp);
                            Debug.Assert(sr.ICFG.Nodes.Contains(block));
                            AddEdge(current, block);
                        }
                        block = Disassemble(instr.Address + instr.Length);
                        AddEdge(current, block);
                        return current;
                    default:
                        throw new NotImplementedException(
                            string.Format(
                                "dasm instruction {0}.", 
                                instr.InstructionClass));
                    }
                }
            }
            AddNode(current);
            return current;
        }

        private bool FallthroughToInvalid(MachineInstruction instr)
        {
            var addrNextInstr = instr.Address + instr.Length;
            if (!isAddrValid(addrNextInstr))
                return true;
            ImageMapItem item;
            if (!program.ImageMap.TryFindItem(addrNextInstr, out item))
                return false;
            return !(item.DataType is UnknownType);
        }

        /// <summary>
        /// Find the constant destination of a transfer instruction.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private Address DestinationAddress(MachineInstruction i)
        {
            var op = i.GetOperand(0) as AddressOperand;
            if (op == null)
            {
                // Z80 has JP Z,<dest> instructions...
                op = i.GetOperand(1) as AddressOperand;
            }
            if (op != null)
            {
                return op.Address;
            }
            return null;
        }

        /// <summary>
        /// Split a block at address <paramref name="addr" />.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        private HeuristicBlock SplitBlock(HeuristicBlock block, Address addr)
        {
            var newBlock = new HeuristicBlock(addr, string.Format("l{0:X}", addr))
            {
                IsValid = block.IsValid
            };
            sr.ICFG.Nodes.Add(newBlock);
            newBlock.Instructions.AddRange(
                block.Instructions.Where(r => r.Address >= addr).OrderBy(r => r.Address));
            foreach (var de in blockMap.Where(d => d.Key >= addr && d.Value == block).ToList())
            {
                blockMap[de.Key] = newBlock;
            }
            block.Instructions.RemoveAll(r => r.Address >= addr);
            var succs = sr.ICFG.Successors(block).ToArray();
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
            sr.ICFG.AddEdge(from, to);
        }

        private void AddNode(HeuristicBlock block)
        {
            if (!sr.ICFG.Nodes.Contains(block))
                sr.ICFG.Nodes.Add(block);
        }

        private void RemoveEdge(HeuristicBlock from, HeuristicBlock to)
        {
            sr.ICFG.RemoveEdge(from, to);
        }
    }
}
