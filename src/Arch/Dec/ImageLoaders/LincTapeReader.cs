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

using Reko.Arch.Dec.Linc;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Dec.ImageLoaders;

/// <summary>
/// Reads files containing LINCtape images. This class expects the
/// 12-bit words of the contents to be stored as consecutive bytes
/// in little-endian order. The index (directory) of the tape is expected
/// to be in two well known locations.
/// </summary>
public class LincTapeReader : ImageLoader
{
    private const int WordsPerIndexEntry = 8;
    private const int EntriesInIndex = 64;

    public LincTapeReader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) 
        : base(services, imageLocation, imgRaw)
    {
    }

    public override ILoadedImage Load(Address? addrLoad)
    {
        ReadMetadata(RawImage, out int blockSize, out int forwardOffset, out int backwardOffset);
        var tape = new LincTape(base.ImageLocation, RawImage, blockSize, forwardOffset);
        var index = ReadIndex(tape);
        tape.RootEntries = index;
        return tape;
    }

    private void ReadMetadata(byte[] rawImage, out int blockSize, out int forwardOffset, out int backwardOffset)
    {
        if (rawImage.Length < 6)
            throw new BadImageFormatException("LINC tape image is too small to contain metadata.");
        if (rawImage.Length == LincTape.BlocksPerTape * LincTape.WordsPerBlock * 2)
        {
            // This is a "plain" LINC tape image, with no metadata.  Use the default values.
            blockSize = LincTape.WordsPerBlock;
            forwardOffset = 0;
            backwardOffset = 0;
            return;
        }
        if (rawImage.Length % LincTape.BytesPerBlock == 6)
        {
            int i = (rawImage.Length / LincTape.BytesPerBlock) * LincTape.BytesPerBlock;
            blockSize = ByteMemoryArea.ReadLeUInt16(RawImage, i);
            forwardOffset = ByteMemoryArea.ReadLeInt16(RawImage, i + 2);
            backwardOffset = ByteMemoryArea.ReadLeInt16(RawImage, i + 4);
            return;
        }
        throw new BadImageFormatException("LINC tape image has an invalid size.");
    }

    private static List<ArchiveDirectoryEntry> ReadIndex(LincTape tape)
    {
        int indexBlockNo = 0x116; // 0o426;
        var indexBlocks = new ushort[LincTape.WordsPerBlock * 2];
        if (!tape.TryReadBlocks(indexBlocks, indexBlockNo, 2) ||
            !IsEmptyIndexEntry(indexBlocks, 0, LincTape.EmptyMarker))
        {
            indexBlockNo = 0x0D6; // 0o326;
            if (!tape.TryReadBlocks(indexBlocks, indexBlockNo, 2) ||
                !IsEmptyIndexEntry(indexBlocks, 0, LincTape.EmptyMarker))
            {
                throw new BadImageFormatException("Couldn't find the LINC table index.");
            }
        }
        var tapeIndex = BuildTapeIndex(tape, indexBlocks);
        return tapeIndex;
    }

    private static bool IsEmptyIndexEntry(ushort[] indexBlock, int entryIndex)
    {
        return IsEmptyIndexEntry(indexBlock, entryIndex, LincTape.EmptyMarker) ||
               IsEmptyIndexEntry(indexBlock, entryIndex, 0);
    }

    private static bool IsEmptyIndexEntry(ushort[] indexBlock, int entryIndex, ushort emptyMarker)
    {
        int wordIndex = entryIndex * WordsPerIndexEntry;
        for (int i = 0; i < WordsPerIndexEntry; ++i)
        {
            if (indexBlock[wordIndex + i] != emptyMarker)
                return false;
        }
        return true;
    }

    private static List<ArchiveDirectoryEntry> BuildTapeIndex(LincTape tape, ushort[] indexBlock)
    {
        List<ArchiveDirectoryEntry> result = [];
        // The first entry is always unused, so we skip it.
        for (int i = 1; i < EntriesInIndex; ++i)
        {
            int iWord = i * WordsPerIndexEntry;
            if (indexBlock[iWord] == LincTape.EmptyMarker)
                continue;
            var name = LincEncoding.Instance.ConvertToUnicode(indexBlock, iWord, 4)
                .Replace(LincEncoding.Ch77, " ")
                .TrimEnd();
            var startBlock = indexBlock[iWord + 4];
            var cBlocks = indexBlock[iWord + 5];
            var execAddress = indexBlock[iWord + 6];
            var typeId = indexBlock[iWord + 7];
            var entry = new LincIndexEntry(tape, name, startBlock, cBlocks, execAddress, typeId);
            Debug.WriteLine(entry.ToString());
            result.Add(entry);
        }
        return result;
    }


    private static string oct(int n, int width)
    {
        return Convert.ToString(n, 8).PadLeft(width, '0');
    }
}
