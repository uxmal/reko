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

using NUnit.Framework;
using NUnit.Framework.Constraints;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.ImageLoaders.Elf;
using Reko.ImageLoaders.Elf.Relocators;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.ImageLoaders.Elf.Relocators
{
    [TestFixture]
    public class MipsRelocatorTests : ElfRelocatorTestBase
    {
        protected override Address GetLoadAddress()
        {
            return Address.Ptr32(0x1000);
        }

        protected override IProcessorArchitecture GetArchitecture()
        {
            return new Reko.Arch.Mips.MipsBe32Architecture(
                new ServiceContainer(),
                "mips-be-32", 
                new Dictionary<string, object>()
                {
                    { ProcessorOption.Endianness, "be" }
                });
        }

        protected override ElfRelocator CreateRelocator(ElfLoader loader, SortedList<Address, ImageSymbol> imageSymbols)
        {
            var arch = GetArchitecture();
            return new MipsRelocator((ElfLoader32) loader, arch, imageSymbols);
        }

        protected override ElfLoader CreateLoader()
        {
            var hdr = new ElfHeader();
            var bin = new ElfBinaryImage(
                ImageLocation.FromUri("test.elf"),
                hdr,
                EndianServices.Big);
            return new ElfLoader32(
                new ServiceContainer(),
                bin,
                Array.Empty<byte>());
        }

        [Test]
        public void ElfRel_Mips_Hi16_Lo16()
        {
            var sym = Given_symbol(3, "puts", ElfSymbolType.STT_FUNC, 0, 0x21234);

            Given_section(".text", 0x4000, 0x100)
                .WriteBeUInt32(0x00000000)
                .WriteBeUInt32(0x00000000)
                .WriteBeUInt32(0x00000000)
                .WriteBeUInt32(0x13378100)  // This is the high part that gets updated.
                .WriteBeUInt32(0x13378100)  // This is the low part that gets updated.
                .WriteBeUInt32(0x00000000)
                .WriteBeUInt32(0x00000000);

            var addrHi = Address.Ptr32(0x400C);
            var addrLo = Address.Ptr32(0x4010);
            var rels = new[]
            {
                Given_rel((uint)addrHi.Offset, sym: 3, type: MIPSrt.R_MIPS_HI16),
                Given_rel((uint)addrLo.Offset, sym: 3, type: MIPSrt.R_MIPS_LO16)
            };
            Given_Relocator();
            Given_Context();

            foreach (var rel in rels)
            {
                if (context.Update(rel, sym))
                    relocator.RelocateEntry(context, rel, sym);
            }

            program.SegmentMap.TryFindSegment(addrHi, out var seg);
            Assert.IsNotNull(seg);
            seg.MemoryArea.TryReadBeUInt32(addrHi, out var hiValue);
            seg.MemoryArea.TryReadBeUInt32(addrLo, out var loValue);
            Assert.AreEqual(0x13378102, hiValue, $"Was {hiValue:X8}");
            Assert.AreEqual(0x13379334, loValue, $"Was {loValue:X8}");
        }

        [Test(Description = "Handle 'orphaned' R_MIPS_LO16 relocations according to the spec")]
        public void ElfRel_Mips_orphaned_Lo16()
        {
            var sym = Given_symbol(3, "puts", ElfSymbolType.STT_FUNC, 0, 0x21234);

            Given_section(".text", 0x4000, 0x100)
                .WriteBeUInt32(0x00000000)
                .WriteBeUInt32(0x00000000)
                .WriteBeUInt32(0x00000000)
                .WriteBeUInt32(0x13378100)  // This is the high part that gets updated.
                .WriteBeUInt32(0x13378100)  // This is the low part that gets updated.
                .WriteBeUInt32(0x13378100)  // This is the orphaned part also updated.
                .WriteBeUInt32(0x00000000);

            var addrHi = Address.Ptr32(0x400C);
            var addrLo = Address.Ptr32(0x4010);
            var addrOr = Address.Ptr32(0x4014);
            var rels = new[]
            {
                Given_rel((uint)addrHi.Offset, sym: 3, type: MIPSrt.R_MIPS_HI16),
                Given_rel((uint)addrLo.Offset, sym: 3, type: MIPSrt.R_MIPS_LO16),
                Given_rel((uint)addrOr.Offset, sym: 3, type: MIPSrt.R_MIPS_LO16)
            };
            Given_Relocator();
            Given_Context();

            foreach (var rel in rels)
            {
                if (context.Update(rel, sym))
                    relocator.RelocateEntry(context, rel, sym);
            }

            program.SegmentMap.TryFindSegment(addrHi, out var seg);
            Assert.IsNotNull(seg);
            seg.MemoryArea.TryReadBeUInt32(addrHi, out var hiValue);
            seg.MemoryArea.TryReadBeUInt32(addrLo, out var loValue);
            seg.MemoryArea.TryReadBeUInt32(addrOr, out var orValue);
            Assert.AreEqual(0x13378102, hiValue, $"Was {hiValue:X8}");
            Assert.AreEqual(0x13379334, loValue, $"Was {loValue:X8}");
            Assert.AreEqual(0x13379334, orValue, $"Was {orValue:X8}");
        }

        [Test(Description = "Some relocations have 0 addresses.")]
        public void ElfRel_Mips_None()
        {
            var symbol = Given_symbol(0, "", ElfSymbolType.STT_NOTYPE, 0, 0);
            var rel = Given_rel(0, 0, type: MIPSrt.R_MIPS_NONE);
            Given_Relocator();
            Given_Context();

            context.Update(rel, symbol);
            relocator.RelocateEntry(context, rel, symbol);
        }
    }
}

