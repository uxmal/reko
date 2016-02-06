using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
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
        private IDictionary<Address, int> possibleCallDestinationTallies;

        public ShingledScanner(Program program, IRewriterHost host)
        {
            this.program = program;
            this.host = host;
            this.bad = program.Platform.MakeAddressFromLinear(~0u);
            this.possibleCallDestinationTallies = new Dictionary<Address,int>();
        }

        /// <summary>
        /// Performs a shingle scan of the executable segments,
        /// returning a list of addresses to probable functions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Address> Scan()
        {
            try {
                Dictionary<ImageSegment, byte[]> map = ScanExecutableSegments();
                return SpeculateCallDests(map);
            }
            catch
            {
                return new Address[0];
            }
        }

        public Dictionary<ImageSegment, byte[]> ScanExecutableSegments()
        {
            var map = new Dictionary<ImageMapSegment, byte[]>();
            foreach (var segment in program.ImageMap.Segments.Values
                .Where(s => (s.Access & AccessMode.Execute) != 0))
            {
                var y = ScanSegment(segment);
                map.Add(segment, y);
            }
            return map;
        }

        /// <summary>
        /// Disassemble every byte of the image, marking those addresses that likely
        /// are code as MaybeCode, everything else as data.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns>An array of bytes classifying each byte as code or data.
        /// </returns>
        public byte[] ScanSegment(ImageSegment segment)
        {
            var G = new DiGraph<Address>();
            G.AddNode(bad);
            var y = new byte[segment.Size];
            var step = program.Architecture.InstructionBitSize / 8;
            bool inDelaySlot = false;
            for (var a = 0; a < y.Length; a += step)
            {
                y[a] = MaybeCode;
                var i = Dasm(segment, a);
                if (i == null || i.InstructionClass == InstructionClass.Invalid)
                {
                    AddEdge(G, bad, i.Address);
                    inDelaySlot = false;
                }
                else
                {
                    if (MayFallThrough(i))
                    {
                        if (!inDelaySlot)
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
                                // jump to data / hyperspace.
                                AddEdge(G, bad, i.Address);
                        }
                    }
                    }

                    // If this is a delayed unconditional branch...
                    inDelaySlot = i.InstructionClass == DT;
                }
            }

            // Find all places that are reachable from "bad" addresses.
            // By transitivity, they must also be be bad.
            foreach (var a in new DfsIterator<Address>(G).PreOrder(bad))
            {
                if (a != bad)
                {
                    y[a - segment.Address] = Data;
                }
            }
            return y;
        }

        /// <summary>
        /// Returns true if this function might continue to the next function.
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
        /// Scans through each segment to find things that look like pointers.
        /// If these pointers point into a valid segment, increment the tally for 
        /// that 
        /// </summary>
        /// <remarks>Tallies saturate at 255, since they're stored as bytes.</remarks>
        /// <returns>A dictionary mapping segments to their pointer tallies.</returns>
        public Dictionary<ImageMapSegment, byte[]> GetPossiblePointerTargets()
        {
            var targetMap = program.ImageMap.Segments.ToDictionary(s => s.Value, s => new byte[s.Value.ContentSize]);
            foreach (var seg in program.ImageMap.Segments.Values)
            {
                foreach (var pointer in GetPossiblePointers(seg))
                {
                    ImageMapSegment segPointee;
                    if (program.ImageMap.TryFindSegment(pointer, out segPointee) &&
                        segPointee.IsInRange(pointer))
                    {
                        int segOffset = (int)(pointer - segPointee.Address);
                        var hits = targetMap[segPointee][segOffset];
                        if (hits < 255)    // Not saturated!
                            targetMap[segPointee][segOffset] = (byte)(hits + 1);
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
        public IEnumerable<Address> GetPossiblePointers(ImageMapSegment seg)
        {
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
            if (!program.ImageMap.TryFindSegment(address, out seg))
                return false;
            return (seg.Access & AccessMode.Execute) != 0;
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

        private MachineInstruction Dasm(ImageSegment segment, int a)
        {
            var dasm = program.CreateDisassembler(segment.Address + a);
            return dasm.FirstOrDefault();
        }

        public IEnumerable<Address> SpeculateCallDests(IDictionary<ImageMapSegment, byte[]> map)
        {
            var addrs = from addr in this.possibleCallDestinationTallies
                    orderby addr.Value descending
                    where IsPossibleExecutableCodeDestination(addr.Key, map)
                    select addr.Key;
            return addrs;
        }

        private bool IsPossibleExecutableCodeDestination(
            Address addr, 
            IDictionary<ImageSegment, byte[]> map)
                        {
            ImageSegment seg;
            if (!program.ImageMap.TryFindSegment(addr, out seg))
                throw new InvalidOperationException(string.Format("Address {0} doesn't belong to any segment.", addr));
            return map[seg][addr - seg.Address] == MaybeCode;
        }
    }
}
