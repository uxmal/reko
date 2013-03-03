using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class Elf32_Rel
    {
        uint r_offset;
        int r_info;
    }

    public class Elf32_Rela
    {
        uint r_offset;
        uint r_info;
        int r_addend;
    }
}
