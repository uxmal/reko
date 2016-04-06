#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class PpcRelocator : ElfRelocator
    {
        private ElfLoader32 loader;
        private Dictionary<Address, ImportReference> importReferences;

        public PpcRelocator(ElfLoader32 loader, Dictionary<Address, ImportReference> importReferences)
        {
            this.loader = loader;
            this.importReferences = importReferences;
        }

        /// <remarks>
        /// According to the ELF PPC32 documentation, the .rela.plt and .plt tables 
        /// should contain the same number of entries, even if the individual entry 
        /// sizes are distinct. The entries in .real.plt refer to symbols while the
        /// entries in .plt are (writeable) pointers.  Any caller that jumps to one
        /// of pointers in the .plt table is a "trampoline", and should be replaced
        /// in the decompiled code with just a call to the symbol obtained from the
        /// .real.plt section.
        /// </remarks>
        public override void Relocate(Program program)
        {
            var rela_plt = loader.GetSectionInfoByName(".rela.plt");
            var plt = loader.GetSectionInfoByName(".plt");
            var relaRdr = loader.CreateReader(rela_plt.FileOffset);
            var pltRdr = loader.CreateReader(plt.FileOffset);
            for (int i = 0; i < rela_plt.EntryCount(); ++i)
            {
                // Read the .rela.plt entry
                uint offset;
                if (!relaRdr.TryReadUInt32(out offset))
                    return;
                uint info;
                if (!relaRdr.TryReadUInt32(out info))
                    return;
                int addend;
                if (!relaRdr.TryReadInt32(out addend))
                    return;

                // Read the .plt entry. We don't care about its contents,
                // only its address. Anyone accessing that address is
                // trying to access the symbol.

                uint thunkAddress;
                if (!pltRdr.TryReadUInt32(out thunkAddress))
                    break;

                uint sym = info >> 8;
                string symStr = loader.GetSymbolName(rela_plt.LinkedSection, sym);

                var addr = plt.Address + (uint)i * 4;
                importReferences.Add(
                    addr,
                    new NamedImportReference(addr, null, symStr));
            }
        }
    }
}
