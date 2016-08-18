using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// This scanner disassembles all possible instruction locations of 
    /// an image, and discards instructions that (transitively) result 
    /// in conflicts.
    /// </summary>
    /// <remarks>
    /// Inspired by the paper "Shingled Graph Disassembly:
    /// Finding the Undecidable Path" by Richard Wartell, Yan Zhou, 
    /// Kevin W.Hamlen, and Murat Kantarcioglu.
    /// </remarks>
    public class ShingledScanner
    {
        private const byte MaybeCode = 1;
        private const byte Data = 0;

        private const InstructionClass L = InstructionClass.Linear;
        private const InstructionClass T = InstructionClass.Transfer;
        
        private const InstructionClass CL = InstructionClass.Linear | InstructionClass.Conditional;
        private const InstructionClass CT = InstructionClass.Transfer | InstructionClass.Conditional;
        
        private const InstructionClass DT = InstructionClass.Transfer | InstructionClass.Delay;
        private const InstructionClass DCT = InstructionClass.Transfer | InstructionClass.Conditional | InstructionClass.Delay;

        private Program program;
        private readonly Address bad;
        private IRewriterHost host;
        private DecompilerEventListener eventListener;
        private IDictionary<Address, int> possibleCallDestinationTallies;
        private IDictionary<Address, int> possiblePointerTargetTallies;
        private SortedList<Address, MachineInstruction> instructions;

        public ShingledScanner(Program program, IRewriterHost host, DecompilerEventListener eventListener)
        {
            this.program = program;
            this.host = host;
            this.eventListener = eventListener;
            this.bad = program.Platform.MakeAddressFromLinear(~0ul);
            this.possibleCallDestinationTallies = new Dictionary<Address,int>();
            this.possiblePointerTargetTallies = new Dictionary<Address, int>();
            this.instructions = new SortedList<Address, MachineInstruction>();
        }

        /// <summary>
        /// Performs a shingle scan of the executable segments,
        /// returning a list of addresses to probable functions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<Address,int>> Scan()
        {
            try
            {
                var map = ScanExecutableSegments();
                return SpeculateCallDests(map);
            }
            catch (Exception ex)
            {
                Debug.Print("Error: {0}", ex.Message);
                return new KeyValuePair<Address, int>[0];
            }
        }


        public Dictionary<ImageSegment, byte[]> ScanExecutableSegments()
        {
            var map = new Dictionary<ImageSegment, byte[]>();
            var workToDo = program.SegmentMap.Segments.Values
                .Where(s => s.IsExecutable)
                .Select(s => s.Size)
                .Aggregate(0ul, (s, n) => s + n);
                
            foreach (var segment in program.SegmentMap.Segments.Values
                .Where(s => s.IsExecutable))
            {
                var sc = ScanSegment(segment, workToDo);
                map.Add(segment, sc.CodeFlags);
            }
            return map;
        }

        /// <summary>
        /// Disassemble every byte of the segment, marking those addresses
        /// that likely are code as MaybeCode, everything else as data.
        /// </summary>
        /// <remarks>
        /// The plan is to disassemble every location of the segment, building
        /// a reverse control graph. Any jump to an illegal address or any
        /// invalid instruction will result in an edge from "bad" to that 
        /// instruction.
        /// </remarks>
        /// <param name="segment"></param>
        /// <returns>An array of bytes classifying each byte as code or data.
        /// </returns>
        public ScannedSegment ScanSegment(ImageSegment segment, ulong workToDo)
        {
            var G = new DiGraph<Address>();
            G.AddNode(bad);
            var cbAlloc = Math.Min(
                segment.Size,
                segment.MemoryArea.EndAddress - segment.Address);
            var y = new byte[cbAlloc];

            // Advance by the instruction granularity.
            var step = program.Architecture.InstructionBitSize / 8;
            var delaySlot = InstructionClass.None;
            for (var a = 0; a < y.Length; a += step)
            {
                y[a] = MaybeCode;
                var i = Dasm(segment, a);
                if (i == null)
                {
                    AddEdge(G, bad, segment.Address + a);
                    break;
                }
                if (IsInvalid(segment.MemoryArea, i))
                {
                    AddEdge(G, bad, i.Address);
                    delaySlot = InstructionClass.None;
                    y[a] = Data;
                }
                else
                {
                    if (MayFallThrough(i))
                    {
                        if (delaySlot != DT)
                        {
                            if (a + i.Length < y.Length)
                            {
                                // Still inside the segment.
                                AddEdge(G, i.Address + i.Length, i.Address);
                            }
                            else
                            {
                                // Fell off segment, i must be a bad instruction.
                                AddEdge(G, bad, i.Address);
                                y[a] = Data;
                            }
                        }
                    }
                    if ((i.InstructionClass & InstructionClass.Transfer) != 0)
                    {
                        var addrDest = DestinationAddress(i);
                        if (addrDest != null)
                        {
                            if (IsExecutable(addrDest))
                            {
                                // call / jump destination is executable
                                AddEdge(G, addrDest, i.Address);
                                if ((i.InstructionClass & InstructionClass.Call) != 0)
                                {
                                    int callTally;
                                    if (!this.possibleCallDestinationTallies.TryGetValue(addrDest, out callTally))
                                        callTally = 0;
                                    this.possibleCallDestinationTallies[addrDest] = callTally + 1;
                                }
                            }
                            else
                            {
                                // Jump to data / hyperspace.
                                AddEdge(G, bad, i.Address);
                                y[a] = Data;
                            }
                        }
                    }

                    // If this is a delayed unconditional branch...
                    delaySlot = i.InstructionClass;
                }
                if (y[a] == MaybeCode)
                    instructions.Add(i.Address, i);
                eventListener.ShowProgress("Shingle scanning", instructions.Count, (int)workToDo);
            }

            // Find all places that are reachable from "bad" addresses.
            // By transitivity, they must also be be bad.
            foreach (var a in new DfsIterator<Address>(G).PreOrder(bad))
            {
                if (a != bad)
                {
                    y[a - segment.Address] = Data;
                    instructions.Remove(a);

                    // Destination can't be a call destination.
                    possibleCallDestinationTallies.Remove(a);
                }
            }

            // Build blocks out of sequences of instructions.
            var blocks = BuildBlocks(G, instructions);
            return new ScannedSegment
            {
                Blocks = blocks,
                CodeFlags = y,
            };
        }

        /// <summary>
        /// The results of running the shingle scanner on an image segment.
        /// </summary>
        public class ScannedSegment
        {
            public SortedList<Address, ShingleBlock> Blocks;
            public byte[] CodeFlags;
        }

        public class ShingleBlock
        {
            public Address BaseAddress;
            public Address EndAddress;
        }

        /// <summary>
        /// Build Shingle blocks from the graph. An instruction can only be 
        /// in one block at a time, so at each point in the graph where the 
        /// successors > 1 or the predecessors > 1, we create a new node.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="instructions"></param>
        /// <returns></returns>
        public SortedList<Address,ShingleBlock> BuildBlocks(
            DiGraph<Address> g,
            SortedList<Address,MachineInstruction> instructions)
        {
            // Remember, the graph is backwards!
            var activeBlocks = new List<ShingleBlock>();
            var allBlocks = new SortedList<Address, ShingleBlock>();
            var wl = instructions.Keys.ToSortedSet();
            while (wl.Count > 0)
            {
                var addr = wl.First();
                wl.Remove(addr);
                var instr = instructions[addr];
                var block = new ShingleBlock { BaseAddress = addr };
                allBlocks.Add(addr, block);
                bool terminateNow = false;
                bool terminateDeferred = false;
                for (;;)
                {
                    var addrInstrEnd = instr.Address + instr.Length;
                    if ((instr.InstructionClass & InstructionClass.Transfer) != 0)
                    {
                        if ((instr.InstructionClass & DT) == DT)
                        {
                            terminateDeferred = true;
                        }
                        else
                        {
                            terminateNow = true;
                        }
                    }
                    else
                    {
                        terminateNow = terminateDeferred;
                    }
                    if (terminateNow || 
                        !wl.Contains(addrInstrEnd) ||
                        !g.Nodes.Contains(addrInstrEnd) || 
                        g.Successors(addrInstrEnd).Count != 1)
                    {
                        block.EndAddress = addrInstrEnd;
                        break;
                    }

                    wl.Remove(addrInstrEnd);
                    instr = instructions[addrInstrEnd];
                    terminateNow = terminateDeferred;
                }
            }
            return allBlocks;
        }

        private List<ShingleBlock> TerminateBlocks(List<ShingleBlock> blocks, Address addrTerm)
        {

            var live = blocks.Where(de => de.EndAddress == addrTerm)
                .ToList();
            return live;
        }

        private bool IsInvalid(MemoryArea mem, MachineInstruction instr)
        {
            if (instr.InstructionClass == InstructionClass.Invalid)
                return true;
            // If an instruction straddles a relocation, it can't be 
            // a real instruction.
            if (mem.Relocations.Overlaps(instr.Address, (uint)instr.Length))
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if this function might continue to the next instruction.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool MayFallThrough(MachineInstruction i)
        {
            return (i.InstructionClass &
                (InstructionClass.Linear 
                | InstructionClass.Conditional 
                | InstructionClass.Call)) != 0;        //$REVIEW: what if you call a terminating function?
        }

        private bool IsTransfer(RtlInstructionCluster i, RtlInstruction r)
        {
            return r is RtlGoto || r is RtlCall;
        }

        /// <summary>
        /// Scans through each address in each segment to find things that
        /// look like pointers. If these pointers point into a valid segment,
        /// increment the tally for that address.
        /// </summary>
        /// <returns>A dictionary mapping segments to their pointer tallies.</returns>
        public Dictionary<ImageSegment, int[]> GetPossiblePointerTargets()
        {
            var targetMap = program.SegmentMap.Segments.ToDictionary(
                s => s.Value, 
                s => new int[s.Value.ContentSize]);
            foreach (var seg in program.SegmentMap.Segments.Values)
            {
                foreach (var pointer in GetPossiblePointers(seg))
                {
                    ImageSegment segPointee;
                    if (program.SegmentMap.TryFindSegment(pointer, out segPointee) &&
                        segPointee.IsInRange(pointer))
                    {
                        int segOffset = (int)(pointer - segPointee.Address);
                        int hits = targetMap[segPointee][segOffset];
                        targetMap[segPointee][segOffset] = hits + 1;

                        if (!this.possiblePointerTargetTallies.TryGetValue(pointer, out hits))
                            hits = 0;
                        this.possiblePointerTargetTallies[pointer] = hits + 1;
                    }
                }
            }
            return targetMap;
        }

        /// <summary>
        /// For each location in the segment, read a pointer-sized chunk and return it.
        /// </summary>
        /// <param name="seg"></param>
        /// <returns></returns>
        public IEnumerable<Address> GetPossiblePointers(ImageSegment seg)
        {
            //$TODO: this assumes pointers must be aligned. Not the case for older machines.
            uint ptrSize = (uint)program.Platform.PointerType.Size;
            var rdr = program.CreateImageReader(seg.Address);
            Constant c;
            while (rdr.TryRead(program.Platform.PointerType, out c))
            {
                yield return program.Architecture.MakeAddressFromConstant(c);
            }
        }

        private void AddEdge(DiGraph<Address> g, Address from, Address to)
        {
            g.AddNode(from);
            g.AddNode(to);
            g.AddEdge(from, to);
        }

        private bool IsExecutable(Address address)
        {
            ImageSegment seg;
            if (!program.SegmentMap.TryFindSegment(address, out seg))
                return false;
            return seg.IsExecutable;
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
        /// Grab a single instruction.
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        private MachineInstruction Dasm(ImageSegment segment, int a)
        {
            var addr = segment.Address + a;
            if (!segment.IsInRange(addr) || !segment.MemoryArea.IsValidAddress(addr))
                return null;
            var dasm = program.CreateDisassembler(addr);
            return dasm.FirstOrDefault();
        }

        public IEnumerable<KeyValuePair<Address, int>> SpeculateCallDests(IDictionary<ImageSegment, byte[]> map)
        {
            var addrs = 
                from addr in this.possibleCallDestinationTallies
                orderby addr.Value descending
                where IsPossibleExecutableCodeDestination(addr.Key, map)
                select addr;
            return addrs;
        }

        private bool IsPossibleExecutableCodeDestination(
            Address addr, 
            IDictionary<ImageSegment, byte[]> map)
        {
            ImageSegment seg;
            if (!program.SegmentMap.TryFindSegment(addr, out seg))
                throw new InvalidOperationException(string.Format("Address {0} doesn't belong to any segment.", addr));
            return map[seg][addr - seg.Address] == MaybeCode;
        }
    }
}
