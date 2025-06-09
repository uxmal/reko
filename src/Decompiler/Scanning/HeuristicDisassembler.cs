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
using Reko.Core.Collections;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// Disassembles basic blocks of instructions and places them in 
    /// the provided control flow graph.
    /// </summary>
    public class HeuristicDisassembler
    {
        private readonly Program program;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly Dictionary<Address, RtlBlock> blockMap;
        private readonly ScanResults sr;
        private readonly Func<Address, bool> isAddrValid;
        private readonly bool assumeCallsDiverge;

        public HeuristicDisassembler(
            Program program,
            IStorageBinder binder,
            ScanResults sr,
            Func<Address, bool> isAddrValid,
            bool assumeCallsDiverge,
            IRewriterHost host)
        {
            this.program = program;
            this.binder = binder;
            this.sr = sr;
            this.isAddrValid = isAddrValid;
            this.assumeCallsDiverge = assumeCallsDiverge;
            this.host = host;
            blockMap = new Dictionary<Address, RtlBlock>();
        }

        /// <summary>
        /// Recursively disassembles the range of addresses.
        /// <paramref name="proc"/>.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="proc"></param>
        /// <returns></returns>
        public RtlBlock Disassemble(Address addr)
        {
            var arch = program.Architecture;
            var current = RtlBlock.CreateEmpty(arch, addr, string.Format("l{0:X}", addr));
            program.TryCreateImageReader(arch, addr, out var rdr);
            
            var dasm = arch.CreateRewriter(
                rdr!,
                arch.CreateProcessorState(),    //$TODO: use state from user.
                binder,
                host);
            foreach (var instr in dasm.TakeWhile(i => isAddrValid(i.Address)))
            {
                if (blockMap.TryGetValue(instr.Address, out RtlBlock? block))
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
                    Address? addrOp;
                    switch (instr.Class & ~(InstrClass.Padding|InstrClass.Zero))
                    {
                    case InstrClass.Invalid:
                    case InstrClass.None:
                        current.IsValid = false;
                        return current;
                    case InstrClass.Linear:
                    case InstrClass.Linear | InstrClass.Conditional:
                    case InstrClass.Linear | InstrClass.Privileged:
                        if (FallthroughToInvalid(instr))
                        {
                            current.IsValid = false;
                            return current;
                        }
                        break;
                    case InstrClass.Transfer | InstrClass.Call:
                        addrOp = DestinationAddress(instr);
                        if (addrOp is not null)
                        {
                            if (program.Memory.IsValidAddress(addrOp.Value))
                            {
                                if (!sr.DirectlyCalledAddresses.TryGetValue(addrOp.Value, out int c))
                                    c = 0;
                                sr.DirectlyCalledAddresses[addrOp.Value] = c + 1;
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
                    case InstrClass.Transfer:
                        addrOp = DestinationAddress(instr);
                        if (addrOp is not null)
                        {
                            if (isAddrValid(addrOp.Value))
                            {
                                block = Disassemble(addrOp.Value);
                                AddEdge(current, block);
                                return current;
                            }
                            else
                            {
                                // Very verbose debugging statement disabled to speed up Reko.
                                //Debug.Print("{0} jumps to invalid address", instr.Address);
                                current.IsValid = false;
                            }
                            return current;
                        }
                        return current;
                    case InstrClass.Transfer | InstrClass.Return:
                        return current;
                    case InstrClass.Transfer | InstrClass.Conditional:
                        FallthroughToInvalid(instr);
                        addrOp = DestinationAddress(instr);
                        if (addrOp is not null && program.Memory.IsValidAddress(addrOp.Value))
                        {
                            block = Disassemble(addrOp.Value);
                            Debug.Assert(sr.ICFG.Nodes.Contains(block));
                            AddEdge(current, block);
                        }
                        block = Disassemble(instr.Address + instr.Length);
                        AddEdge(current, block);
                        return current;
                    default:
                        throw new NotImplementedException(
                            string.Format(
                                "RTL class {0}.", 
                                instr.Class));
                    }
                }
            }
            AddNode(current);
            return current;
        }

        private bool FallthroughToInvalid(RtlInstructionCluster instr)
        {
            var addrNextInstr = instr.Address + instr.Length;
            if (!isAddrValid(addrNextInstr))
                return true;
            if (!program.ImageMap.TryFindItem(addrNextInstr, out ImageMapItem? item))
                return false;
            return item!.DataType is not UnknownType;
        }

        /// <summary>
        /// Find the constant destination of a transfer instruction.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static Address? DestinationAddress(RtlInstructionCluster i)
        {
            var last = i.Instructions[^1];
            var xfer = last as RtlTransfer;
            if (xfer is null)
            {
                if (last is not RtlIf cond)
                    return null;
                xfer = cond.Instruction as RtlGoto;
                if (xfer is null)
                    return null;
            }

            var addr = xfer.Target as Address?;
            return addr;
        }

        /// <summary>
        /// Split a block at address <paramref name="addr" />.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        private RtlBlock SplitBlock(RtlBlock block, Address addr)
        {
            var newBlock = RtlBlock.CreateEmpty(block.Architecture, addr, string.Format("l{0:X}", addr));
            newBlock.IsValid = block.IsValid;
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

        private void AddEdge(RtlBlock from, RtlBlock to)
        {
            sr.ICFG.AddEdge(from, to);
        }

        private void AddNode(RtlBlock block)
        {
            if (!sr.ICFG.Nodes.Contains(block))
                sr.ICFG.Nodes.Add(block);
        }

        private void RemoveEdge(RtlBlock from, RtlBlock to)
        {
            sr.ICFG.RemoveEdge(from, to);
        }
    }
}
