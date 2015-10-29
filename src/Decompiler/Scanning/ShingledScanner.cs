using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
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
        const byte MaybeCode = 1;
        const byte Data = 0;

        private Program program;
        private readonly Address bad;

        public ShingledScanner(Program program)
        {
            this.program = program;
            this.bad = program.Platform.MakeAddressFromLinear(~0u);
        }

        public void Scan()
        {
            foreach (var segment in program.ImageMap.Segments.Values
                .Where(s => (s.Access & AccessMode.Execute) != 0))
            {
                ScanSegment(segment);
            }
        }

        /// <summary>
        /// Disassemble every byte of the image, but discard
        /// </summary>
        /// <param name="segment"></param>
        public byte[] ScanSegment(ImageMapSegment segment)
        {
            const InstructionClass L = InstructionClass.Linear;
            const InstructionClass T = InstructionClass.Transfer;
            const InstructionClass TK = InstructionClass.Transfer | InstructionClass.Call;

            const InstructionClass CL = InstructionClass.Linear | InstructionClass.Conditional;
            const InstructionClass CT = InstructionClass.Transfer | InstructionClass.Conditional;

            const InstructionClass DT = InstructionClass.Transfer | InstructionClass.Delay;
            const InstructionClass DCT = InstructionClass.Transfer | InstructionClass.Conditional | InstructionClass.Delay;
            const InstructionClass DTK = InstructionClass.Transfer  | InstructionClass.Call | InstructionClass.Delay;

            var addrBase = segment.Address;

            var G = new DiGraph<Address>();
            G.AddNode(bad);
            var y = new byte[segment.Size];
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
    }
}
