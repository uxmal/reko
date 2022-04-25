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
    public struct aux_id
    {
        public ushort bits;
        //unsigned int mandatory : 1;
        //unsigned int copy : 1;
        //unsigned int append : 1;
        //unsigned int ignore : 1;
        //unsigned int reserved : 12;
        public ushort type;
        public uint length;
    }

    public static class aux_id_type
    {
        public const ushort exec_aux_header = 0x0004;
    }
}
