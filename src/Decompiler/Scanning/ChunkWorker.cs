#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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
using Reko.Core.Diagnostics;
using Reko.Core.Rtl;
using Reko.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Reko.Scanning
{
    /// <summary>
    /// Performs a shingle scan of a chunk of memory that wasn't reached by
    /// the <see cref="RecursiveScanner"/> to find any possible blocks.
    /// </summary>
    [DebuggerDisplay("ChunkWorker {Address} ({Length} units)")]
    public class ChunkWorker : AbstractProcedureWorker
    {
        private readonly ShingleScanner shScanner;
        private readonly int[] blockNos; // 0 = not owned by anyone.
        private readonly BTreeDictionary<Address, RtlBlock> blockStarts;
        private BlockWorker? fallThroughJob;

        public ChunkWorker(
            ShingleScanner scanner,
            IProcessorArchitecture arch,
            Address addrStart,
            int chunkUnits,
            InstrClass rejectMask,
            IDecompilerEventListener listener)
            : base(scanner, addrStart, rejectMask, listener)
        {
            this.shScanner = scanner;
            this.Architecture = arch;
            this.Length = chunkUnits;
            this.blockNos = new int[chunkUnits];
            this.blockStarts = new();
        }

        public IProcessorArchitecture Architecture { get; }

        public int Length { get; }

        public void Run()
        {
            var stepsize = Architecture.InstructionBitSize / Architecture.MemoryGranularity;
            for (int i = 0; i < Length; i += stepsize)
            {
                if (this.blockNos[i] != 0)
                {
                    // Some other block already starts at addrNext + i
                    continue;
                }
                var addrNext = this.Address + i;
                var state = Architecture.CreateProcessorState();
                while (IsInsideChunk(addrNext))
                {
                    if (!shScanner.TryRegisterBlockStart(addrNext, addrNext))
                        break;
                    var trace = MakeTrace(addrNext, state); //$BUG: we should be reusing the trace if at all possible
                    var job = AddJob(addrNext, trace, state);
                    var (block, subinstrTargets, newState) = job.ParseBlock();
                    if (block is null)
                        break;
                    blockStarts.Add(addrNext, block);
                    if (block.IsValid)
                    {
                        HandleBlockEnd(block, job.Trace, subinstrTargets, newState);
                    }
                    else
                    {
                        HandleBadBlock(block.Address);
                        if (block.Length == 0)
                            break;
                    }
                    // Don't use block.Fallthrough, because that would
                    // skip any instructions in delay slots.
                    addrNext = block.Address + block.Length;
                }
				//$TODO: handle crossing the chunk boundary.
            }
        }

        private bool IsInsideChunk(Address addr)
        {
            var offset = addr - this.Address;
            return 0 <= offset && offset < blockNos.Length &&
                blockNos[offset] == 0;
        }

        public override BlockWorker AddJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            if (fallThroughJob is not null && fallThroughJob.Address == addr)
            {
                var job = fallThroughJob;
                fallThroughJob = null;
                return job;
            }
            return base.CreateBlockWorker(shScanner, this, addr, trace, state);
        }

        public override BlockWorker AddFallthroughJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            this.fallThroughJob = base.CreateBlockWorker(shScanner, this, addr, trace, state);
            return fallThroughJob;
        }

        public override RtlBlock? SplitExistingBlock(Address addr)
        {
            if (!this.blockStarts.TryGetLowerBoundIndex(addr, out int iMin))
                return null;
            // Blocks with start address > addr cannot possibly contain addr.
            for (int i = iMin; i >= 0; --i)
            {
                var block = this.blockStarts.Values[i];
                if (block.Address == addr)
                    return block;
                if (block.Address < addr)
                {
                    var blockNew = shScanner.SplitBlockAt(block, addr);
                    if (blockNew is { })
                    {
                        blockStarts[addr] = blockNew;
                        return blockNew;
                    }
                }
            }
            return null;
        }

        public override bool TryMarkVisited(Address addr)
        {
            log.Verbose("        Marking {0} visited.", addr);
            var index = addr - this.Address;
            if (index >= blockNos.Length)
                return false;
            var oldValue = Interlocked.Exchange(ref blockNos[index], 1);
            return oldValue == 0;
        }

        protected override void ProcessCall(RtlBlock blockCaller, Edge edge, ProcessorState state)
        {
            if (IsBadCallTarget(edge.To))
            {
                blockCaller.MarkLastInstructionInvalid();
                return;
            }
            shScanner.RegisterSpeculativeProcedure(edge.To);

            // Assume that the call returns. This is true the majority of the time. Users
            // can always override this by adding user annotations like [[noreturn]]
            if (!shScanner.IsExecutableAddress(blockCaller.FallThrough))
            {
                // We can have a diverging call right before a data segment; don't fall
                // through to it.
                return;
            }
            var fallThrough = new Edge(blockCaller.Address, blockCaller.FallThrough, EdgeType.Fallthrough);
            shScanner.RegisterEdge(fallThrough);
        }

        private bool IsBadCallTarget(Address addrTarget)
        {
            Debug.Assert(shScanner.IsExecutableAddress(addrTarget), "BlockWorker shoudn't allowed invalid call target.");
            return false;
        }

        protected override void ProcessReturn()
        {
        }

        protected override bool TryRegisterTrampoline(
            Address addrFinalInstr,
            List<RtlInstructionCluster> trampolineStub,
            [MaybeNullWhen(false)] out Trampoline trampoline)
        {
            if (!this.shScanner.TryRegisterTrampoline(addrFinalInstr, trampolineStub, out trampoline))
                return false;
            // We may have fallen through into a PLT stub, so make sure there is an edge.
            SplitExistingBlock(trampoline.StubAddress);
            return true;
        }
    }
}
