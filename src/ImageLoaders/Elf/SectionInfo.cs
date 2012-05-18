using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    
    public class SectionInfo
    {
        internal string pSectionName;
        //internal ElfLoader.SectionType uType;
        internal bool IsCode;
        internal bool IsBss;
        internal uint uNativeAddr;
        internal uint uHostAddr;
        internal uint uSectionSize;
        internal uint uSectionEntrySize;
        internal bool bData;
        public bool IsReadOnly;

        [Conditional("DEBUG")]
        public void Dump()
        {
            Debug.Print("pSectionName: {0}", pSectionName);
            //Debug.Print("uType: {0}", uType);
            Debug.Print("IsCode: {0}", IsCode);
            Debug.Print("IsBss: {0}", IsBss);
            Debug.Print("uNativeAddr: {0:X}", uNativeAddr);
            Debug.Print("uHostAddr: {0:X}", uHostAddr);
            Debug.Print("uSectionSize: {0:X}", uSectionSize);
            Debug.Print("uSectionEntrySize: {0:X}", uSectionEntrySize);
            Debug.Print("bData: {0}", bData);
            Debug.Print("IsReadOnly: {0}", IsReadOnly);
        }
    }
}
