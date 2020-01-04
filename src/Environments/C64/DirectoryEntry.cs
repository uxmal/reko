#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Models directory entries on D64 disks.
    /// </summary>
    public class DirectoryEntry
    {
        public byte NextDirectoryTrack;
        public byte NextDirectorySector;
        public FileType FileType;
        public byte FileTrack;
        public byte FileSector;
        public byte[] FileName;         // 16 bytes padded with A0
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
