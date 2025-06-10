#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Scanning
{
    /// <summary>
    /// A basic block of RTL clusters.
    /// </summary>
    public class RtlBlock
    {
        private RtlBlock(
            IProcessorArchitecture arch,
            Address addr,
            string id,
            int length,
            Address addrFallThrough,
            ProvenanceType provenance,
            List<RtlInstructionCluster> instructions)
        {
            this.Architecture = arch;
            this.Name = id;
            this.Address = addr;
            this.Length = length;
            this.FallThrough = addrFallThrough;
            this.Provenance = provenance;
            this.Instructions = instructions;
            this.IsValid = true;
        }

        public static RtlBlock Create(
            IProcessorArchitecture arch,
            Address addr,
            string id,
            int length,
            Address addrFallThrough,
            ProvenanceType provenance,
            List<RtlInstructionCluster> instructions)
        {
            return new RtlBlock(arch, addr, id, length, addrFallThrough, provenance, instructions);
        }


        public static RtlBlock CreateEmpty(IProcessorArchitecture arch, Address addr, string id)
        {
            return new RtlBlock(
                arch,
                addr,
                id,
                0,
                default!,
                ProvenanceType.None,
                []);
        }

        public static RtlBlock CreatePartial(
            IProcessorArchitecture arch,
            Address addr,
            string id,
            List<RtlInstructionCluster> clusters)
        {
            return new RtlBlock(
                arch,
                addr,
                id,
                0,
                default!,
                ProvenanceType.None,
                clusters);
        }

        /// <summary>
        /// Address at which the block starts.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// CPU architecture used to disassemble this block.
        /// </summary>
        public IProcessorArchitecture Architecture { get; }

        /// <summary>
        /// The address after the block if control flow falls through.
        /// Note that this is not necessarily <see cref="Address"/> + <see cref="Length"/>, because
        /// control instructions with delay slots may require skipping one extra instruction.
        /// </summary>
        public Address FallThrough { get; set; }

        /// <summary>
        /// Indicates whether this block is valid or not.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// The size of the basic block starting 
        /// at <see cref="Address"/> and including the length of the final
        /// instruction, but not including any "stolen" delay slots.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Invariant identifier used for this block.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The provenance of this block; i.e. how it was found.
        /// </summary>
        public ProvenanceType Provenance { get; set; }

        /// <summary>
        /// The instructions this block consists of.</param>
        /// <summary>
        public List<RtlInstructionCluster> Instructions { get; init; }

        /// <summary>
        /// True if this block has been identified as a shared exit block
        /// of a procedure.
        /// </summary>
        public bool IsSharedExitBlock { get; set; }

        /// <summary>
        /// Get the address of the instruction immediately 
        /// following this basic block, including any
        /// stolen delay slot.
        /// </summary>
        /// <returns>The <see cref="Address"/> following
        /// the basic block.
        /// </returns>
        //$REVIEW: isn't this th same as FallThrough?
        public Address GetEndAddress()
        {
            int iLast = Instructions.Count - 1;
            if (iLast < 0)
                return Address;
            var instr = Instructions[iLast];
            return instr.Address + instr.Length;
        }

        public override string ToString()
        {
            return string.Format("block({0})", Address);
        }

        public void MarkLastInstructionInvalid()
        {
            Debug.Assert(this.Instructions.Count > 0);
            int iLast = this.Instructions.Count - 1;
            var cluster = this.Instructions[iLast];
            this.Instructions.RemoveAt(iLast);
            this.Instructions.Add(new RtlInstructionCluster(
                cluster.Address,
                cluster.Length,
                new RtlInvalid()));
            this.IsValid = false;
        }
    }
}
