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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public class BackwardSlicerHost : IBackWalkHost<RtlBlock, RtlInstruction>
    {
        private readonly Dictionary<Block, RtlBlock> cache;
        private readonly Dictionary<RtlBlock, Block> invCache;

        public BackwardSlicerHost(Program program, IProcessorArchitecture arch)
        {
            this.Program = program;
            this.Architecture = arch;
            this.cache = new Dictionary<Block, RtlBlock>();
            this.invCache = new Dictionary<RtlBlock, Block>();
        }

        public IProcessorArchitecture Architecture { get; }

        public Program Program { get; }

        public (Expression?, Expression?) AsAssignment(RtlInstruction instr)
        {
            if (instr is RtlAssignment ass)
                return (ass.Dst, ass.Src);
            return (null, null);
        }

        public Expression? AsBranch(RtlInstruction instr)
        {
            if (instr is RtlBranch bra)
                return bra.Condition;
            return null;
        }

        public List<RtlBlock> GetPredecessors(RtlBlock block)
        {
            return invCache[block].Pred
                .Select(p => this.GetRtlBlock(p))
                .ToList();
        }

        public int BlockInstructionCount(RtlBlock rtlBlock)
        {
            var block = invCache[rtlBlock];
            return block.Statements.Count;
        }

        public IEnumerable<(Address, RtlInstruction?)> GetBlockInstructions(RtlBlock rtlBlock)
        {
            var block = invCache[rtlBlock];
            if (block.Statements.Count < 1)
                yield break;
            var last = block.Statements[^1];
            if (last is not null && last.Instruction is SwitchInstruction)
            {
                //$TODO: this a workaround; when we run this class on 
                // "raw" RTL, we won't need to special case the SwitchInstruction
                // as it won't exist.
                yield return (default, null);
                yield break;
            }
            foreach (var stm in block.Statements)
            {
                RtlInstruction rtl;
                switch (stm.Instruction)
                {
                case Assignment ass:
                    rtl = new RtlAssignment(ass.Dst, ass.Src);
                    break;
                case Store store:
                    rtl = new RtlAssignment(store.Dst, store.Src);
                    break;
                case Branch branch:
                    //$TODO: this is also a workaround; some blocks have
                    // no addresses because they are synthesized from thin air
                    // after conversion from "raw" RTL.
                    if (branch.Target.Address.Offset == 0)  //$REVIEW: unit test this.
                        yield break;
                    rtl = new RtlBranch(branch.Condition, branch.Target.Address, InstrClass.ConditionalTransfer);
                    break;
                case CallInstruction call:
                    rtl = new RtlCall(call.Callee, (byte)call.CallSite.SizeOfReturnAddressOnStack, InstrClass.Call);
                    break;
                case SideEffect side:
                    rtl = new RtlSideEffect(side.Expression, InstrClass.Linear);
                    break;
                case GotoInstruction go:
                    rtl = new RtlGoto(go.Target, InstrClass.Transfer);
                    break;
                case ReturnInstruction ret:
                    rtl = new RtlReturn(0, 0, InstrClass.Transfer | InstrClass.Return);
                    break;
                default:
                    throw new NotImplementedException($"Translation needed for {stm.Instruction}.");
                }
                yield return (stm.Address, rtl);
            }
        }


        public RtlBlock GetSinglePredecessor(RtlBlock block)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage GetSubregister(RegisterStorage reg, BitRange range)
        {
            return Architecture.GetRegister(reg.Domain, range)!;
        }

        public bool IsFallthrough(RtlInstruction instr, RtlBlock block)
        {
            throw new NotImplementedException();
        }

        public bool IsStackRegister(Storage storage)
        {
            throw new NotImplementedException();
        }

        public bool IsValidAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public Address? MakeAddressFromConstant(Constant c)
        {
            throw new NotImplementedException();
        }

        public Address MakeSegmentedAddress(Constant selector, Constant offset)
        {
            throw new NotImplementedException();
        }

        public RtlBlock GetRtlBlock(Block block)
        {
            if (!cache.TryGetValue(block, out var rtlBlock))
            {
                rtlBlock = RtlBlock.CreateEmpty(block.Procedure.Architecture, block.Address, block.Id);
                cache.Add(block, rtlBlock);
                invCache.Add(rtlBlock, block);
            }
            return rtlBlock;
        }
    }
}
