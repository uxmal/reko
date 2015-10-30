using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
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
        private const InstructionClass TK = InstructionClass.Transfer | InstructionClass.Call;
        
        private const InstructionClass CL = InstructionClass.Linear | InstructionClass.Conditional;
        private const InstructionClass CT = InstructionClass.Transfer | InstructionClass.Conditional;
        
        private const InstructionClass DT = InstructionClass.Transfer | InstructionClass.Delay;
        private const InstructionClass DCT = InstructionClass.Transfer | InstructionClass.Conditional | InstructionClass.Delay;
        private const InstructionClass DTK = InstructionClass.Transfer | InstructionClass.Call | InstructionClass.Delay;

        private Program program;
        private readonly Address bad;

        public ShingledScanner(Program program)
        {
            this.program = program;
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
        /// Disassemble every byte of the image, but discard
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
                if (i == null || !i.IsValid)
                    AddEdge(G, bad, i.Address);
                switch (i.InstructionClass)
                {
                case L:
                case CL:
                case CT:
                case DT:
                case DTK:
                case DCT:
                    if (!inDelaySlot)
                    {
                        if (a + i.Length < y.Length)
                            AddEdge(G, i.Address + i.Length, i.Address);
                        else
                            AddEdge(G, bad, i.Address);
                    }
                    break;
                }
                switch (i.InstructionClass)
                {
                case T:
                case TK:
                case CT:
                case DT:
                case DTK:
                case DCT:
                    var dest = Destination(i);
                    if (dest != null)
                    {
                        if (IsExecutable(dest))
                            AddEdge(G, dest, i.Address);
                        else
                            AddEdge(G, bad, i.Address);
                    }
                    break;
                }
                // If this is a delayed unconditional branch...
                inDelaySlot = i.InstructionClass == DT;
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
                    if (i.ToString().Contains("jal"))   //$DEBUG
                        i.ToString();
                    if ((i.InstructionClass & TK) == TK)
                    {
                        var dest = Destination(i);
                        if (dest != null && IsExecutable(dest))
                        {
                            if (dest.ToString().EndsWith("230"))    //$DEBUG
                                dest.ToLinear();
                            yield return dest;
                        }
                    }
                }
            }
        }
    }
}
