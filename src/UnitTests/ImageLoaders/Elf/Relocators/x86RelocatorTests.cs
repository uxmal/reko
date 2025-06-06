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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Loading;
using Reko.ImageLoaders.Elf;
using Reko.ImageLoaders.Elf.Relocators;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.ImageLoaders.Elf.Relocators
{
    [TestFixture]
    public class x86RelocatorTests : ElfRelocatorTestBase
    {
        protected override Address GetLoadAddress()
        {
            return Address.Ptr32(0x1000);
        }

        protected override IProcessorArchitecture GetArchitecture()
        {
            return new Reko.Arch.X86.X86ArchitectureFlat32(new ServiceContainer(), "x86-flat-32", new Dictionary<string, object>());
        }

        protected override ElfRelocator CreateRelocator(ElfLoader loader, SortedList<Address, ImageSymbol> imageSymbols)
        {
            return new x86Relocator((ElfLoader32) loader, imageSymbols);
        }

        protected override ElfLoader CreateLoader()
        {
            var hdr = new ElfHeader
            {
                BinaryFileType = BinaryFileType.Executable,
            };
            var bin = new ElfBinaryImage(
                ImageLocation.FromUri("test.elf"),
                hdr,
                EndianServices.Little);
            return new ElfLoader32(
                new ServiceContainer(),
                bin,
                []);
        }

        [Test]
        public void ElfRel_x86_NullSymbolValue()
        {
            var sym = Given_symbol(3, "puts", ElfSymbolType.STT_FUNC, 0, 0);

            Given_section(".plt", 0x1000, 0x100);

            Given_section(".got.plt", 0x4000, 0x100)
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00001036)  // Points 6 bytes into PLT stub at 0x1030<16>
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00123400);

            var rel = Given_rel(0x400C, sym: 3, type: i386Rt.R_386_JMP_SLOT);

            Given_Relocator();
            Given_Context();

            context.Update(rel, sym);
            var (addr, symNew) = relocator.RelocateEntry(context, rel, sym);

            Assert.AreEqual(0x400Cu, addr.Value.ToUInt32());
            Assert.AreEqual("puts", symNew.Name);
        }
    }
}
