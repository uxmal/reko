#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using System.Linq;
using System.Text;

namespace Decompiler.Arch.X86
{
    public class PointerScanner : IEnumerable<uint>
    {
        private ImageReader rdr;
        private HashSet<uint> knownLinAddresses;
        private PointerScannerFlags flags;

        public PointerScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
        {
            this.rdr = rdr;
            this.knownLinAddresses = knownLinAddresses;
            this.flags = flags;
        }

        public IEnumerator<uint> GetEnumerator()
        {
            return new Enumerator(this, this.rdr.Clone());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

        private class Enumerator : IEnumerator<uint>
        {
            private uint target;
            private PointerScanner scanner;
            private ImageReader rdr;

            public Enumerator(PointerScanner scanner, ImageReader rdr)
            {
                this.scanner = scanner;
                this.rdr = rdr;
            }

            public uint Current { get; set; }

            object System.Collections.IEnumerator.Current { get { return Current; } }

            public bool MoveNext()
            {
                while (rdr.IsValid)
                {
                    uint linAddrInstr = rdr.Address.Linear;
                    var opcode = rdr.ReadByte(0);
                    if (MatchCode(scanner.flags, rdr, opcode) && scanner.knownLinAddresses.Contains(target))
                    {
                        rdr.Seek(1);
                        Current = linAddrInstr;
                        return true;
                    }
                    else if ((scanner.flags & PointerScannerFlags.Pointers) != 0)
                    {
                        if (MatchPointer(rdr))
                        {
                            rdr.Seek(1);
                            Current = linAddrInstr;
                            return true;
                        }
                    }
                    rdr.Seek(1);
                }
                return false;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public void Dispose() { }

            private bool MatchPointer(ImageReader rdr)
            {
                if (!rdr.IsValidOffset(rdr.Offset + 4-1))
                    return false;
                return scanner.knownLinAddresses.Contains(rdr.ReadLeUInt32(0));
            }

            private bool MatchCode(PointerScannerFlags flags, ImageReader rdr, byte opcode)
            {
                if ((flags & PointerScannerFlags.Calls) != 0
                    &&
                    opcode == 0xE8 // CALL NEAR
                    &&
                    rdr.IsValidOffset(rdr.Offset + 4u))
                {
                    int callOffset = rdr.ReadLeInt32(1);
                    target = (uint) (callOffset + rdr.Address.Linear + 5);
                    return true;
                }
                if ((flags & PointerScannerFlags.Jumps) != 0)
                {
                    if (opcode == 0xE9 // JMP NEAR
                        &&
                        rdr.IsValidOffset(rdr.Offset + 5u))
                    {
                        int callOffset = rdr.ReadLeInt32(1);
                        target = (uint) (callOffset + rdr.Address.Linear + 5);
                        return true;
                    }
                    if (0x70 <= opcode && opcode <= 0x7F &&       // short branch.
                        rdr.IsValidOffset(rdr.Offset + 1u))
                    {
                        sbyte callOffset = rdr.ReadSByte(1);
                        target = (uint) (rdr.Address.Linear + callOffset + 2);
                        return true;
                    }
                    if (opcode == 0x0F && rdr.IsValidOffset(rdr.Offset + 5u))
                    {
                        opcode = rdr.ReadByte(1);
                        int callOffset = rdr.ReadLeInt32(2);
                        uint linAddr = rdr.Address.Linear;
                        if (0x80 <= opcode && opcode <= 0x8F)   // long branch
                        {
                            target = (uint) (callOffset + linAddr + 6);
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
