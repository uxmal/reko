#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Diagnostics;
using Reko.Core.Memory;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Omf
{
    /// <summary>
    /// Reads in OMF records from an <see cref="EndianImageReader"/>.
    /// </summary>
    public class OmfParser
    {
        private static readonly TraceSwitch trace = new(nameof(OmfParser), "")
        {
            Level = TraceLevel.Verbose
        };

        private readonly EndianImageReader rdr;

        public OmfParser(EndianImageReader rdr)
        {
            this.rdr = rdr;
        }

        public bool TryReadRecord([MaybeNullWhen(false)] out OmfRecord record)
        {
            record = null;
            if (!this.rdr.TryReadByte(out byte rt))
                return false;
            if (!this.rdr.TryReadLeUInt16(out ushort length))
                return false;
            var recordData = this.rdr.ReadBytes(length);
            var rdr = new LeImageReader(recordData, 0, length - 1);
            bool success;
            switch (rt)
            {
            case 0x80:
                trace.Verbose("{0:X2}: THEADR length 0x{1:X4}", (byte) rt, length);
                success =  ParseTheadr(rdr, out record);
                break;
            case 0x88:
                trace.Verbose("{0:X2}: COMENT length 0x{1:X4}", (byte) rt, length);
                success = ParseComent(rdr, length, out record);
                break;
            case 0x8A:
                trace.Verbose("{0:X2}: MODEND length 0x{1:X4}", (byte) rt, length);
                success = ParseModend(rdr, out record);
                break;
            case 0x90:
                trace.Verbose("{0:X2}: PUBDEF length 0x{1:X4}", (byte) rt, length);
                success = ParsePubdef(rdr, out record);
                break;
            case 0x96:
                trace.Verbose("{0:X2}: LNAMES length 0x{1:X4}", (byte) rt, length);
                success = ParseLnames(rdr, out record);
                break;
            case 0x98:
                trace.Verbose("{0:X2}: SEGDEF length 0x{1:X4}", (byte) rt, length);
                success = ParseSegdef(rdr, out record);
                break;
            case 0xA0:
                trace.Verbose("{0:X2}: LEDATA length 0x{1:X4}", (byte) rt, length);
                success = ParseLedata(rdr, out record);
                break;
            case 0xA2:
                trace.Verbose("{0:X2}: LIDATA length 0x{1:X4}", (byte) rt, length);
                success = ParseLidata(rdr, out record);
                break;
            default:
                trace.Verbose("{0:X2}: length 0x{1:X4}", (byte) rt, length);
                success = ParseUnknownRecord(rdr, rt, out record);
                trace.Verbose("Unknown record type {0:X2}", rt);
                trace.Verbose("{0}", string.Join(" ", rdr.Bytes.Select(b => b.ToString("X2"))));
                break;
            }
            if (success && record is not null)
            {
                trace.Verbose("    {0}", record.ToString() ?? "?");
            }
            return success;
        }

        private static bool ParseLedata(EndianImageReader rdr, [MaybeNullWhen(false)] out OmfRecord result)
        {
            result = null;
            if (!TryReadIndex(rdr, out var segmentIndex))
                return false;
            if (!rdr.TryReadLeUInt16(out ushort dataOffset))
                return false;
            var bytes = rdr.ReadToEnd();
            result = new DataRecord(segmentIndex, dataOffset, bytes);
            return true;
        }

        private static bool ParseLidata(EndianImageReader rdr, [MaybeNullWhen(false)] out OmfRecord result)
        {
            result = null;
            if (!TryReadIndex(rdr, out var segmentIndex))
                return false;
            if (!rdr.TryReadLeUInt16(out ushort dataOffset))
                return false;
            var bytes = TryReadDataBlock(rdr);
            if (bytes is null)
                return false;
            result = new DataRecord(segmentIndex, dataOffset, bytes);
            return true;
        }

        private static byte[]? TryReadDataBlock(EndianImageReader rdr)
        {
            if (!rdr.TryReadLeUInt16(out var repeatCount))
                return null;
            if (!rdr.TryReadLeUInt16(out var blockCount))
                return null;
                
            var blockData = new List<byte>();
            if (blockCount == 0)
            {
                if (!rdr.TryReadByte(out var cBytes))
                    return null;
                var bytes = rdr.ReadBytes(cBytes);
                if (bytes.Length != cBytes)
                    return null;
                blockData.AddRange(bytes);
            }
            else
            {
                for (int i = 0; i < blockCount; ++i)
                {
                    var block = TryReadDataBlock(rdr);
                    if (block is null)
                        return null;
                    blockData.AddRange(block);
                }
            }
            
            var list = new List<byte>();
            for (int i = 0; i < repeatCount; ++i)
            {
                list.AddRange(blockData);
            }
            return list.ToArray();
        }

        private static bool ParseModend(LeImageReader rdr, [MaybeNullWhen(false)] out OmfRecord result)
        {
            result = null;
            if (!rdr.TryReadByte(out byte moduleAttributes))
                return false;
            result = new ModendRecord(moduleAttributes);
            return true;
        }

        private static bool ParsePubdef(EndianImageReader rdr, [MaybeNullWhen(false)] out OmfRecord result)
        {
            result = null;
            if (!TryReadIndex(rdr, out var baseGroupIndex))
                return false;
            if (!TryReadIndex(rdr, out var baseSegmentIndex))
                return false;
            if (baseSegmentIndex == 0)
            {
                if (!rdr.TryReadLeUInt16(out ushort baseFrame))
                    return false;
                // Base frame not used according to spec.
            }
            var defs = new List<PublicName>();
            while (TryReadName(rdr, out var s))
            {
                if (!rdr.TryReadLeUInt16(out ushort offset))
                    return false;
                if (!TryReadIndex(rdr, out var typeIndex))
                    return false;
                var name = new PublicName(s, offset, typeIndex);
                defs.Add(name);
            }
            result = new PubdefRecord(baseGroupIndex, baseSegmentIndex, defs);
            return true;
        }

        private static bool ParseSegdef(EndianImageReader rdr, [MaybeNullWhen(false)] out OmfRecord result)
        {
            result = null;
            if (!rdr.TryReadByte(out var segAttributes))
                return false;
            var alignment = (segAttributes >> 5) & 7;

            var combination = (segAttributes >> 2) & 7;
            if (alignment == 0)
            {
                if (!rdr.TryReadLeUInt16(out var frameNumber))
                    return false;
                if (!rdr.TryReadByte(out var offset))
                    return false;
                trace.Verbose("    Frame number {0:X4}, offset: {1:X2}", frameNumber, offset);
            }

            if (!rdr.TryReadLeUInt16(out ushort segLength))
                return false;
            if (!TryReadIndex(rdr, out var nameIndex))
                return false;
            if (!TryReadIndex(rdr, out var classIndex))
                return false;
            result = new SegdefRecord(alignment, combination, segLength, nameIndex, classIndex);
            return true;
        }

        private static bool ParseLnames(EndianImageReader rdr, [MaybeNullWhen(false)] out OmfRecord result)
        {
            var names = new List<string>();
            while (TryReadName(rdr, out string? str))
            {
                names.Add(str);
            }
            result = new LnamesRecord(names);
            return true;
        }

        private static bool ParseTheadr(EndianImageReader rdr, [MaybeNullWhen(false)] out OmfRecord result)
        {
            result = null;
            if (!TryReadName(rdr, out var s))
                return false;
            result = new TheaderRecord(s);
            return true;
        }

        private static bool ParseComent(EndianImageReader rdr, int recLength, [MaybeNullWhen(false)] out OmfRecord result)
        {
            result = null;
            if (!rdr.TryReadByte(out byte type))
                return false;
            if (!rdr.TryReadByte(out byte comClass))
                return false;
            trace.Verbose("    Type: {0:X2}, Class: {1:X2}", type, comClass);
            switch (comClass)
            {
            case 0:
                var bytes = rdr.ReadBytes(recLength - 3);
                result = new CommentRecord("Translator", Encoding.ASCII.GetString(bytes));
                return true;
            case 0xE9: // Dependency file
                rdr.Offset += 4;
                if (!rdr.TryReadByte(out byte length))
                    break;
                bytes = rdr.ReadBytes(length);
                var str = Encoding.ASCII.GetString(bytes);
                result = new CommentRecord("Dependency", str);
                return true;
            }
            return ParseUnknownRecord(rdr, 0, out result);
        }


        private static bool ParseUnknownRecord(EndianImageReader rdr, int code, [MaybeNullWhen(false)] out OmfRecord result)
        {
            rdr.Offset = 0;
            var bytes = rdr.ReadToEnd();
            result = new UnknownRecord(code, bytes);
            return true;
        }


        private static int ReadRecordLength(Stream stm)
        {
            int lo = stm.ReadByte();
            if (lo < 0)
                return -1;
            int hi = stm.ReadByte();
            if (hi < 0)
                return -1;
            return (hi << 8) | lo;
        }

        private static bool TryReadIndex(EndianImageReader rdr, out ushort index)
        {
            // According to the OMF spec, the high byte is indicated with a high bit
            // set.
            if (!rdr.TryReadByte(out byte hi))
            {
                index = 0;
                return false;
            }
            if ((hi & 0x80) == 0)
            {
                index = hi;
                return true;
            }
            if (!rdr.TryReadByte(out byte lo))
            {
                index = 0;
                return false;
            }
            index = (ushort) (((hi & 0x7F) << 8) | lo);
            return true;
        }

        private static bool TryReadName(EndianImageReader rdr, [MaybeNullWhen(false)] out string s)
        {
            if (!rdr.TryReadByte(out byte length))
            {
                s = null;
                return false;
            }
            var bytes = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                if (!rdr.TryReadByte(out byte b))
                    break;
                bytes[i] = b;
            }
            s = Encoding.ASCII.GetString(bytes);
            return true;
        }
    }
}
