using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class SectionInfo
    {
        internal string pSectionName;
        internal uint uType;
        internal bool bCode;
        internal bool bBss;
        internal uint uNativeAddr;
        internal uint uHostAddr;
        internal uint uSectionSize;
        internal uint uSectionEntrySize;
        internal bool bData;
        public bool IsReadOnly;
    }
}
