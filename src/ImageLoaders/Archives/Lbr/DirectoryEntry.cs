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

using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.ImageLoaders.Archives.Lbr;

public class DirectoryEntry : ArchivedFile
{
    public const int SectorSize = 128;
    private LbrArchive archive;

    private DirectoryEntry(LbrArchive archive, ImageLocation parentLocation)
    {
        this.archive = archive;
        ParentLocation = parentLocation;
    }

    public static bool TryRead(
        LbrArchive archive,
        ImageLocation location,
        ByteImageReader rdr,
        [MaybeNullWhen(false)] out DirectoryEntry entry)
    {
        entry = default;
        if (!rdr.TryReadByte(out byte status))
            return false;
        var aName = rdr.ReadBytes(8);
        if (aName is null || aName.Length != 8)
            return false;
        var aExt = rdr.ReadBytes(3);
        if (aExt is null || aExt.Length != 3)
            return false;
        if (!rdr.TryReadLeUInt16(out ushort index))
            return false;
        if (!rdr.TryReadLeUInt16(out ushort length))
            return false;
        if (!rdr.TryReadLeUInt16(out ushort crc))
            return false;
        if (!rdr.TryReadLeUInt16(out ushort creationDate))
            return false;
        if (!rdr.TryReadLeUInt16(out ushort modifiedDate))
            return false;
        if (!rdr.TryReadLeUInt16(out ushort creationTime))
            return false;
        if (!rdr.TryReadLeUInt16(out ushort modifiedTime))
            return false;
        if (!rdr.TryReadByte(out byte padCount))
            return false;
        rdr.Seek(5);    // Skip to next record.
        entry = new DirectoryEntry(archive, location)
        {
            Status = (Status) status,
            ParentLocation = location,
            Name = Encoding.UTF8.GetString(aName).TrimEnd(),
            Extension = Encoding.UTF8.GetString(aExt).TrimEnd(),
            Index = index,
            Length = length,
            Crc = crc,
            CreationDate = creationDate,
            LastModifiedDate = modifiedDate,
            CreationTime = creationTime,
            LastModifiedTime = modifiedTime,
            PadCount = padCount,
        };
        return true;
    }


    public Status Status { get; set; }

    public string? Name { get; set; }

    string ArchiveDirectoryEntry.Name => this.DottedFilename;

    ArchiveDirectoryEntry? ArchiveDirectoryEntry.Parent => null;

    public string? Extension { get; set; }

    /// <summary>
    /// Index of the first 128-byte sector of this member in the file.
    /// </summary>
    public ushort Index { get; set; }

    /// <summary>
    /// Size in 128-byte sectors.
    /// </summary>
    public ushort Length { get; set; }

    /// <summary>
    /// CCITT CRC-16.
    /// </summary>
    public ushort Crc { get; set; }

    /// <summary>
    /// # of days since 1977-jan-31
    /// </summary>
    public ushort CreationDate { get; set; }
    public ushort LastModifiedDate { get; set; }
    public ushort CreationTime { get; set; }
    public ushort LastModifiedTime { get; set; }

    public byte PadCount { get; set; }
    public string DottedFilename
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Extension))
                return Name ?? "";
            return $"{Name}.{Extension}";
        }
    }

    long ArchivedFile.Length => this.Length * SectorSize - this.PadCount;

    public ImageLocation ParentLocation { get; private set; }

    public byte[] GetBytes()
    {
        return archive.GetBytes(this);
    }

    public ILoadedImage LoadImage(IServiceProvider services, Address? addrPreferred)
    {
        var imageLocation = archive.Location.AppendFragment(DottedFilename);
        return new Blob(imageLocation, GetBytes());
    }
}

public enum Status : byte
{
    Active = 0x00,
    Deleted = 0xFE,
    Unused = 0xFF,
    // Any other value is treated as a deleted entry.

}
