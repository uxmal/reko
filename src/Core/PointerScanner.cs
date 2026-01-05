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

using Reko.Core.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// PointerScanners are used by the user to guess at pointers based on bit
    /// patterns.
    /// </summary>
    /// <remarks>
    /// Each architecture should create its own class derived from this class
    /// and implement the abstract methods.
    /// </remarks>
    public abstract class PointerScanner<T> : IEnumerable<T>
    {
        private readonly EndianImageReader rdr;
        private readonly HashSet<T> knownLinAddresses;
        private readonly PointerScannerFlags flags;

        /// <summary>
        /// Initializes a pointer scanner instance.
        /// </summary>
        /// <param name="rdr">Image reader to read.</param>
        /// <param name="knownLinAddresses"></param>
        /// <param name="flags"></param>
        public PointerScanner(EndianImageReader rdr, HashSet<T> knownLinAddresses, PointerScannerFlags flags)
        {
            this.rdr = rdr;
            this.knownLinAddresses = knownLinAddresses;
            this.flags = flags;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this, this.rdr.Clone());
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        private struct Enumerator : IEnumerator<T>
        {
            private readonly PointerScanner<T> scanner;
            private readonly EndianImageReader r;
            private T current;

            public Enumerator(PointerScanner<T> scanner, EndianImageReader rdr)
            {
                this.scanner = scanner;
                this.r = rdr;
                this.current = default!;
            }

            public T Current => current;

            object IEnumerator.Current => current!;

            public bool MoveNext()
            {
                while (r.IsValid)
                {
                    var rdr = this.r;
                    if (scanner.ProbeForPointer(rdr, out T linAddrInstr))
                    {
                        current = linAddrInstr;
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public void Dispose() { }
        }

        /// <summary>
        /// Reads a word from the image reader and checks if they are pointers.
        /// </summary>
        /// <param name="rdr">Image readed to read from.</param>
        /// <param name="linAddrInstr">Resulting address of instruction.</param>
        /// <returns>True if a pointer appears to have been found; otherwise false.
        /// </returns>
        public virtual bool ProbeForPointer(EndianImageReader rdr, out T linAddrInstr)
        {
            linAddrInstr = GetLinearAddress(rdr.Address);
            T target;
            if (TryPeekOpcode(rdr, out uint opcode))
            {
                if ((flags & PointerScannerFlags.Calls) != 0)
                {
                    if (MatchCall(rdr, opcode, out target) && knownLinAddresses.Contains(target))
                    {
                        rdr.Seek(PointerAlignment);
                        return true;
                    }
                }
                if ((flags & PointerScannerFlags.Jumps) != 0)
                {
                    if (MatchJump(rdr, opcode, out target) && knownLinAddresses.Contains(target))
                    {
                        rdr.Seek(PointerAlignment);
                        return true;
                    }
                }
                if ((flags & PointerScannerFlags.Pointers) != 0)
                {
                    if (TryPeekPointer(rdr, out target) && knownLinAddresses.Contains(target))
                    {
                        rdr.Seek(PointerAlignment);
                        return true;
                    }
                }
            }
            rdr.Seek(PointerAlignment);
            return false;
        }

        /// <summary>
        /// Converts the given address to a linear address.
        /// </summary>
        /// <param name="address">Address given.</param>
        /// <returns>Linear counterpart.
        /// </returns>
        public abstract T GetLinearAddress(Address address);

        /// <summary>
        /// The alignment of pointers in the target architecture, expressed
        /// in storage units.
        /// </summary>
        public abstract int PointerAlignment { get; }

        /// <summary>
        /// The implementations of this abstract method should read a chunk of bytes
        /// equal to the size of an opcode in the relevant architecture.
        /// </summary>
        /// <remarks>Most architectures have opcode whose size &lt;= 32 bits, which should
        /// fit comfortably in a System.UInt32.</remarks>
        /// <param name="rdr">Image reader to read from.</param>
        /// <param name="opcode">The opcode at the current position of the reader.</param>
        /// <returns>True of the peek operation was successful, false otherwise.</returns>
        public abstract bool TryPeekOpcode(EndianImageReader rdr, out uint opcode);

        /// <summary>
        /// Reads a pointer-sized value from the image reader and checks if it is a pointer.
        /// </summary>
        /// <param name="rdr">Image reader to read from.</param>
        /// <param name="target">The read pointer-sized value.</param>
        /// <returns>True if a value could be read; otherwise false.</returns>
        public abstract bool TryPeekPointer(EndianImageReader rdr, out T target);

        /// <summary>
        /// Reads an instruction from the image reader and checks if it is a call.
        /// </summary>
        /// <param name="rdr">Image reader to read from.</param>
        /// <param name="opcode">Expected opcode.</param>
        /// <param name="target">The target of the call.</param>
        /// <returns>True if a value could be read; otherwise false.</returns>
        public abstract bool MatchCall(EndianImageReader rdr, uint opcode, out T target);

        /// <summary>
        /// Reads an instruction from the image reader and checks if it is a jump.
        /// </summary>
        /// <param name="rdr">Image reader to read from.</param>
        /// <param name="opcode">Expected opcode.</param>
        /// <param name="target">The target of the jump.</param>
        /// <returns>True if a value could be read; otherwise false.</returns>
        public abstract bool MatchJump(EndianImageReader rdr, uint opcode, out T target);
    }
}
