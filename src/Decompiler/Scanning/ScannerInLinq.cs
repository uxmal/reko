using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Scanning
{
    public class ScannerInLinq : ScanResults
    {
        // change @binary_size to simulate a large executable
        //private const int binary_size = 10;
        private IServiceProvider services;
        private Program program;
        private Scanner scanner;
        private DecompilerEventListener eventListener;
        private ScanResults sr;

        public ScannerInLinq(IServiceProvider services, Program program, Scanner scanner, DecompilerEventListener eventListener)
        {
            this.services = services;
            this.program = program;
            this.scanner = scanner;
            this.eventListener = eventListener;
        }

        public class block
        {
            public long id;
            public long component_id;
        }

        private class new_block
        {
            public link edge;
            public long block_id;
        }

        public void CollectStatistics(int binary_size)
        {
            // warm up cache
            var sw = new Stopwatch();
            var times = new List<TimeSpan>();
            for (int i = 0; i < 3; ++i)
            {
                sw.Reset();
                sw.Start();
                SimulateBinary(binary_size);
                var sr = new ScanResults();
                sw.Stop();
                times.Add(sw.Elapsed);
            }
            Debug.Print("Times for {0}: {1}", binary_size, string.Join(" ", times.Select(t => t.TotalSeconds)));
        }

        #region Simulate binary
        public void SimulateBinary(int binary_size)
        {
            sr.FlatInstructions = new SortedList<long, instr>();
            // links betweeen instructions
            sr.FlatEdges = new List<link>();


            // Create a simulated program ----------------------------------------------------

            long @offset = 0x40000;
            long @end = @offset + @binary_size;
            while (@offset < @end)
            {
                AddInstr(1 + @offset, 1, 'l'); ;
                AddLink(1 + @offset, 2 + @offset);

                AddInstr(2 + @offset, 1, 'l'); ;
                AddLink(2 + @offset, 4 + @offset);

                AddInstr(3 + @offset, 1, 'C'); ; // Capital 'C' means this was called by someone.
                AddLink(3 + @offset, 4 + @offset);

                AddInstr(4 + @offset, 1, 'c');
                AddLink(4 + @offset, 5 + @offset);
                AddLink(4 + @offset, 6 + @offset);

                AddInstr(5 + @offset, 1, 'l');
                AddLink(5 + @offset, 6 + @offset);

                AddInstr(6 + @offset, 1, 'x');
                AddLink(6 + @offset, 2 + @offset);
                AddLink(6 + @offset, 7 + @offset);

                AddInstr(7 + @offset, 1, 'l');
                AddLink(7 + @offset, 8 + @offset);

                AddInstr(8 + @offset, 1, 'l');
                AddLink(8 + @offset, 9 + @offset);

                AddInstr(9 + @offset, 1, 'l');
                AddLink(9 + @offset, 10 + @offset);

                AddInstr(10 + @offset, 1, 'l');

                @offset = @offset + 20;

            }
        }
        #endregion

        public ScanResults ScanImage(ScanResults sr)
        {
            this.sr = sr;

            // At this point, we have some entries in the image map
            // that are data, and unscanned ranges in betweeen. We
            // have hopefully a bunch of procedure addresses to
            // break up the unscanned ranges.

            var ranges = FindUnscannedRanges();
            var frame = program.Architecture.CreateFrame();
            var shsc = new ShingledScanner(this.program, this.scanner, frame, sr, this.eventListener);
            bool unscanned = false;
            foreach (var range in ranges)
            {
                unscanned = true;
                try
                {
                    shsc.ScanRange(range.Item1,
                        range.Item2,
                        range.Item3,
                        range.Item3.ToLinear() - range.Item2.ToLinear());
                }
                catch (AddressCorrelatedException aex)
                {
                    scanner.Error(aex.Address, aex.Message);
                }
            }
            if (!unscanned)
            {
                // No unscanned blocks were found.
                return null;
            }

            Dictionary<long, block> the_blocks = BuildBasicBlocks(sr);

            // Find all links that don't point to the beginning of a block.
            Debug.Print("Broken links {0}",
                (from l in sr.FlatEdges
                join b in the_blocks.Values on l.first equals b.id into bs
                from b in bs.DefaultIfEmpty()
                where b == null
                select l).Count());


            FindInvalidInstructionClosure();

            BuildWeaklyConnectedComponents(the_blocks);

            // Remove blocks that fall off the end of the segment
            // or into data.
            Probe(sr);
            shsc.Dump("After shingle scan graph built");
            var deadNodes = shsc.RemoveBadInstructionsFromGraph();
            shsc.BuildIcfg(deadNodes);
            Probe(sr);
            sr.Dump("After shingle scan");

            // On processors with variable length instructions,
            // there may be many blocks that partially overlap the 
            // "real" blocks that would actually have been executed
            // by the processor. Starting with known "roots", try to
            // remove as many invalid blocks as possible.

            var hsc = new HeuristicProcedureScanner(
                program,
                sr,
                program.SegmentMap.IsValidAddress,
                scanner);
            //RemoveInvalidBlocks(sr);
            Probe(sr);
            hsc.ResolveBlockConflicts(sr.KnownProcedures.Concat(sr.DirectlyCalledAddresses.Keys));
            Probe(sr);
            sr.Dump("After block conflict resolution");
            var pd = new ProcedureDetector(program, sr, this.eventListener);
            var procs = pd.DetectProcedures();
            sr.Procedures = procs;
            return sr;
        }

        [Conditional("DEBUG")]
        public void Probe(ScanResults sr)
        {

        }

        /// <summary>
        /// Scans the Program object looking for address ranges that have not
        /// been identified as code/data yet.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<MemoryArea, Address, Address>> FindUnscannedRanges()
        {
            return this.program.ImageMap.Items
                .Where(de => de.Value.DataType is UnknownType)
                .Select(de => CreateUnscannedArea(de))
                .Where(tup => tup != null);
        }

        private Tuple<MemoryArea, Address, Address> CreateUnscannedArea(KeyValuePair<Address, ImageMapItem> de)
        {
            ImageSegment seg;
            if (!this.program.SegmentMap.TryFindSegment(de.Key, out seg))
                return null;
            if (!seg.IsExecutable)
                return null;
            return Tuple.Create(
                seg.MemoryArea,
                de.Key,
                de.Key + de.Value.Size);
        }

        private void BuildWeaklyConnectedComponents(Dictionary<long, block> the_blocks)
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
                     select new { block = bb, target = cc.target }))
                {
                    bc.block.component_id = Math.Min(bc.block.component_id, bc.target);
                }
            }
        }

        public Dictionary<long, block> BuildBasicBlocks(ScanResults sr)
        {
            // Compute all basic blocks -----------------------------------------------

            // count and save the # of predecessors and successors for each instr
            foreach (var cSucc in
                    from link in sr.FlatEdges
                    group link by link.first into g
                    select new { addr = g.Key, Count = g.Count() })
            {
                ScanResults.instr instr;
                if (sr.FlatInstructions.TryGetValue(cSucc.addr, out instr))
                    instr.succ = cSucc.Count;
            }
            Debug.Print("succ: {0}", sr.FlatInstructions.Values.Count(v => v.succ > 0));
            foreach (var cPred in
                    from link in sr.FlatEdges
                    group link by link.second into g
                    select new { addr = g.Key, Count = g.Count() })
            {
                ScanResults.instr instr;
                if (sr.FlatInstructions.TryGetValue(cPred.addr, out instr))
                    instr.pred = cPred.Count;
            }
            // This set contains all links between instructions and their immediate successors

            var the_excluded_edges = new HashSet<link>();

            foreach (var instr in sr.FlatInstructions.Values)
            {
                if (instr.type != (ushort)RtlClass.Linear)
                    continue;
                ScanResults.instr succ;
                if (!sr.FlatInstructions.TryGetValue(instr.addr + instr.size, out succ))
                    continue;
                if (instr.succ == 1 && succ.pred == 1)
                {
                    succ.block_id = instr.block_id;
                }
            }

            // Build global block graph
            var the_blocks =
                     sr.FlatInstructions
                     .Select(i => i.Value.block_id)
                     .Distinct()
                     .ToDictionary(k => k, v => new block { id = v, component_id = v });
            sr.FlatEdges = 
                (from link in sr.FlatEdges
                join f in sr.FlatInstructions.Values on link.first equals f.addr
                join t in sr.FlatInstructions.Values on link.second equals t.addr
                select new link { first = f.block_id, second = t.block_id })
                .ToList();
            return the_blocks;
        }

        private void FindInvalidInstructionClosure()
        {
            // Find transitive closure of bad instructions ------------------------

            var linBad = (long) program.Platform.MakeAddressFromLinear(~0UL).ToLinear();
            foreach (var e in sr.FlatEdges.Where(e => e.second == linBad))
            {
                sr.FlatInstructions[e.first].type = (ushort)RtlClass.Invalid;
            }
            for (;;)
            {
                // Find all instructions that are reachable from instructions
                // that already are known to be "bad"
                var new_bad = new HashSet<long>(
                    (from item in sr.FlatInstructions.Values
                     join link in sr.FlatEdges on item.addr equals link.second
                     join pred in sr.FlatInstructions.Values on link.first equals pred.addr
                     where item.type == (ushort)RtlClass.Invalid && pred.type != (ushort)RtlClass.Invalid
                     select pred.addr).Concat(
                    from item in sr.FlatInstructions.Values
                    join link in sr.FlatEdges on item.addr equals link.first
                    join succ in sr.FlatInstructions.Values on link.second equals succ.addr
                    where item.type == (ushort)RtlClass.Invalid && succ.type != (ushort)RtlClass.Invalid
                    select succ.addr));

                if (new_bad.Count == 0)
                    break;

                foreach (var n in new_bad)
                {
                    sr.FlatInstructions[n].type = (ushort)RtlClass.Invalid;
                }
            }
            Debug.Print("Bad instrs: {0} of {1}", sr.FlatInstructions.Values.Where(v => v.type == (ushort)RtlClass.Invalid), sr.FlatInstructions.Count);
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
        private void Dump(IEnumerable<new_block> new_blocks)
        {
            foreach (var b in new_blocks)
            {
                Debug.Print("[{0:X8} -> {1:X8}] {2:X8}", b.edge.first, b.edge.second, b.block_id);
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

        [Conditional("DEBUG")]
        private void AddInstr(long addr, int size, char type)
        {
            sr.FlatInstructions.Add(addr, new instr
            {
                addr = addr,
                size = size,
                type = type,
                block_id = addr,
            });
        }

        [Conditional("DEBUG")]
        private void AddLink(long from, long to)
        {
            sr.FlatEdges.Add(new link { first = from, second = to });
        }

    }
}