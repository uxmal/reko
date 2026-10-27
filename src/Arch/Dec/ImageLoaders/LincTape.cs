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
using System.Linq;

namespace Reko.Arch.Dec.ImageLoaders;

/// <summary>
/// Represents a LINC tape archive backed by a raw tape image and exposed through
/// the <see cref="IArchive"/> interface.
/// </summary>
/// <remarks>
/// The raw bytes are expected to store the 12-bit words as 16-bit little-endian values,
/// with the upper 4 bits of each word set to zero.
/// </remarks>
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

    public ArchiveDirectoryEntry? this[string path] => RootEntries.FirstOrDefault(e => string.Compare(
        e.Name,
        path, 
        StringComparison.OrdinalIgnoreCase) == 0);

    /// <inheritdoc/>
    public List<ArchiveDirectoryEntry> RootEntries { get; internal set; }

    /// <inheritdoc/>
    public ImageLocation Location { get; }

    /// <inheritdoc/>
    public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
    {
        return visitor.VisitArchive(this, context);
    }

    /// <inheritdoc/>
    public string GetRootPath(ArchiveDirectoryEntry? entry)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read <paramref name="blockCount"/> blocks from the tape, starting at 
    /// <paramref name="blockNumber"/>. The words from the read blocks are
    /// stored in <paramref name="blocks"/>.
    /// </summary>
    /// <param name="blocks">The buffer into which the read words are stored.</param>
    /// <param name="blockNumber">Block from which to start.</param>
    /// <param name="blockCount">The number of blocks to read.</param>
    /// <returns>
    /// The method returns true if the read was successful, false otherwise.
    /// </returns>
    public bool TryReadBlocks(ushort[] blocks, int blockNumber, int blockCount)
    {
        int actualBlock = (blockNumber - this.forwardOffset) * BytesPerBlock;
        if (actualBlock + BytesPerBlock > rawImage.Length)
            return false;
        ReadOnlySpan<byte> blockSpan = rawImage.AsSpan(actualBlock, BytesPerBlock * blockCount);
        int cTotalWords = blockCount * WordsPerBlock;
        for (int i = 0; i < cTotalWords; ++i)
        {
            blocks[i] = BinaryPrimitives.ReadUInt16LittleEndian(blockSpan);
            blockSpan = blockSpan.Slice(2);
        }
        return true;
    }

}
