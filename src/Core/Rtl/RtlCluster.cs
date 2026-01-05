#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using System.IO;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// A RtlInstructionCluster contains the <see cref="RtlInstruction"/> that are generated when 
    /// a machine instruction is rewritten.
    /// </summary>
    public sealed class RtlInstructionCluster : IAddressable
    {
        /// <summary>
        /// Constructs a new <see cref="RtlInstructionCluster"/> instance.
        /// </summary>
        /// <param name="addr">Address from which this cluster was lifted.</param>
        /// <param name="instrLength">The length of the machine code instruction
        /// that was lifted to this cluster.</param>
        /// <param name="instrs">Array of RTL instructions for this cluster.
        /// </param>
        public RtlInstructionCluster(Address addr, int instrLength, params RtlInstruction[] instrs)
        {
            this.Address = addr;
            this.Length = instrLength;
            this.Instructions = instrs;
        }

        /// <summary>
        /// The address of the original machine instruction.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// The instruction class of the original machine instruction.
        /// </summary>
        public InstrClass Class { get; set; }

        /// <summary>
        /// The instructions that were generated when the original machine instruction
        /// was lifted.
        /// </summary>
        public RtlInstruction[] Instructions { get; }

        /// <summary>
        /// The length of the original machine instruction, in bytes.
        /// </summary>
        public int Length { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}({1}): {2} instructions", Address, Length, Instructions.Length);
        }

        /// <inheritdoc/>
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
