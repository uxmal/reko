#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Scanning
{
    using block = ScanResults.block;
    using instr = ScanResults.instr;
    using link = ScanResults.link;

    public class ScannerInLinq
    {
        private readonly IServiceProvider services;
        private readonly Program program;
        private readonly IRewriterHost host;
        private readonly DecompilerEventListener eventListener;

        public ScannerInLinq(IServiceProvider services, Program program, IRewriterHost host, DecompilerEventListener eventListener)
        {
            this.services = services;
            this.program = program;
            this.host = host;
            this.eventListener = eventListener;
        }

        public ScanResults ScanImage(ScanResults sr)
        {
            sr.WatchedAddresses.Add(Address.Ptr32(0x001126FC)); //$DEBUG
            sr.WatchedAddresses.Add(Address.Ptr32(0x00112762)); //$DEBUG
            
            // At this point, we have some entries in the image map
            // that are data, and unscanned ranges in betweeen. We
            // have hopefully a bunch of procedure addresses to
            // break up the unscanned ranges.

            if (ScanInstructions(sr) == null)
                return sr;

            var the_blocks = BuildBasicBlocks(sr);
            sr.BreakOnWatchedAddress(the_blocks.Select(q => q.Key));
            the_blocks = RemoveInvalidBlocks(sr, the_blocks);

            // Remove blocks that fall off the end of the segment
            // or into data.
            Probe(sr);
            sr.ICFG = BuildIcfg(sr, program.NamingPolicy, the_blocks);
            Probe(sr);
            sr.Dump("After shingle scan");

            // On processors with variable length instructions,
            // there may be many blocks that partially overlap the 
            // "real" blocks that would actually have been executed
            // by the processor. Starting with known "roots", try to
            // remove as many invalid blocks as possible.

            var hsc = new BlockConflictResolver(
                program,
                sr,
                program.SegmentMap.IsValidAddress,
                host);
            hsc.ResolveBlockConflicts(sr.KnownProcedures.Concat(sr.DirectlyCalledAddresses.Keys));
            Probe(sr);
            sr.Dump("After block conflict resolution");

            // If we detect padding bytes between blocks, 
            // we remove them now.
            var ppf = new ProcedurePaddingFinder(sr);
            var pads = ppf.FindPaddingBlocks();
            ppf.Remove(pads);

            // Detect procedures from the "soup" of baslic blocks in sr.
            var pd = new ProcedureDetector(program, sr, this.eventListener);
            var procs = pd.DetectProcedures();
            sr.Procedures = procs;
            sr.RemovedPadding = pads;
            return sr;
        }

        public ScanResults ScanInstructions(ScanResults sr)
        {
            var ranges = FindUnscannedRanges().ToList();
            DumpRanges(ranges);
            var workToDo = (ulong)ranges.Sum(r => r.Item4);
            var binder = new StorageBinder();
            var shsc = new ShingledScanner(this.program, this.host, binder, sr, this.eventListener);
            bool unscanned = false;
            foreach (var range in ranges)
            {
                unscanned = true;
                try
                {
                    shsc.ScanRange(
                        range.Item1,
                        range.Item2,
                        range.Item3,
                        range.Item4,
                        workToDo);
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
        private void DumpRanges(List<(IProcessorArchitecture, MemoryArea, Address, uint)> ranges)
        {
            foreach (var range in ranges)
            {
                Debug.Print("{0}: {1} - {2}", range.Item1, range.Item3, range.Item4);
            }
        }

        [Conditional("DEBUG")]
        public void Probe(ScanResults sr)
        {
            sr.BreakOnWatchedAddress(sr.ICFG.Nodes.Select(n => n.Address));
        }

        /// <summary>
        /// Scans the Program object looking for address ranges that have not
        /// been identified as code/data yet.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(IProcessorArchitecture, MemoryArea, Address, uint)> FindUnscannedRanges()
        {
            return MakeTriples(program.ImageMap.Items.Values)
                .Select(triple => CreateUnscannedArea(triple))
                .Where(triple => triple.HasValue)
                .Select(triple => triple.Value);
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
            T prev = default(T);
            var e = items.GetEnumerator();
            if (!e.MoveNext())
                yield break;
            T item = e.Current;
            while (e.MoveNext())
            {
                var next = e.Current;
                yield return (prev, item, next);
                prev = item;
                item = next;
            }
            yield return (prev, item, default(T));
        }

        /// <summary>
        /// Given a triple of blocks, decides whether the middle block is an unscanned area
        /// that could be fed to the shingle scanner.
        /// </summary>
        /// <param name="triple"></param>
        /// <returns></returns>
        private (IProcessorArchitecture, MemoryArea, Address, uint)? CreateUnscannedArea((ImageMapItem, ImageMapItem, ImageMapItem) triple)
        {
            var (prev, item, next) = triple;
            if (!(item.DataType is UnknownType unk))
                return null;
            if (unk.Size > 0)
                return null;
            if (!this.program.SegmentMap.TryFindSegment(item.Address, out ImageSegment seg))
                return null;
            if (!seg.IsExecutable)
                return null;

            // Determine an architecture for the item.
            var prevArch = GetBlockArchitecture(prev);
            var nextArch = GetBlockArchitecture(next);
            IProcessorArchitecture arch = null;
            if (prevArch == null)
            {
                arch = nextArch ?? program.Architecture;
            }
            else if (nextArch == null)
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

            return (
                arch,
                seg.MemoryArea,
                item.Address,
                item.Size);
        }

        private static IProcessorArchitecture GetBlockArchitecture(ImageMapItem item)
        {
            return (item is ImageMapBlock imb)
                ? imb.Block.Procedure.Architecture
                : null;
        }

        private void BuildWeaklyConnectedComponents(ScanResults sr, Dictionary<long, block> the_blocks)
        {
            while (true)
            {
                // Find all links that connect instructions that have
                // different components
                var components_to_merge_raw =
                    (from link in sr.FlatEdges
                     join t1 in the_blocks.Values on link.first equals t1.id
                     join t2 in the_blocks.Values on link.second equals t2.id
                     where t1.component_id != t2.component_id
                     select new link { first = t1.component_id, second = t2.component_id })
                    .Distinct();
                // Ensure symmetry (only for WCC, SCC should remove this)
                var components_to_merge =
                    components_to_merge_raw
                    .Concat(components_to_merge_raw.Select(c => new link { first = c.second, second = c.first }))
                    .Distinct()
                    .ToList();

                if (components_to_merge.Count == 0)
                    break;

                foreach (var bc in
                    (from bb in the_blocks.Values
                     join cc in (
                        from c in components_to_merge
                        group c by c.first into g
                        select new
                        {
                            source = g.Key,
                            target = g.Min(gg => gg.second)
                        }) on bb.component_id equals cc.source
                     select new { block = bb, cc.target }))
                {
                    bc.block.component_id = Address.Min(bc.block.component_id, bc.target);
                }
            }
            Debug.Print("comp: {0}",
                string.Join("\r\n      ",
                from b in the_blocks.Values
                group b by b.component_id into g
                orderby g.Key
                select string.Format("{0:X8}:{1}", g.Key, g.Count())));
        }

        /// <summary>
        /// From the "soup" of instructions and links, we construct
        /// basic blocks by finding those instructions that have 0 or more than
        /// 1 predecessor or successors. These instructions delimit the start and 
        /// end of the basic blocks.
        /// </summary>
        public static Dictionary<Address, block> BuildBasicBlocks(ScanResults sr)
        {
            // Count and save the # of successors for each instruction.
            foreach (var cSucc in
                    from link in sr.FlatEdges
                    group link by link.first into g
                    select new { addr = g.Key, Count = g.Count() })
            {
                if (sr.FlatInstructions.TryGetValue(cSucc.addr.ToLinear(), out var instr))
                    instr.succ = cSucc.Count;
            }
            // Count and save the # of predecessors for each instruction.
            foreach (var cPred in
                    from link in sr.FlatEdges
                    group link by link.second into g
                    select new { addr = g.Key, Count = g.Count() })
            {
                if (sr.FlatInstructions.TryGetValue(cPred.addr.ToLinear(), out var instr))
                    instr.pred = cPred.Count;
            }

            var the_excluded_edges = new HashSet<link>();
            foreach (var instr in sr.FlatInstructions.Values)
            {
                // All blocks must start with a linear instruction.
                if ((instr.type & (ushort)InstrClass.Linear) == 0)
                    continue;
                // Find the instruction that is located directly after instr.
                if (!sr.FlatInstructions.TryGetValue(instr.addr.ToLinear() + (uint) instr.size, out instr succ))
                    continue;
                // If the first instruction was padding the next one must also be padding, 
                // otherwise we start a new block.
                if (((instr.type ^ succ.type) & (ushort)InstrClass.Padding) != 0)
                    continue;
                // If the first instruction was a zero instruction the next one must also be zero,
                // otherwise we start a new block.
                if (((instr.type ^ succ.type) & (ushort) InstrClass.Zero) != 0)
                    continue;

                // If succ follows instr and it's not the entry of a known procedure
                // or a called address, we don't need the edge between them since they're inside
                // a basic block. We also mark succ as belonging to the same block as instr.
                // Since we're iterating through FlatInstructions in ascending address order,
                // the block_id's will propagate from the first instruction in each block 
                // to the next.
                if (instr.succ == 1 && succ.pred == 1 &&
                    !sr.KnownProcedures.Contains(succ.addr) &&
                    !sr.DirectlyCalledAddresses.ContainsKey(succ.addr))
                {
                    succ.block_id = instr.block_id;
                    the_excluded_edges.Add(new link { first = instr.addr, second = succ.addr });
                }
            }

            // Build the blocks by grouping the instructions.
            var the_blocks =
                (from i in sr.FlatInstructions.Values
                 group i by i.block_id into g
                 select new block
                 {
                     id = g.Key,
                     instrs = g.OrderBy(ii => ii.addr).ToArray()
                 })
                .ToDictionary(b => b.id);
            // Exclude the now useless edges.
            sr.FlatEdges = 
                (from link in sr.FlatEdges
                join f in sr.FlatInstructions.Values on link.first equals f.addr
                where !the_excluded_edges.Contains(link)
                select new link { first = f.block_id, second = link.second })
                .Distinct()
                .ToList();
            return the_blocks;
        }

        /// <summary>
        /// From the candidate set of <paramref name="blocks"/>, remove blocks that 
        /// are invalid.
        /// </summary>
        /// <returns>A (hopefully smaller) set of blocks.</returns>
        public static Dictionary<Address, block> RemoveInvalidBlocks(ScanResults sr, Dictionary<Address, block> blocks)
        {
            // Find transitive closure of bad instructions 

            var bad_blocks = new HashSet<Address>(
                (from i in sr.FlatInstructions.Values
                 where i.type == (ushort)InstrClass.Invalid
                 select i.block_id));
            var new_bad = bad_blocks;
            var preds = sr.FlatEdges.ToLookup(e => e.second);
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
                new_bad = new HashSet<Address>(new_bad
                    .SelectMany(bad => preds[bad])
                    .Where(l =>
                        !bad_blocks.Contains(l.first)
                        &&
                        !BlockEndsWithCall(blocks[l.first]))
                    .Select(l => l.first));

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
                .Where(e => !bad_blocks.Contains(e.second))
                .ToList();
            blocks = blocks.Values
                .Where(b => !bad_blocks.Contains(b.id))
                .ToDictionary(k => k.id);

            return blocks;
        }

        private static bool BlockEndsWithCall(block block)
        {
            int len = block.instrs.Length;
            if (len < 1)
                return false;
            if (block.instrs[len - 1].type == (uint)(InstrClass.Call | InstrClass.Transfer))
                return true;
            return false;
        }

        public static DiGraph<RtlBlock> BuildIcfg(
            ScanResults sr,
            NamingPolicy namingPolicy,
            Dictionary<Address, block> blocks)
        {
            var icfg = new DiGraph<RtlBlock>();
            var map = new Dictionary<Address, RtlBlock>();
            var rtlBlocks = 
                from b in blocks.Values
                join i in sr.FlatInstructions.Values on b.id equals i.block_id into instrs
                orderby b.id
                select new RtlBlock(b.id, namingPolicy.BlockName(b.id))
                {
                    Instructions = instrs.Select(x => x.rtl).ToList()
                };

            foreach (var rtlBlock in rtlBlocks)
            {
                map[rtlBlock.Address] = rtlBlock;
                icfg.AddNode(rtlBlock);
            }
            foreach (var edge in sr.FlatEdges)
            {
                if (!map.TryGetValue(edge.first, out var from) ||
                    !map.TryGetValue(edge.second, out var to))
                    continue;
                icfg.AddEdge(from, to);
            }
            return icfg;
        }

        void DumpInstructions(ScanResults sr)
        {
            Debug.WriteLine(
                string.Join("\r\n",
                    from instr in sr.FlatInstructions.Values
                    join e in sr.FlatEdges on instr.addr equals e.first into es
                    from e in new[] { string.Join(", ", es.Select(ee => string.Format("{0:X8}", ee.second))) }
                    orderby instr.addr
                    select string.Format(
                        "{0:X8} {1} {2} {3}",
                            instr.addr,
                            instr.size,
                            (char)(instr.type + 'A'),
                            e)));
        }

        private void DumpBlocks(ScanResults sr, Dictionary<Address, block> blocks)
        {
            DumpBlocks(sr, blocks, s => Debug.WriteLine(s));
        }

        // Writes the start and end addresses, size, and successor edges of each block, 
        public void DumpBlocks(ScanResults sr, Dictionary<Address, block> blocks, Action<string> writeLine)
        {
            writeLine(
               string.Join(Environment.NewLine,
               from b in blocks.Values
               join i in (
                    from ii in sr.FlatInstructions.Values
                    group ii by ii.block_id into g
                    select new { block_id = g.Key, max = g.Max(iii => iii.addr.ToLinear() + (uint) iii.size ) })
                    on b.id equals i.block_id
               join l in sr.FlatInstructions.Values on b.id equals l.addr
               join e in sr.FlatEdges on b.id equals e.first into es
               from e in new[] { string.Join(", ", es.Select(ee => string.Format("{0:X8}", ee.second))) }
               orderby b.id
               select string.Format(
                   "{0:X8}-{1:X8} ({2}): {3}{4}",
                       b.id,
                       b.id + (i.max - b.id.ToLinear()),
                       i.max - b.id.ToLinear(),
                       RenderType(b.instrs.Last().type),
                       e)));

            string RenderType(ushort type)
            {
                var t = (InstrClass)type;
                if ((t & InstrClass.Zero) != 0)
                    return "Zer ";
                if ((t & InstrClass.Padding) != 0)
                    return "Pad ";
                if ((t & InstrClass.Call) != 0)
                    return "Cal ";
                if ((t & InstrClass.ConditionalTransfer) == InstrClass.ConditionalTransfer)
                    return "Bra ";
                if ((t & InstrClass.Transfer) != 0)
                    return "End";
                return "Lin ";
            }
        }

        private void DumpBadBlocks(ScanResults sr, Dictionary<long, block> blocks, IEnumerable<link> edges, HashSet<Address> bad_blocks)
        {
            Debug.Print(
                "{0}",
                string.Join(Environment.NewLine,
                from b in blocks.Values
                join i in (
                     from ii in sr.FlatInstructions.Values
                     group ii by ii.block_id into g
                     select new { block_id = g.Key, max = g.Max(iii => iii.addr.ToLinear()  + (uint) iii.size) })
                     on b.id equals i.block_id
                join e in edges on b.id equals e.first into es
                from e in new[] { string.Join(", ", es.Select(ee => string.Format("{0:X8}", ee.second))) }
                orderby b.id
                select string.Format(
                    "{0:X8}-{1:X8} {2} ({3}): {4}",
                        b.id,
                        b.id + (i.max  - b.id.ToLinear()),
                        bad_blocks.Contains(b.id) ? "*" : " ",
                        i.max - b.id.ToLinear(),
                        e)));
        }

        [Conditional("DEBUG")]
        private void Dump(Dictionary<long, block> the_blocks)
        {
            foreach (var block in the_blocks.Values)
            {
                Debug.Print("{0:X8} - component {1:X8}", block.id, block.component_id);
            }
        }

        [Conditional("DEBUG")]
        private void Dump(IEnumerable<link> edges)
        {
            foreach (var link in edges)
            {
                Debug.Print("[{0:X8} -> {1:X8}]", link.first, link.second);
            }
        }

        [Conditional("DEBUG")]
        private void Dump(IEnumerable<instr> blocks)
        {
            foreach (var i in blocks)
            {
                Debug.Print("{0:X8} {1} {2} {3:X8} {4,2} {5,2}]",
                    i.addr, i.size, i.type, i.block_id, i.pred, i.succ);
            }
        }
    }
}
