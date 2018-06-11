using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Elf
{
    public class ElfRelocation
    {
        public ulong Offset;
        public ulong Info;
        public long Addend;

        public int SymbolIndex => (int)(Info >> 32);

        public ElfSymbol Symbol { get; internal set; }
    }
}
