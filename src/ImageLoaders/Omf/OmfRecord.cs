#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.ImageLoaders.Omf
{
    public abstract class OmfRecord
    {
    }

    public class CommentRecord : OmfRecord
    {
        private string type;
        private string comment;

        public CommentRecord(string type, string comment)
        {
            this.type = type;
            this.comment = comment;
        }

        public override string ToString()
        {
            return $"{type}: {comment}";
        }
    }

    public class DataRecord : OmfRecord
    {

        public DataRecord(ushort segmentIndex, ushort dataOffset, byte[] bytes)
        {
            this.Offset = dataOffset;
            this.SegmentIndex = segmentIndex;
            this.Data = bytes;
        }

        public ushort SegmentIndex { get; }
        public ushort Offset { get; }
        public byte[] Data { get; }

        public override string ToString()
        {
            return string.Format("Seg:Offset: {0:X2}:{1:X4}", SegmentIndex, Offset);
        }
    }

    public class LnamesRecord : OmfRecord
    {
        public LnamesRecord(List<string> names)
        {
            this.Names = names;
        }

        public List<string> Names { get; }
    }

    public class ModendRecord : OmfRecord
    {
        private byte moduleAttributes;

        public ModendRecord(byte moduleAttributes)
        {
            this.moduleAttributes = moduleAttributes;
        }
    }

    public class PubdefRecord : OmfRecord
    {
        private ushort baseGroupIndex;

        public PubdefRecord(ushort baseGroupIndex, ushort baseSegmentIndex, List<PublicName> defs)
        {
            this.baseGroupIndex = baseGroupIndex;
            this.SegmentIndex = baseSegmentIndex;
            this.Names = defs;
        }

        public ushort SegmentIndex { get; }
        public List<PublicName> Names { get; }
    }

    public class SegdefRecord : OmfRecord
    {
        private int alignment;
        private int combination;

        public SegdefRecord(int alignment, int combination, ushort segLength,
            ushort nameIndex,
            ushort classIndex)
        {
            this.alignment = alignment;
            this.combination = combination;
            this.Length = segLength;
            this.NameIndex = nameIndex;
            this.ClassIndex = classIndex;
        }

        public ushort Length { get; }
        public ushort NameIndex { get; }
        public ushort ClassIndex { get; }

        public override string ToString()
        {
            string sAlignment = alignment switch
            {
                0 => "Absolute segment",
                1 => "Relocatable, byte aligned",
                2 => "Relocatable, word (2-byte, 16-bit) aligned",
                3 => "Relocatable, paragraph (16-byte) aligned",
                4 => "Relocatable, aligned on a page boundary. (The original Intel 8086 specification " +
                     "defines a page to be 256 bytes. The IBM implementation of OMF uses a 4096-byte " +
                     "or 4K page size)",
                5 => "Relocatable, aligned on a double word (4-byte) boundary",
                6 => "Not supported",
                _ => "Not defined"
            };
            Console.WriteLine("    Segment alignment: {0}", sAlignment);

            string sCombination = combination switch
            {
                0 => "Private. Do not combine with any other program segment.",
                2 => "Public. Combine by appending at an offset that meets the alignment requirement",
                4 => "Same as C=2 (public)",
                5 => "Stack. Combine as for C=2. This combine type forces byte alignment",
                6 => "Common. Combine by overlay using maximum size",
                7 => "Same as C=2 (public)",
                _ => "Reserved"
            };
            return $"{sAlignment}:{sCombination}";
        }
    }

    public class TheaderRecord : OmfRecord
    {
        private string s;

        public TheaderRecord(string s)
        {
            this.s = s;
        }
    }

    public class UnknownRecord : OmfRecord
    {
        private int code;
        private byte[] bytes;

        public UnknownRecord(int code, byte[] bytes)
        {
            this.code = code;
            this.bytes = bytes;
        }
    }
}
