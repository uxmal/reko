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
using NUnit.Framework;
using Reko.Core;
using Reko.ImageLoaders.Elf;
using Reko.ImageLoaders.Elf.Relocators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.Elf.Relocators
{
    [TestFixture]
    public class AArch32RelocatorTests : ElfRelocatorTestBase
    {
        protected override ElfLoader CreateLoader()
        {
            return new ElfLoader32();
        }

        protected override ElfRelocator CreateRelocator(ElfLoader loader, SortedList<Address, ImageSymbol> imageSymbols)
        {
            return new ArmRelocator((ElfLoader32)loader, imageSymbols);
        }

        protected override IProcessorArchitecture GetArchitecture()
        {
            return new Reko.Arch.Arm.Arm32Architecture("arm32");
        }

        protected override Address GetLoadAddress()
        {
            return Address.Ptr32(0x00001000);
        }

        [Test]
        public void ElfRel_AArch32_R_ARM_GLOB_DAT()
        {
            var sym = Given_symbol(3, "myfn", ElfSymbolType.STT_FUNC, 0, 0x4000);

            Given_section(".text", 0x3000, 0x100);
            Given_section(".got", 0x4000, 0x100);

            var rel = Given_rel(0x4008, sym: 3, type: Arm32Rt.R_ARM_GLOB_DAT);

            Given_Relocator();

            var symNew = relocator.RelocateEntry(program, sym, null, rel);

            Assert.AreEqual(0x4000, (uint) symNew.Value);
        }
    }
}
