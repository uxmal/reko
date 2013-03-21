#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Decompiler.Scanning
{
	/// <summary>
	/// In the absence of any other information, scans address ranges in search of code sequences that may represent
	/// valid procedures. It needs help from the processor architecture to specify what byte patterns to look for.
	/// </summary>
	public class HeuristicScanner
	{
		private Program prog;

		public HeuristicScanner(Program prog)
		{
			this.prog = prog;
		}

        public IEnumerable<Address> FindCallOpcodesBackwards(Address address)
        {
            int procOffset = address - prog.Image.BaseAddress;
            int offset = procOffset
                    - (4) // Address length 
                        - 1;  // opcode 
            while (offset >= 0)
            {
                if (prog.Image.Bytes[offset] == 0xE8)       // CALL NEAR
                {
                    int callOffset = prog.Image.ReadLeInt32((uint) offset + 1);
                    int targetOffset = offset + callOffset + 1 + 4;
                    if (targetOffset == procOffset)
                        yield return prog.Image.BaseAddress + offset;
                }
                --offset;
            }
        }

        public IEnumerable<uint> FindCallOpcodeLinearAddresses_32(IEnumerable<Address> procedureEntryAddresses)
        {
            var procEntryLinearAddresses = procedureEntryAddresses
                .Select(addr => addr.Linear)
                .Aggregate(
                    new HashSet<uint>(),
                    (set, linear) => { set.Add(linear); return set; });
            var rdr = prog.Image.CreateReader(0);
            uint linBase = prog.Image.BaseAddress.Linear;
            while (rdr.IsValid)
            {
                uint callingOffset = rdr.Offset;
                var opcode = rdr.ReadByte();
                if (opcode == 0xE8 && rdr.IsValidOffset(rdr.Offset + 4u))         // CALL NEAR
                {
                    int callOffset = prog.Image.ReadLeInt32(rdr.Offset);
                    uint target = (uint)(linBase + callOffset + rdr.Offset + 4);
                    if (procEntryLinearAddresses.Contains(target))
                        yield return callingOffset + linBase;
                }
            }
        }

        public IEnumerable<uint> FindCallOpcodeLinearAddresses_16(IEnumerable<Address> procedureEntryAddresses)
        {
            var procEntryLinearAddresses = procedureEntryAddresses
                .Select(addr => addr.Linear)
                .Aggregate(
                    new HashSet<uint>(),
                    (set, linear) => { set.Add(linear); return set; });
            var rdr = prog.Image.CreateReader(0);
            uint linBase = prog.Image.BaseAddress.Linear;
            while (rdr.IsValid)
            {
                uint callingOffset = rdr.Offset;
                var opcode = rdr.ReadByte();
                if (opcode == 0xE8 && rdr.IsValidOffset(rdr.Offset + 2u))         // CALL NEAR
                {
                    int callOffset = prog.Image.ReadLeInt16(rdr.Offset);
                    uint target = (uint) (linBase + callOffset + rdr.Offset + 2);
                    if (procEntryLinearAddresses.Contains(target))
                        yield return callingOffset + linBase;
                }
                else if (opcode == 0x9A && rdr.IsValidOffset(rdr.Offset + 4u))     // CALL FAR
                {
                    uint off = prog.Image.ReadLeUInt16(rdr.Offset);
                    uint seg = prog.Image.ReadLeUInt16(rdr.Offset + 2);
                    if (procEntryLinearAddresses.Contains((seg << 4) + off))
                        yield return callingOffset + linBase;
                }
            }
        }

        public IEnumerable<uint> FindCallOpcodeLinearAddresses_Arm32(IEnumerable<Address> procedureEntryAddresses)
        {
            var procEntryLinearAddresses = procedureEntryAddresses
                            .Select(addr => addr.Linear)
                            .Aggregate(
                                new HashSet<uint>(),
                                (set, linear) => { set.Add(linear); return set; });
            var rdr = prog.Image.CreateReader(0);
            uint linBase = prog.Image.BaseAddress.Linear;
            while (rdr.IsValid)
            {
                uint callOffset = rdr.Offset;
                var opcode = rdr.ReadLeUInt32();
                if ((opcode & 0x0F000000) == 0x0B000000)         // BL
                {
                    int offset = ((int)opcode << 8) >> 6;
                    uint target = (uint)(linBase + callOffset + 8 + offset);
                    if (procEntryLinearAddresses.Contains(target))
                        yield return callOffset + linBase;
                }
            }
        }
    }
}
