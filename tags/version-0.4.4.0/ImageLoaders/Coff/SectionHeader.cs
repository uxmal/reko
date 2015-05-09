using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Coff
{
    public class SectionHeader
    {
        public string s_name; // [8];  /* section name                     */
        public uint s_paddr;    /* physical address, aliased s_nlib */
        public uint s_vaddr;    /* virtual address                  */
        public uint s_size;     /* section size                     */
        public uint s_scnptr;   /* file ptr to raw data for section */
        public uint s_relptr;   /* file ptr to relocation           */
        public uint s_lnnoptr;  /* file ptr to line numbers         */
        public ushort s_nreloc;   /* number of relocation entries     */
        public ushort s_nlnno;    /* number of line number entries    */
        public uint s_flags;    /* flags                            */
    }
}
