using Reko.Core;
using Reko.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.HpSom
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    [Endian(Endianness.BigEndian)]
    public struct SOM_Exec_aux_hdr
    {
        //struct aux_id som_auxhdr;/* som auxiliary header */
        public uint exec_tsize; /* text size in bytes */
        public uint exec_tmem; /* offset of text in memory */
        public uint exec_tfile; /* location of text in file */
        public uint exec_dsize; /* initialized data */
        public uint exec_dmem; /* offset of data in memory */
        public uint exec_dfile; /* location of data in file */
        public uint exec_bsize; /* uninitialized data (bss) */
        public uint exec_entry; /* offset of entrypoint */
        public uint exec_flags; /* loader flags */
        public uint exec_bfill; /* bss initialization value */
    }
}
