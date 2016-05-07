using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.VmsExe
{
    class Header
    {
        public ushort HdrSize { get; internal set; }
        public ushort RvaTaa { get; internal set; }
        public ushort RvaSymbols { get; internal set; }
        public ushort RvaIdent { get; internal set; }
        public ushort RvaPatchData { get; internal set; }
        public ushort Spare0A { get; set; }
        public ushort IdMajor { get; internal set; }
        public ushort IdMinor { get; internal set; }

        public byte HeaderBlocks { get; internal set; }
        public byte ImageType { get; internal set; }
        public ushort Spare12 { get; set; }
        public uint ImageFlags { get; internal set; }
        public ushort IoChannels { get; internal set; }
        public ushort IoSegPages { get; internal set; }
        public ulong RequestedPrivilegeMask { get; internal set; }
        public uint GlobalSectionID { get; internal set; }
        public uint SystemVersionNumber { get; internal set; }
    }
}
