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

        public ShingledScanner(Program program, IRewriterHost host)
        {
            this.program = program;
            this.host = host;
            this.bad = program.Platform.MakeAddressFromLinear(~0u);
        }

        public IEnumerable<Address> Scan()
        {
            Dictionary<ImageMapSegment, byte[]> map = ScanExecutableSegments();
            return SpeculateCallDests(map);
        }

        public Dictionary<ImageMapSegment, byte[]> ScanExecutableSegments()
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
        public byte[] ScanSegment(ImageMapSegment segment)
        {
            var G = new DiGraph<Address>();
            G.AddNode(bad);
            var y = new byte[segment.ContentSize];
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
                                AddEdge(G, i.Address + i.Length, i.Address);
                            else
                                AddEdge(G, bad, i.Address);
                        }
                    }
                    if ((i.InstructionClass & InstructionClass.Transfer) != 0) 
                    {
                        var dest = Destination(i);
                        if (dest != null)
                        {
                            if (IsExecutable(dest))
                                AddEdge(G, dest, i.Address);
                            else
                                AddEdge(G, bad, i.Address);
                        }
                    }
                    // If this is a delayed unconditional branch...
                    inDelaySlot = i.InstructionClass == DT;
                }
            }
            foreach (var a in new DfsIterator<Address>(G).PreOrder(bad))
            {
                if (a != bad)
                {
                    y[a - segment.Address] = Data;
                }
            }
            return y;
        }

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

        public Dictionary<ImageMapSegment, byte[]> GetPossiblePointerTargets()
        {
            var targetMap = program.ImageMap.Segments.ToDictionary(s => s.Value, s => new byte[s.Value.ContentSize]);
            foreach (var seg in program.ImageMap.Segments.Values)
            {
                foreach (var pointer in GetPossiblePointers(seg))
                {
                    ImageMapSegment segPointee;
                    if (program.ImageMap.TryFindSegment(pointer, out segPointee))
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

        public IEnumerable<Address> GetPossiblePointers(ImageMapSegment seg)
        {
            const uint ptrAlignment = 4;
            const uint ptrSize = 4;
            var addr = seg.Address;
            for (uint offset = 0; offset <= seg.ContentSize - ptrSize; offset += ptrAlignment, addr += ptrAlignment)
            {
                yield return Address.Ptr32(program.Image.ReadLeUInt32(addr));     //$TODO: platform dep.
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
            ImageMapSegment seg;
            if (!program.ImageMap.TryFindSegment(address, out seg))
                return false;
            return (seg.Access & AccessMode.Execute) != 0;
        }

        private Address Destination(MachineInstruction i)
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

        private MachineInstruction Dasm(ImageMapSegment segment, int a)
        {
            var dasm = program.CreateDisassembler(segment.Address + a);
            return dasm.FirstOrDefault();
        }

        private RtlInstructionCluster DasmOld(ImageMapSegment segment, int a)
        {
            var rw = program.Architecture.CreateRewriter(
                program.CreateImageReader(segment.Address + a),
                program.Architecture.CreateProcessorState(),
                program.Architecture.CreateFrame(),
                host);
            return rw.FirstOrDefault();
        }

        public IEnumerable<Address> SpeculateCallDests(Dictionary<ImageMapSegment, byte[]> map)
        {
            var q = from addr in GetCalledAddresses(map)
                    group addr by addr into g
                    orderby g.Count(), g.Key
                    select g.Key;
            return q;
        }

        /// <summary>
        /// Find all addresses that appear to be the destination of a call 
        /// instruction.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public IEnumerable<Address> GetCalledAddresses(Dictionary<ImageMapSegment, byte[]> map)
        { 
            foreach (var item in map)
            {
                for (int a = 0; a < item.Value.Length; ++a)
                {
                    if (item.Value[a] != MaybeCode)
                        continue;
                    var i = Dasm(item.Key, a);
                    if ((i.InstructionClass & InstructionClass.Call) != 0)
                    {
                        var dest = Destination(i);
                        if (dest != null && IsExecutable(dest))
                        {
                            yield return dest;
                        }
                    }
                }
            }
        }
    }
}
