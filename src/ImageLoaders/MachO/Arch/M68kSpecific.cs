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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Configuration;

namespace Reko.ImageLoaders.MachO.Arch
{
    public class M68kSpecific : ArchSpecific
    {
        public M68kSpecific(IProcessorArchitecture arch) : base(arch)
        {
        }

        public override Address ReadStub(Address addrStub, MemoryArea mem)
        {
            var offsetInSection = (uint) (addrStub - mem.BaseAddress);
            var opcode = mem.ReadBeUInt16(offsetInSection);
            // Expect move.l offset(pc),a0
            if (opcode != 0x207B)
                return null;
            var offsetToGotEntry = mem.ReadBeInt32(offsetInSection + 4);
            var addrGotEntry = addrStub + 2 + offsetToGotEntry;
            return addrGotEntry;
        }
    }
}
