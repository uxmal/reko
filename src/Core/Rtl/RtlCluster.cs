#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
    public sealed class RtlInstructionCluster
    {
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

        public InstrClass Class { get; set; }

        public RtlInstruction[] Instructions { get; }

        /// <summary>
        /// The length of the original machine instruction, in bytes.
        /// </summary>
        public int Length { get; }

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
