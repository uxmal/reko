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

        private const RtlClass L = RtlClass.Linear;
        private const RtlClass T = RtlClass.Transfer;
        
        private const RtlClass CL = RtlClass.Linear | RtlClass.Conditional;
        private const RtlClass CT = RtlClass.Transfer | RtlClass.Conditional;
        
        private const RtlClass DT = RtlClass.Transfer | RtlClass.Delay;
        private const RtlClass DCT = RtlClass.Transfer | RtlClass.Conditional | RtlClass.Delay;

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
            var map = new Dictionary<ImageMapSegment, byte[]>();
            foreach (var segment in program.ImageMap.Segments.Values
                .Where(s => (s.Access & AccessMode.Execute) != 0))
            {
                var y = ScanSegment(segment);
                map.Add(segment, y);
            }

            return SpeculateCallDests(map);
        }

        /// <summary>
        /// Disassemble every byte of the image, marking those addresses that likely
        /// are code as MaybeCode, everything else as data.
        /// </summary>
        /// <param name="segment"></param>
        public byte[] ScanSegment(ImageMapSegment segment)
        {
            var addrBase = segment.Address;

            var G = new DiGraph<Address>();
            G.AddNode(bad);
            var y = new byte[segment.ContentSize];
            var step = program.Architecture.InstructionBitSize / 8;
            bool inDelaySlot = false;
            for (var a = 0; a < y.Length; a += step)
            {
                y[a] = MaybeCode;
                var i = Dasm(segment, a);
                var r = i.Instructions.Last();
                if (i == null || r is RtlInvalid)
                    AddEdge(G, bad, i.Address);
                if (MayFallThrough(i, r))
                { 
                    if (!inDelaySlot)
                    {
                        if (a + i.Length < y.Length)
                            AddEdge(G, i.Address + i.Length, i.Address);
                        else
                            AddEdge(G, bad, i.Address);
                    }
                }
                var tr = r as RtlTransfer;
                if (tr != null)
                {
                    var dest = Destination(tr);
                    if (dest != null)
                    {
                        if (IsExecutable(dest))
                            AddEdge(G, dest, i.Address);
                        else
                            AddEdge(G, bad, i.Address);
                    }
                }
                // If this is a delayed unconditional branch...
                inDelaySlot = i.Class == DT;
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

        private bool MayFallThrough(RtlInstructionCluster i, RtlInstruction r)
        {
            return r is RtlAssignment
                || r is RtlBranch
                || r is RtlCall;        //$REVIEW: what if you call a terminating function.
        }

        private bool IsTransfer(RtlInstructionCluster i, RtlInstruction r)
        {
            return r is RtlGoto || r is RtlCall;
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

        private Address Destination(RtlTransfer r)
        {
            return r.Target as Address;
        }

        private RtlInstructionCluster Dasm(ImageMapSegment segment, int a)
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

        public IEnumerable<Address> GetCalledAddresses(Dictionary<ImageMapSegment, byte[]> map)
        { 
            foreach (var item in map)
            {
                for (int a = 0; a < item.Value.Length; ++a)
                {
                    if (item.Value[a] != MaybeCode)
                        continue;
                    var i = Dasm(item.Key, a);
                    var r = i.Instructions.Last();
                    var c = r as RtlCall;
                    if (c != null)
                    {
                        var dest = Destination(c);
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
