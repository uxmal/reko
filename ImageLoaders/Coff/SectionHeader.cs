using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Coff
{
    public class SectionHeader
    {
        public string s_name; // [8];  /* section name                     */
        uint s_paddr;    /* physical address, aliased s_nlib */
        uint s_vaddr;    /* virtual address                  */
        uint s_size;     /* section size                     */
        uint s_scnptr;   /* file ptr to raw data for section */
        uint s_relptr;   /* file ptr to relocation           */
        uint s_lnnoptr;  /* file ptr to line numbers         */
        ushort s_nreloc;   /* number of relocation entries     */
        ushort s_nlnno;    /* number of line number entries    */
        uint s_flags;    /* flags                            */
    }
}
