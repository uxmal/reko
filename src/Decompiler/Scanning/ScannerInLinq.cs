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
using Reko.Core.Diagnostics;
using Reko.Core.Graphs;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    using Instr = ScanResults.Instr;
    using Link = ScanResults.Link;

    public class ScannerInLinq
    {
        private static readonly TraceSwitch trace = new(nameof(ScannerInLinq), "");

        private readonly IServiceProvider services;
        private readonly Program program;
        private readonly IRewriterHost host;
        private readonly IDecompilerEventListener eventListener;

        public ScannerInLinq(IServiceProvider services, Program program, IRewriterHost host, IDecompilerEventListener eventListener)
        {
            this.services = services;
            this.program = program;
            this.host = host;
            this.eventListener = eventListener;
        }

        public ScanResults ScanImage(ScanResults sr)
        {
            sr.WatchedAddresses.Add(Address.Ptr32(0x001126FC));
            sr.WatchedAddresses.Add(Address.Ptr32(0x00112762));

            var binder = new StorageBinder();

            // At this point, we have some entries in the image map
            // that are data, and unscanned ranges in betweeen. We
            // have hopefully a bunch of procedure addresses to
            // break up the unscanned ranges.

            if (ScanInstructions(sr, binder) is null)
                return sr;

            var the_blocks = BuildBasicBlocks(sr, binder);
            sr.BreakOnWatchedAddress(the_blocks.Select(q => q.Key));
            the_blocks = ProcessIndirectTransfers(sr, the_blocks);
            the_blocks = RemoveInvalidBlocks(sr, the_blocks);

            // Remove blocks that fall off the end of the segment
            // or into data.
            Probe(sr);
            sr.ICFG = BuildIcfg(sr, program.NamingPolicy, the_blocks);
            Probe(sr);
            sr.Dump("After shingle scan");

            var sr2 = MakeScanResults(sr);
            // On processors with variable length instructions,
            // there may be many blocks that partially overlap the 
            // "real" blocks that would actually have been executed
            // by the processor. Starting with known "roots", try to
            // remove as many invalid blocks as possible.

            var hsc = new BlockConflictResolver(
                program,
                sr2,
                program.Memory.IsValidAddress,
                host);
            hsc.ResolveBlockConflicts(sr.KnownProcedures.Concat(sr.DirectlyCalledAddresses.Keys));
            Probe(sr);
            sr.Dump("After block conflict resolution");

            // If we detect padding bytes between blocks, 
            // we remove them now.
            var ppf = new ProcedurePaddingFinder(sr);
            var pads = ppf.FindPaddingBlocks();
            ppf.Remove(pads);

            // Detect procedures from the "soup" of basic blocks in sr.
            var pd = new ProcedureDetector(sr2, this.eventListener);
            var procs = pd.DetectProcedures();
            sr.Procedures = procs;
            sr.RemovedPadding = pads;
            return sr;
        }

        private Dictionary<Address, RtlBlock> ProcessIndirectTransfers(ScanResults sr, Dictionary<Address, RtlBlock> the_blocks)
        {
            foreach (var ind in sr.IndirectJumps)
            {
                var instr = sr.FlatInstructions[ind.ToLinear()];
                var block = the_blocks[instr.block_id];
            }
            return the_blocks;
        }

        private ScanResultsV2 MakeScanResults(ScanResults sr)
        {
            var sr2 = new ScanResultsV2();
            foreach (var block in sr.ICFG.Nodes)
            {
                sr2.Blocks.TryAdd(block.Address, block);
                var succs = sr.ICFG.Successors(block);
                if (succs.Count > 0)
                {
                    foreach (var s in succs)
                    {
                        sr2.ICFG.AddEdge(block.Address, s.Address);
                    }
                }
            }
            foreach (var addrProc in sr.KnownProcedures)
            {
                sr2.Procedures.TryAdd(addrProc, new Proc(addrProc, ProvenanceType.Scanning,
                    program.Architecture, program.NamingPolicy.ProcedureName(addrProc)));
            }
            foreach (var de in sr.DirectlyCalledAddresses)
            {
                sr2.SpeculativeProcedures.TryAdd(de.Key, de.Value);
            }
            return sr2;
        }

        /// <summary>
        /// Shingle scan all unscanned regions of the image map, returning an unstructured
        /// "soup" of instructions in the <see cref="ScanResults"/>.
        /// </summary>
        /// <param name="sr">Initial scan results, with known entry points,
        /// procedures, etc.</param>
        /// <returns>The <paramref name="sr"/> object, mutated to contain all the
        /// new instructions.
        /// </returns>
        public ScanResults? ScanInstructions(ScanResults sr, StorageBinder binder)
        {
            var ranges = FindUnscannedRanges().ToList();
            DumpRanges(ranges);
            var workToDo = (ulong)ranges.Sum(r => r.Length);
            var shsc = new ShingledScanner(this.program, this.host, binder, sr, this.eventListener);
            bool unscanned = false;
            foreach (var range in ranges)
            {
                unscanned = true;
                try
                {
                    shsc.ScanRange(range, workToDo);
                }
                catch (AddressCorrelatedException aex)
                {
                    host.Error(aex.Address, aex.Message);
                }
            }
            if (!unscanned)
            {
                // No unscanned blocks were found.
                return null;
            }
            shsc.Dump("After shingle scan graph built");
            return sr;
        }

        [Conditional("DEBUG")]
        private static void DumpRanges(List<Chunk> ranges)
            // List<(IProcessorArchitecture, MemoryArea, Address, uint)> ranges)
        {
            foreach (var range in ranges)
            {
                Debug.Print("{0}: {1} - {2}", range.Architecture?.Name ?? "???", range.Address, range.Length);
            }
        }

        [Conditional("DEBUG")]
        public static void Probe(ScanResults sr)
        {
            sr.BreakOnWatchedAddress(sr.ICFG.Nodes.Select(n => n.Address));
        }

        /// <summary>
        /// Scans the Program object looking for address ranges that have not
        /// been identified as code/data yet.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Chunk> FindUnscannedRanges()
        {
            return MakeTriples(program.ImageMap.Items.Values)
                .Select(triple => CreateUnscannedArea(triple))
                .Where(triple => triple.HasValue)
                .Select(triple => triple!.Value);
        }

        /// <summary>
        /// From an <see cref="IEnumerable{T}"/> of items, generate an <see cref="IEnumerable{T}"/> of 
        /// triples, where the middle item of each triple corresponds to the items from <paramref name="items"/>.
        /// </summary>
        /// <remarks>
        /// Consider the sequence A B C D..X Y Z. The output of this method is:
        /// (_ A B)
        /// (A B C)
        /// (B C D)
        /// ...
        /// (X Y Z)
        /// (Y Z _)
        /// (where '_' is the default element.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns>A sequence of triples</returns>
        private static IEnumerable<(T, T, T)> MakeTriples<T>(IEnumerable<T> items)
        {
            T prev = default!;
            var e = items.GetEnumerator();
            if (!e.MoveNext())
                yield break;
            T item = e.Current;
            while (e.MoveNext())
            {
                var next = e.Current;
                yield return (prev!, item, next);
                prev = item;
                item = next;
            }
            yield return (prev!, item, default(T)!);
        }

        /// <summary>
        /// Given a triple of blocks, decides whether the middle block is an unscanned area
        /// that could be fed to the shingle scanner.
        /// </summary>
        /// <param name="triple"></param>
        /// <returns></returns>
        private Chunk? CreateUnscannedArea((ImageMapItem, ImageMapItem, ImageMapItem) triple)
        {
            var (prev, item, next) = triple;
            if (item.DataType is not UnknownType unk)
                return null;
            if (unk.Size > 0)
                return null;
            if (!this.program.SegmentMap.TryFindSegment(item.Address, out ImageSegment? seg))
                return null;
            if (!seg.IsExecutable)
                return null;

            // Determine an architecture for the item.
            var prevArch = GetBlockArchitecture(prev);
            var nextArch = GetBlockArchitecture(next);
            IProcessorArchitecture? arch;
            if (prevArch is null)
            {
                arch = nextArch ?? program.Architecture;
            }
            else if (nextArch is null)
            {
                arch = prevArch ?? program.Architecture;
            }
            else
            {
                // Both prev and next have an architecture.
                if (prevArch == nextArch)
                {
                    arch = prevArch;
                }
                else
                {
                    // Different architectures on both sides. 
                    // Arbitrarily pick the architecture of the largest 
                    // adjacent block. If they're the same size, default 
                    // to the predecessor.
                    arch = (prev.Size < next.Size)
                        ? nextArch
                        : prevArch;
                }
            }

            return new Chunk(
                arch,
                seg.MemoryArea,
                item.Address,
                item.Size);
        }

        private static IProcessorArchitecture? GetBlockArchitecture(ImageMapItem item)
        {
            return (item is ImageMapBlock imb)
                ? imb.Block!.Procedure.Architecture
                : null;
        }

        /// <summary>
        /// From the "soup" of instructions and links, we construct
        /// basic blocks by finding those instructions that have 0 or more than
        /// 1 predecessor or successors. These instructions delimit the start and 
        /// end of the basic blocks.
        /// </summary>
        public static Dictionary<Address, RtlBlock> BuildBasicBlocks(
            ScanResults sr,
            StorageBinder binder)
        {
            // Count and save the # of successors for each instruction.
            foreach (var cSucc in
                    from link in sr.FlatEdges
                    group link by link.From into g
                    select new { addr = g.Key, Count = g.Count() })
            {
                if (sr.FlatInstructions.TryGetValue(cSucc.addr.ToLinear(), out var instr))
                    instr.succ = cSucc.Count;
            }
            // Count and save the # of predecessors for each instruction.
            foreach (var cPred in
                    from link in sr.FlatEdges
                    group link by link.To into g
                    select new { addr = g.Key, Count = g.Count() })
            {
                if (sr.FlatInstructions.TryGetValue(cPred.addr.ToLinear(), out var instr))
                    instr.pred = cPred.Count;
            }

            var the_excluded_edges = new HashSet<Link>();
            foreach (var instr in sr.FlatInstructions.Values)
            {
                // All blocks longer than 2 instructions must start with a linear instruction.
                if ((instr.Class & InstrClass.Linear) == 0)
                    continue;
                // Find the instruction that is located directly after instr.
                if (!sr.FlatInstructions.TryGetValue(instr.Address.ToLinear() + (uint) instr.Length, out Instr? succ))
                    continue;
                // If the first instruction was padding the next one must also be padding, 
                // otherwise we start a new block.
                if (((instr.Class ^ succ.Class) & InstrClass.Padding) != 0)
                    continue;
                // If the first instruction was a zero instruction the next one must also be zero,
                // otherwise we start a new block.
                if (((instr.Class ^ succ.Class) & InstrClass.Zero) != 0)
                    continue;

                // If succ follows instr and it's not the entry of a known procedure
                // or a called address, we don't need the edge between them since they're inside
                // a basic block. We also mark succ as belonging to the same block as instr.
                // Since we're iterating through FlatInstructions in ascending address order,
                // the block_id's will propagate from the first instruction in each block 
                // to the next.
                if (instr.succ == 1 && succ.pred == 1 &&
                    !sr.KnownProcedures.Contains(succ.Address) &&
                    !sr.DirectlyCalledAddresses.ContainsKey(succ.Address))
                {
                    succ.block_id = instr.block_id;
                    the_excluded_edges.Add(new Link(instr.Address, succ.Address));
                }
            }

            // Build the blocks by grouping the instructions.
            var the_blocks =
                (from i in sr.FlatInstructions.Values
                 group i by i.block_id into g
                 select MakeBlock(sr, g, binder))
                .ToDictionary(b => b.Address);

            // Exclude the now useless edges.
            sr.FlatEdges = 
                (from link in sr.FlatEdges
                join f in sr.FlatInstructions.Values on link.From equals f.Address
                where !the_excluded_edges.Contains(link)
                select new Link(f.block_id, link.To))
                .Distinct()
                .ToList();
            return the_blocks;
        }

        private static RtlBlock MakeBlock(
            ScanResults sr, 
            IGrouping<Address, Instr> g,
            StorageBinder binder)
        {
            var instrs = g.OrderBy(i => i.Address)
                .Select(i => i.rtl)
                .ToList();
            var id = NamingPolicy.Instance.BlockName(g.Key);
            var instrLast = instrs[^1];
            var length = instrLast.Address.ToLinear() + (uint)instrLast.Length - g.Key.ToLinear();
            var addrFallThrough = instrLast.Address + instrLast.Length;
            if (instrLast.Class.HasFlag(InstrClass.Delay))
            {
                if (!sr.FlatInstructions.TryGetValue(addrFallThrough.ToLinear(), out var instrDelayed))
                {
                    trace.Warn("Fell through to an instruction that does not exist: {0}", addrFallThrough);
                    instrLast.Class = InstrClass.Invalid;
                }
                else
                {
                    instrs.RemoveAt(instrs.Count - 1);
                    BlockWorker.StealDelaySlotInstruction(
                        instrLast,
                        instrs,
                        instrDelayed.rtl,
                        (cluster, e) =>
                        {
                            var tmp = binder.CreateTemporary(e.DataType);
                            var ass = new RtlAssignment(tmp, e);
                            return (tmp, new RtlInstructionCluster(cluster.Address, cluster.Length, ass));
                        });
                    addrFallThrough += instrs[^1].Length;
                }
            }

            return RtlBlock.Create(
                g.First().Architecture,
                g.Key,
                id,
                (int) length,
                addrFallThrough,
                ProvenanceType.Heuristic,
                instrs);
        }

        /// <summary>
        /// From the candidate set of <paramref name="blocks"/>, remove blocks that 
        /// are invalid.
        /// </summary>
        /// <returns>A (hopefully smaller) set of blocks.</returns>
        public static Dictionary<Address, RtlBlock> RemoveInvalidBlocks(ScanResults sr, Dictionary<Address, RtlBlock> blocks)
        {
            // Find transitive closure of bad instructions 

            var bad_blocks = new HashSet<Address>(
                (from i in sr.FlatInstructions.Values
                 where i.Class == InstrClass.Invalid
                 select i.block_id));
            var new_bad = bad_blocks;
            var preds = sr.FlatEdges.ToLookup(e => e.To);
            //Debug.Print("Bad {0}",

            //    string.Join(
            //        "\r\n      ",
            //        bad_blocks
            //            .OrderBy(x => x)
            //            .Select(x => string.Format("{0:X8}", x))));
            for (;;)
            {
                // Find all blocks that are reachable from blocks
                // that already are known to be "bad", but that don't
                // end in a call.
                //$TODO: delay slots. @#$#@
                new_bad = new_bad
                    .SelectMany(bad => preds[bad])
                    .Where(l =>
                        !bad_blocks.Contains(l.From)
                        &&
                        !BlockEndsWithCall(blocks[l.From]))
                    .Select(l => l.From)
                    .ToHashSet();

                if (new_bad.Count == 0)
                    break;

                //Debug.Print("new {0}",
                //    string.Join(
                //        "\r\n      ",
                //        bad_blocks
                //            .OrderBy(x => x)
                //            .Select(x => string.Format("{0:X8}", x))));

                bad_blocks.UnionWith(new_bad);
            }
            Debug.Print("Bad blocks: {0} of {1}", bad_blocks.Count, blocks.Count);
            //DumpBadBlocks(sr, blocks, sr.FlatEdges, bad_blocks);

            // Remove edges to bad blocks and bad blocks.
            sr.FlatEdges = sr.FlatEdges
                .Where(e => !bad_blocks.Contains(e.To))
                .ToList();
            blocks = blocks.Values
                .Where(b => !bad_blocks.Contains(b.Address))
                .ToDictionary(k => k.Address);

            return blocks;
        }

        private static bool BlockEndsWithCall(RtlBlock block)
        {
            int iLast = block.Instructions.Count - 1;
            if (iLast < 0)
                return false;
            if (block.Instructions[iLast].Class == (InstrClass.Call | InstrClass.Transfer))
                return true;
            return false;
        }


        public static DiGraph<RtlBlock> BuildIcfg(
            ScanResults sr,
            NamingPolicy namingPolicy,
            Dictionary<Address, RtlBlock> blocks)
        {
            var icfg = new DiGraph<RtlBlock>();
            var map = new Dictionary<Address, RtlBlock>();
            var rtlBlocks =
                from b in blocks.Values
                join i in sr.FlatInstructions.Values on b.Address equals i.block_id into instrs
                orderby b.Address
                select RtlBlock.CreatePartial(
                    null!,
                    b.Address,
                    namingPolicy.BlockName(b.Address),
                    instrs.Select(x => x.rtl).ToList());

            foreach (var rtlBlock in rtlBlocks)
            {
                map[rtlBlock.Address] = rtlBlock;
                icfg.AddNode(rtlBlock);
            }
            foreach (var edge in sr.FlatEdges)
            {
                if (!map.TryGetValue(edge.From, out var from) ||
                    !map.TryGetValue(edge.To, out var to))
                    continue;
                icfg.AddEdge(from, to);
            }
            return icfg;
        }

        private static void DumpInstructions(ScanResults sr)
        {
            Debug.WriteLine(
                string.Join("\r\n",
                    from instr in sr.FlatInstructions.Values
                    join e in sr.FlatEdges on instr.Address equals e.From into es
                    from e in new[] { string.Join(", ", es.Select(ee => string.Format("{0:X8}", ee.To))) }
                    orderby instr.Address
                    select string.Format(
                        "{0:X8} {1} {2} {3}",
                            instr.Address,
                            instr.Length,
                            (char)(instr.Class + 'A'),
                            e)));
        }

        private void DumpBlocks(ScanResults sr, Dictionary<Address, RtlBlock> blocks)
        {
            DumpBlocks(sr, blocks, s => Debug.WriteLine(s));
        }

        // Writes the start and end addresses, size, and successor edges of each block, 
        public void DumpBlocks(ScanResults sr, Dictionary<Address, RtlBlock> blocks, Action<string> writeLine)
        {
            writeLine(
               string.Join(Environment.NewLine,
               from b in blocks.Values
               join i in (
                    from ii in sr.FlatInstructions.Values
                    group ii by ii.block_id into g
                    select new { block_id = g.Key, max = g.Max(iii => iii.Address.ToLinear() + (uint) iii.Length ) })
                    on b.Address equals i.block_id
               join l in sr.FlatInstructions.Values on b.Address equals l.Address
               join e in sr.FlatEdges on b.Address equals e.From into es
               from e in new[] { string.Join(", ", es.Select(ee => string.Format("{0:X8}", ee.To))) }
               orderby b.Address
               select string.Format(
                   "{0:X8}-{1:X8} ({2}): {3}{4}",
                       b.Address,
                       b.Address + (i.max - b.Address.ToLinear()),
                       i.max - b.Address.ToLinear(),
                       RenderType(b.Instructions.Last().Class),
                       e)));

            static string RenderType(InstrClass t)
            {
                if ((t & InstrClass.Zero) != 0)
                    return "Zer ";
                if ((t & InstrClass.Padding) != 0)
                    return "Pad ";
                if ((t & InstrClass.Delay) != 0)
                {
                    if ((t & InstrClass.Call) != 0)
                        return "CalD ";
                    if ((t & InstrClass.ConditionalTransfer) == InstrClass.ConditionalTransfer)
                        return "BraD ";
                    if ((t & InstrClass.Transfer) != 0)
                        return "EndD";
                }
                else
                {
                    if ((t & InstrClass.Call) != 0)
                        return "Cal ";
                    if ((t & InstrClass.ConditionalTransfer) == InstrClass.ConditionalTransfer)
                        return "Bra ";
                    if ((t & InstrClass.Transfer) != 0)
                        return "End";
                }
                return "Lin ";
            }
        }

        private static void DumpBadBlocks(ScanResults sr, Dictionary<long, Block> blocks, IEnumerable<Link> edges, HashSet<Address> bad_blocks)
        {
            Debug.Print(
                "{0}",
                string.Join(Environment.NewLine,
                from b in blocks.Values
                join i in (
                     from ii in sr.FlatInstructions.Values
                     group ii by ii.block_id into g
                     select new { block_id = g.Key, max = g.Max(iii => iii.Address.ToLinear()  + (uint) iii.Length) })
                     on b.Address equals i.block_id
                join e in edges on b.Address equals e.From into es
                from e in new[] { string.Join(", ", es.Select(ee => string.Format("{0:X8}", ee.To))) }
                orderby b.Address
                select string.Format(
                    "{0:X8}-{1:X8} {2} ({3}): {4}",
                        b.Address,
                        b.Address + (i.max  - b.Address.ToLinear()),
                        bad_blocks.Contains(b.Address) ? "*" : " ",
                        i.max - b.Address.ToLinear(),
                        e)));
        }

        [Conditional("DEBUG")]
        private static void Dump(Dictionary<long, Block> the_blocks)
        {
            foreach (var block in the_blocks.Values)
            {
                Debug.Print("{0:X8}", block.Address);
            }
        }

        [Conditional("DEBUG")]
        private static void Dump(IEnumerable<Link> edges)
        {
            foreach (var link in edges)
            {
                Debug.Print("[{0:X8} -> {1:X8}]", link.From, link.To);
            }
        }

        [Conditional("DEBUG")]
        private static void Dump(IEnumerable<Instr> blocks)
        {
            foreach (var i in blocks)
            {
                Debug.Print("{0:X8} {1} {2} {3:X8} {4,2} {5,2}]",
                    i.Address, i.Length, i.Class, i.block_id, i.pred, i.succ);
            }
        }
    }
}
