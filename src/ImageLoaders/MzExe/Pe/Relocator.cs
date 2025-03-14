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
using Reko.Core.Collections;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.MzExe.Pe
{
    public abstract class Relocator
    {
        protected Program program;

        public Relocator(Program program)
        {
            this.program = program;
        }

        public abstract void ApplyRelocation(Address baseOfImage, uint page, EndianImageReader rdr, RelocationDictionary relocations);

        protected EndianImageReader CreateImageReader(Program program, Address addr)
        {
            //$BUG: this returns null. Need to change the logic to more robustly handle
            // maformed
            if (!program.TryCreateImageReader(addr, out var rdr))
                throw new BadImageFormatException();
            return rdr;
        }

        protected EndianImageReader CreateImageReader(Program program, IProcessorArchitecture arch, Address addr)
        {
            //$BUG: this returns null. Need to change the logic to more robustly handle
            // maformed
            if (!program.TryCreateImageReader(arch, addr, out var rdr))
                throw new BadImageFormatException();
            return rdr;
        }

    }
}
