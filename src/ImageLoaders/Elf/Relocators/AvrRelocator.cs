using System;
using Reko.Core;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class AvrRelocator : ElfRelocator32
    {
        public AvrRelocator(ElfLoader32 ldr) : base(ldr)
        {
        }

        public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, Elf32_Rela rela)
        {
            throw new NotImplementedException();
        }

        public override string RelocationTypeToString(uint type)
        {
            return null;
        }
    }
}