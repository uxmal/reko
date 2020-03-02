using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// A RtlInstructionCluster contains the RtlInstructions that are generated when 
    /// a machine instruction is rewritten.
    /// </summary>
    public class RtlInstructionCluster
    {
        public RtlInstructionCluster(Address addr, int instrLength, params RtlInstruction[] instrs)
        {
            this.Address = addr;
            this.Length = (byte) instrLength;
            this.Instructions = instrs;
        }

        /// <summary>
        /// The address of the original machine instruction.
        /// </summary>
        public Address Address { get; private set; }

        public InstrClass Class { get; set; }

        public RtlInstruction[] Instructions { get; private set; }

        /// <summary>
        /// The length of the original machine instruction, in bytes.
        /// </summary>
        public byte Length { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}({1}): {2} instructions", Address, Length, Instructions.Length);
        }

        public void Write(TextWriter writer)
        {
            writer.WriteLine("{0}({1}):", Address, Length);
            foreach (var ri in Instructions)
            {
                ri.Write(writer);
                writer.WriteLine();
            }
        }

    }
}
