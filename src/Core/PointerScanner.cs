#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
        private EndianImageReader rdr;
        private HashSet<T> knownLinAddresses;
        private PointerScannerFlags flags;

        public PointerScanner(EndianImageReader rdr, HashSet<T> knownLinAddresses, PointerScannerFlags flags)
        {
            this.rdr = rdr;
            this.knownLinAddresses = knownLinAddresses;
            this.flags = flags;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this, this.rdr.Clone());
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        private class Enumerator : IEnumerator<T>
        {
            private PointerScanner<T> scanner;
            private EndianImageReader r;

            public Enumerator(PointerScanner<T> scanner, EndianImageReader rdr)
            {
                this.scanner = scanner;
                this.r = rdr;
            }

            public T Current { get; set; }

            object System.Collections.IEnumerator.Current { get { return Current; } }

            public bool MoveNext()
            {
                while (r.IsValid)
                {
                    var rdr = this.r;
                    T linAddrInstr;
                    if (scanner.ProbeForPointer(rdr, out linAddrInstr))
                    {
                        Current = linAddrInstr;
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

        public virtual bool ProbeForPointer(EndianImageReader rdr, out T linAddrInstr)
        {
            linAddrInstr = GetLinearAddress(rdr.Address);
            T target;
            uint opcode;
            if (TryPeekOpcode(rdr, out opcode))
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

        public abstract T GetLinearAddress(Address address);

        public abstract int PointerAlignment { get; }

        /// <summary>
        /// The implementations of this abstract method should read a chunk of bytes
        /// equal to the size of an opcode in the relevant architecture.
        /// </summary>
        /// <remarks>Most architectures have opcode whose size <= 32 bits, which should
        /// fit comfortably in a System.UInt32.</remarks>
        /// <param name="rdr"></param>
        /// <returns>The opcode at the current position of the reader.</returns>
        public abstract bool TryPeekOpcode(EndianImageReader rdr, out uint opcode);

        public abstract bool TryPeekPointer(EndianImageReader rdr, out T target);

        public abstract bool MatchCall(EndianImageReader rdr, uint opcode, out T target);

        public abstract bool MatchJump(EndianImageReader rdr, uint opcode, out T target);
    }
}
