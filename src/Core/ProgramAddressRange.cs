#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
    /// <summary>
    /// Represents a range of addresses inside the address space of a <see cref="Program"/>.
    /// </summary>
    public class ProgramAddressRange
    {
        private ProgramAddressRange(Program program, Address address, long length)
        {
            this.Program = program;
            this.Address = address;
            this.Length = length;
        }

        public static ProgramAddressRange Create(Program program, AddressRange range)
        {
            return ProgramAddressRange.HalfOpenRange(program, range.Begin, range.End);
        }

        public static ProgramAddressRange Create(Program program, Address address, long length)
        {
            return new ProgramAddressRange(program, address, length);
        }

        public static ProgramAddressRange Empty(Program program, Address address)
        {
            return new ProgramAddressRange(program, address, 0);
        }

        /// <summary>
        /// Creates a half open range from two <see cref="Address"/>es. The addresses
        /// can be passed in any order; the lowest address will become the start of
        /// the range. The result will represent either the range [addr1-addr2) or
        /// [addr2-addr1), depending the order of the addresses.
        /// </summary>
        /// <param name="program"><see cref="Program"/> instance.</param>
        /// <param name="addr1">One address.</param>
        /// <param name="addr2">Second address.</param>
        /// <returns>A <see cref="ProgramAddressRange"/> instance which treats the 
        /// addresses as a half open range.</returns>
        public static ProgramAddressRange HalfOpenRange(Program program, Address addr1, Address addr2)
        {
            long length = addr2 - addr1;
            if (length >= 0)
            {
                return new ProgramAddressRange(program, addr1, length);
            }
            else
            {
                return new ProgramAddressRange(program, addr2, -length);
            }
        }

        /// <summary>
        /// Creates a closed range from two <see cref="Address"/>es. The addresses
        /// can be passed in any order; the lowest address will become the start of
        /// the range. The result will represent either the range [addr1-addr2] or
        /// [addr2-addr1], depending the order of the addresses.
        /// </summary>
        /// <param name="program"><see cref="Program"/> instance.</param>
        /// <param name="addr1">One address.</param>
        /// <param name="addr2">Second address.</param>
        /// <returns>A <see cref="ProgramAddressRange"/> instance which treats the 
        /// addresses as a closed range.</returns>
        public static ProgramAddressRange ClosedRange(Program program, Address addr1, Address addr2)
        {
            long length = 1 + (addr2 - addr1);
            if (length >= 0)
            {
                return new ProgramAddressRange(program, addr1, length);
            }
            else
            {
                return new ProgramAddressRange(program, addr2, 2 - length);
            }
        }

        /// <summary>
        /// The <see cref="Program"/> instance inside whose address space contains 
        /// the <see cref="Address"/> property.
        /// </summary>
        public Program Program { get; }

        /// <summary>
        /// The starting address of the range.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// The length of the range. A value of 0 indicates an empty range.
        /// </summary>
        //$REVIEW: We cannot represent ranges longer that 2^63-1. Will this 
        // be a problem? Even with ulong, we cannot represent the range
        // corresponding to a full 64-bit address space without using BigInteger.
        public long Length { get; }

        public override bool Equals(object? obj)
        {
            if (obj is not ProgramAddressRange that)
                return false;
            return this == that;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Program, this.Address, this.Length);
        }

        public static bool operator==(ProgramAddressRange left, ProgramAddressRange right)
        {
            return
                left.Program == right.Program &&
                left.Length ==  right.Length &&
                left.Address == right.Address;
        }

        public static bool operator !=(ProgramAddressRange left, ProgramAddressRange right) =>
            !(left == right);


        public override string ToString()
        {
            if (Length > 1)
            {
                return $"{Program.Name}: {Address} ({Length})";
            }
            else
            {
                return $"{Program.Name}: {Address}";
            }
        }
    }
}
