#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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
using System;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.PIC16
{
    public class PIC16PointerScanner : PointerScanner<uint>
    {
        public PIC16PointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override uint GetLinearAddress(Address address) => throw new NotImplementedException();

        public override int PointerAlignment => throw new NotImplementedException();

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode) => throw new NotImplementedException();
        public override bool TryPeekPointer(EndianImageReader rdr, out uint target) => throw new NotImplementedException();
        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target) => throw new NotImplementedException();
        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target) => throw new NotImplementedException();
    }
}
