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
        const byte Data = 2;

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
        public void ScanSegment(ImageMapSegment segment)
        {
            var addrBase = segment.Address;

            var G = new DiGraph<Address>();
            var y = new byte[segment.Size];
            var step = program.Architecture.InstructionBitSize / 8;
            for (var a = 0; a < y.Length; a += step)
            {
                y[a] = MaybeCode;
                var i = Dasm(segment, a);
                if (i == null || !i.IsValid)
                    G.AddEdge(bad, i.Address);
                switch (i.InstructionClass)
                {
                case InstructionClass.Linear:
                case InstructionClass.Conditional | InstructionClass.Transfer:
                
                    if (a + i.Length < y.Length)
                        G.AddEdge(i.Address + i.Length, i.Address);
                    else
                        G.AddEdge(bad, i.Address);
                    break;
                }
                switch (i.InstructionClass)
                {
                case InstructionClass.Transfer:
                case InstructionClass.Call:
                case InstructionClass.Conditional | InstructionClass.Transfer:
                    if (IsExecutable(Destination(i)))
                        G.AddEdge(Destination(i), i.Address);
                    else
                        G.AddEdge(bad, i.Address);
                    break;
                }
            }
            foreach (var a in new DfsIterator<Address>(G).PreOrder(bad))
            {
                y[a - segment.Address] = Data;
            }
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

            return bad;
        }

        private MachineInstruction Dasm(ImageMapSegment segment, int a)
        {
            var dasm = program.CreateDisassembler(segment.Address + a);
            return dasm.FirstOrDefault();
        }
    }
}
