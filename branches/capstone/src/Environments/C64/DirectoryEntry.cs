using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.C64
{
    public class DirectoryEntry
    {
        public byte NextDirectoryTrack;
        public byte NextDirectorySector;
        public FileType FileType;
        public byte FileTrack;
        public byte FileSector;
        public byte[] FileName;     // 16 bytes padded with A0
        public byte SideSectorTrack;    // REL files only
        public byte SideSectorSector;    // REL files only
        public byte RelRecordLength;
        public byte Byte18;
        public byte Byte19;
        public byte Byte1A;
        public byte Byte1B;
        public byte Byte1C;
        public byte Byte1D;
        public short SectorCount;
    }

    [Flags]
    public enum FileType : byte
    {
        DEL = 0x0,
        SEQ = 0x1,
        PRG = 0x2,
        USR = 0x3,
        REL = 0x4,
        FileTypeMask = 0x7,

        Locked = 0x40,
        Closed = 0x80,
    }
}
