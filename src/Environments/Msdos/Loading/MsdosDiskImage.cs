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
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.Environments.Msdos.Loading;

public class MsdosDiskImage : IArchive
{
    private static TraceSwitch trace = new TraceSwitch("MsDosFloppyImage", "MS-DOS Disk Image Loader")
    {
        Level = TraceLevel.Verbose
    };

    private readonly byte[] disk;
    private readonly List<ArchiveDirectoryEntry> entries;

    public MsdosDiskImage(byte[] disk, ImageLocation location)
    {
        this.disk = disk;
        this.Location = location;
        this.entries = [];
    }

    /// <summary>
    /// The number of bytes per sector in the disk image.
    /// </summary>
    public ushort BytesPerSector { get; set; }

    /// <summary>
    /// The number of sectors per cluster in the disk image.
    /// </summary>
    public byte SectorsPerCluster { get; set; }

    /// <summary>
    /// Offset at which the FAT(s) start in the disk image.
    /// </summary>
    public int FatStartOffset { get; set; }

    /// <summary>
    /// Offset at which the data starts in the disk image.
    /// </summary>
    public int DataStartOffset { get; set; }

    /// <summary>
    /// The type of FAT used in the disk image.
    /// </summary>
    public FatType FatType { get; set; }


    public ArchiveDirectoryEntry? this[string path]
    {
        get
        {
            foreach (var entry in entries)
            {
                if (string.Equals(entry.Name, path, StringComparison.OrdinalIgnoreCase))
                    return entry;
            }
            return null;
        }
    }

    /// <inheritdoc/>

    public List<ArchiveDirectoryEntry> RootEntries => entries;

    /// <inheritdoc/>
    public ImageLocation Location { get; }

    public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context) =>
        visitor.VisitArchive(this, context);

    /// <inheritdoc/>
    public string GetRootPath(ArchiveDirectoryEntry? entry)
    {
        if (entry is null)
            return "";
        List<string> components = [];
        while (entry is not null)
        {
            components.Add(entry.Name);
            entry = entry.Parent;
        }
        components.Reverse();
        return string.Join('\\', components);
    }

    public int MapLbaToPhysicalOffset(int lba)
    {
        // Raw images (.img/.vfd) are clean byte-for-byte sector dumps ordered by LBA
        return lba * BytesPerSector;
    }

    /// <summary>
    /// Creates a <see cref="Stream"/> for reading the contents of a file stored
    /// on the disk image.
    /// </summary>
    /// <param name="entry">The directory entry for the file to open.</param>
    /// <returns>A <see cref="Stream"/> for reading the file's contents.</returns>
    public Stream OpenStream(MsdosArchiveEntry entry)
    {
        var ms = new MemoryStream();
        uint currentCluster = entry.StartingCluster;
        uint bytesLeft = (uint) entry.Length;
        int clusterBytes = this.SectorsPerCluster * BytesPerSector;
        trace.Inform("Opening stream for file {0}, starting cluster: {1:X}, length: {2} bytes", entry.Name, currentCluster, bytesLeft);
        while (currentCluster >= 0x2 && currentCluster < this.GetEofMarker(this.FatType))
        {
            int dataLba = (DataStartOffset / BytesPerSector) + (int) (currentCluster - 2) * SectorsPerCluster;
            int physicalOffset = MapLbaToPhysicalOffset(dataLba);
            trace.Verbose("Reading cluster {0:X} for file {1}, bytes left: {2}, physical offset: {3}", currentCluster, entry.Name, bytesLeft, physicalOffset);
            int toRead = (int) Math.Min(clusterBytes, bytesLeft);
            ms.Write(disk, physicalOffset, toRead);
            bytesLeft -= (uint) toRead;
            if (bytesLeft <= 0)
                break;
            currentCluster = GetNextClusterNode(currentCluster);
        }
        ms.Position = 0;
        return ms;
    }

    public uint GetEofMarker(FatType fatType)
    {
        return fatType switch
        {
            FatType.Fat12 => 0xFF8,
            FatType.Fat16 => 0xFFF8,
            FatType.Fat32 => 0x0FFFFFF8,
            _ => throw new NotSupportedException("Unknown FAT type.")
        };
    }

    public uint GetNextClusterNode(uint cluster)
    {
        switch (FatType)
        {
        case FatType.Fat12:
            int fatByteOffset = FatStartOffset + (int) (cluster * 3) / 2;
            ushort value = BitConverter.ToUInt16(disk, fatByteOffset);
            return (uint) ((cluster % 2 == 0) ? (value & 0x0FFF) : (value >> 4));
        case FatType.Fat16:
            fatByteOffset = FatStartOffset + (int) (cluster * 2);
            return BitConverter.ToUInt16(disk, fatByteOffset);
        case FatType.Fat32:
            fatByteOffset = FatStartOffset + (int) (cluster * 4);
            return BitConverter.ToUInt32(disk, fatByteOffset) & 0x0FFFFFFF;
        }
        throw new NotSupportedException("Unknown FAT type.");
    }



}

public sealed class MsdosFileEntry : MsdosArchiveEntry, ArchivedFile
{
    public MsdosFileEntry(string name, uint length, uint startingCluster, DateTime modifiedTime, MsdosDiskImage image)
        : base(name, length, startingCluster, modifiedTime, image)
    {
    }

    public byte[] GetBytes()
    {
        using Stream stm = base.Open();
        var bytes = new byte[Length];
        int bytesRead = stm.Read(bytes, 0, bytes.Length);
        if (bytesRead != bytes.Length)
            throw new IOException($"Expected to read {bytes.Length} bytes, but only read {bytesRead} bytes.");
        return bytes;
    }

    public ILoadedImage LoadImage(IServiceProvider services, Address? addrPreferred)
    {
        return new Blob(Image.Location.AppendFragment(Image.GetRootPath(this)), GetBytes());
    }
}

public abstract class MsdosArchiveEntry : ArchiveDirectoryEntry
{
    public MsdosDiskImage Image { get; }
    public string Name { get; }
    public long Length { get; }
    public uint StartingCluster { get; }
    public DateTime ModifiedTime { get; }

    public ArchiveDirectoryEntry? Parent => null;

    public MsdosArchiveEntry(string name, uint length, uint startingCluster, DateTime modifiedTime, MsdosDiskImage image)
    {
        this.Name = name;
        this.Length = length;
        this.StartingCluster = startingCluster;
        this.ModifiedTime = modifiedTime;
        this.Image = image;
    }
    public Stream Open() => Image.OpenStream(this);
}
