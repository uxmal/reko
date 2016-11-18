﻿#region License
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

using System;
using System.Collections.Generic;
using Reko.Core;

namespace Reko.ImageLoaders.Elf
{
    public class MipsRelocator : ElfRelocator32
    {
        private ElfLoader32 elfLoader;

        public MipsRelocator(ElfLoader32 elfLoader) : base(elfLoader)
        {
            this.elfLoader = elfLoader;
        }

        public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, Elf32_Rela rela)
        {
            throw new NotImplementedException();
        }

        public override string RelocationTypeToString(uint type)
        {
            throw new NotImplementedException();
        }
    }

    public class MipsRelocator64 : ElfRelocator64
    {
        private ElfLoader64 elfLoader;

        public MipsRelocator64(ElfLoader64 elfLoader) : base(elfLoader)
        {
            this.elfLoader = elfLoader;
        }

        public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, Elf64_Rela rela)
        {
            throw new NotImplementedException();
        }

        public override string RelocationTypeToString(uint type)
        {
            throw new NotImplementedException();
        }
    }
}