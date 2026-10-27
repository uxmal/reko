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

using Reko.Core;
using Reko.Core.Loading;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Reko.Arch.Dec.ImageLoaders;

public class LincTape : IArchive
{
    public const int BytesPerBlock = 512;
    public const int WordsPerBlock = 256;
    public const int BlocksPerTape = 512;
    public const int EmptyMarker = 0xBEF;  // 0o5757 


    private readonly int forwardOffset;
    private readonly int bytesPerBlock;
    private readonly byte[] rawImage;

    public LincTape(ImageLocation location, byte[] rawImage, int bytesPerBlock, int forwardOffset)
    {
        this.Location = location;
        this.rawImage = rawImage;
        this.bytesPerBlock = bytesPerBlock;
        this.forwardOffset = forwardOffset;
        this.RootEntries = [];
    }

    public ArchiveDirectoryEntry? this[string path] => throw new NotImplementedException();

    public List<ArchiveDirectoryEntry> RootEntries { get; internal set; }

    public ImageLocation Location { get; }


    public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
    {
        throw new NotImplementedException();
    }

    public string GetRootPath(ArchiveDirectoryEntry? entry)
    {
        throw new NotImplementedException();
    }

    public bool TryReadBlocks(int indexBlockNo, ushort[] indexBlock, int cBlocks)
    {
        int actualBlock = (indexBlockNo - this.forwardOffset) * BytesPerBlock;
        if (actualBlock + BytesPerBlock > rawImage.Length)
            return false;
        ReadOnlySpan<byte> blockSpan = rawImage.AsSpan(actualBlock, BytesPerBlock * cBlocks);
        int cTotalWords = cBlocks * WordsPerBlock;
        for (int i = 0; i < cTotalWords; ++i)
        {
            indexBlock[i] = BinaryPrimitives.ReadUInt16LittleEndian(blockSpan);
            blockSpan = blockSpan.Slice(2);
        }
        return true;
    }

}
