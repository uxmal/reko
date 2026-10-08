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
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.Msdos.Loading;

public class MsdosDiskImageLoader : ImageLoader
{
    private static readonly DateTime MsdosEpoch = new(1980, 1, 1);

    public MsdosDiskImageLoader(
        IServiceProvider services,
        ImageLocation location,
        byte[] rawData) :
        base(services, location, rawData)
    {
    }

    // Note: use this method in the future if some other file format starts E9... or EB...
    // but isn't a MS-DOS disk image.
    public bool IsMatch(byte[] buffer)
    {
        // Accommodate standard floppy sizes up to common FAT32 disk image sizes
        if (buffer.Length < 368640)
            return false;


        return buffer[0] == 0xEB || buffer[0] == 0xE9;
    }

    public override ILoadedImage Load(Address? addrLoad)
    {
        byte[] disk = this.RawImage;
        var image = new MsdosDiskImage(
            disk,
            this.ImageLocation);

        // Parse BIOS Parameter Block (BPB) to determine FAT type and layout.
        image.BytesPerSector = BitConverter.ToUInt16(disk, 11);
        image.SectorsPerCluster = disk[13];
        ushort reservedSectors = BitConverter.ToUInt16(disk, 14);
        int numFats = disk[16];
        ushort maxRootEntries = BitConverter.ToUInt16(disk, 17);

        ushort totalSectors16 = BitConverter.ToUInt16(disk, 19);
        ushort sectorsPerFat16 = BitConverter.ToUInt16(disk, 22);
        uint totalSectors32 = BitConverter.ToUInt32(disk, 32);

        uint totalSectors = totalSectors16 != 0 ? totalSectors16 : totalSectors32;

        uint sectorsPerFat;
        uint rootCluster;
        FatType fatType = default;
        if (sectorsPerFat16 == 0)
        {
            sectorsPerFat = BitConverter.ToUInt32(disk, 36);
            rootCluster = BitConverter.ToUInt32(disk, 44);
            fatType = FatType.Fat32;
        }
        else
        {
            sectorsPerFat = sectorsPerFat16;
            rootCluster = 0;
        }

        // Standard Flat LBA Offset Mapping
        image.FatStartOffset = reservedSectors * image.BytesPerSector;
        int rootDirStartOffset = image.FatStartOffset + (int) (numFats * sectorsPerFat * image.BytesPerSector);

        int rootDirSectors = ((maxRootEntries * 32) + (image.BytesPerSector - 1)) / image.BytesPerSector;
        image.DataStartOffset = rootDirStartOffset + (rootDirSectors * image.BytesPerSector);

        if (fatType != FatType.Fat32)
        {
            int dataSectors = (int) totalSectors - (reservedSectors + (int) (numFats * sectorsPerFat) + rootDirSectors);
            int totalClusters = dataSectors / image.SectorsPerCluster;
            fatType = totalClusters < 4085 ? FatType.Fat12 : FatType.Fat16;
        }
        image.FatType = fatType;

        List<ArchiveDirectoryEntry> entries;
        if (fatType == FatType.Fat32)
            entries = ParseClusterDirectory(image, rootCluster, "");
        else
            entries = ParseFixedDirectory(image, rootDirStartOffset, maxRootEntries, "");

        image.RootEntries.AddRange(entries);
        return image;
    }

    private List<ArchiveDirectoryEntry> ParseClusterDirectory(
        MsdosDiskImage image,
        uint startCluster,
        string currentPath)
    {
        uint currentCluster = startCluster;
        int clusterBytes = image.SectorsPerCluster * image.BytesPerSector;
        byte[] clusterBuffer = new byte[clusterBytes];

        List<ArchiveDirectoryEntry> entries = [];

        while (currentCluster >= 0x002 && currentCluster < image.GetEofMarker(image.FatType))
        {
            int dataLba = (image.DataStartOffset / image.BytesPerSector) + (int) (currentCluster - 2) * image.SectorsPerCluster;
            int physicalOffset = image.MapLbaToPhysicalOffset(dataLba);

            if (physicalOffset + clusterBytes > RawImage.Length)
                break;
            Array.Copy(RawImage, physicalOffset, clusterBuffer, 0, clusterBytes);

            for (int i = 0; i < clusterBytes / 32; i++)
            {
                int entryPos = i * 32;
                byte firstByte = clusterBuffer[entryPos];
                if (firstByte == 0x00)
                    return entries;
                if (firstByte == 0xE5)
                    continue;

                var attributes = (MsdosFileAttributes) clusterBuffer[entryPos + 11];
                if ((attributes & MsdosFileAttributes.LongFileName) == MsdosFileAttributes.LongFileName)
                    continue;

                string name = Encoding.ASCII.GetString(clusterBuffer, entryPos, 8).TrimEnd();
                string ext = Encoding.ASCII.GetString(clusterBuffer, entryPos + 8, 3).TrimEnd();
                if (name.StartsWith('.') || attributes.HasFlag(MsdosFileAttributes.VolumeId)) continue;

                string relativeName = string.IsNullOrEmpty(ext) ? name : $"{name}.{ext}";
                string fullPath = string.IsNullOrEmpty(currentPath) ? relativeName : $"{currentPath}/{relativeName}";

                ushort clusterLow = BitConverter.ToUInt16(clusterBuffer, entryPos + 26);
                ushort clusterHigh = BitConverter.ToUInt16(clusterBuffer, entryPos + 20);
                uint targetCluster = ((uint) clusterHigh << 16) | clusterLow;
                uint size = BitConverter.ToUInt32(clusterBuffer, entryPos + 28);

                ushort lastModTime = BitConverter.ToUInt16(clusterBuffer, entryPos + 22);
                ushort lastModDate = BitConverter.ToUInt16(clusterBuffer, entryPos + 24);
                DateTime modifiedTime = ParseMsDosDateTime(lastModDate, lastModTime);

                if (attributes.HasFlag(MsdosFileAttributes.Directory))
                {
                    ParseClusterDirectory(image, targetCluster, fullPath);
                }
                else if (size >= 0)
                {
                    entries.Add(new MsdosFileEntry(fullPath, size, targetCluster, modifiedTime, image));
                }
            }

            currentCluster = image.GetNextClusterNode(currentCluster);
        }
        return entries;
    }


    private List<ArchiveDirectoryEntry> ParseFixedDirectory(
        MsdosDiskImage image,
        int byteOffset,
        int maxEntries,
        string currentPath)
    {
        var disk = RawImage;
        List<ArchiveDirectoryEntry> entries = [];
        for (int i = 0; i < maxEntries; i++)
        {
            int entryPos = byteOffset + (i * 32);
            if (entryPos + 32 > disk.Length)
                break;

            byte firstByte = disk[entryPos];
            if (firstByte == 0x00)
                break;
            if (firstByte == 0xE5)
                continue;

            var attributes = (MsdosFileAttributes) disk[entryPos + 11];
            if ((attributes & MsdosFileAttributes.LongFileName) == MsdosFileAttributes.LongFileName) continue;

            string name = Encoding.ASCII.GetString(disk, entryPos, 8).TrimEnd();
            string ext = Encoding.ASCII.GetString(disk, entryPos + 8, 3).TrimEnd();
            if (name.StartsWith('.') || attributes.HasFlag(MsdosFileAttributes.VolumeId))
                continue;

            string relativeName = string.IsNullOrEmpty(ext) ? name : $"{name}.{ext}";
            string fullPath = string.IsNullOrEmpty(currentPath) ? relativeName : $"{currentPath}/{relativeName}";

            uint targetCluster = BitConverter.ToUInt16(disk, entryPos + 26);
            uint size = BitConverter.ToUInt32(disk, entryPos + 28);

            ushort lastModTime = BitConverter.ToUInt16(disk, entryPos + 22);
            ushort lastModDate = BitConverter.ToUInt16(disk, entryPos + 24);
            DateTime modifiedTime = ParseMsDosDateTime(lastModDate, lastModTime);

            if (attributes.HasFlag(MsdosFileAttributes.Directory))
            {
                var dirEntries = ParseClusterDirectory(image, targetCluster, fullPath);
                entries.Add(new MsdosDirectory(fullPath, dirEntries));
            }
            else if (size > 0)
            {
                entries.Add(new MsdosFileEntry(fullPath, size, targetCluster, modifiedTime, image));
            }
        }
        return entries;
    }

    /// <summary>
    /// Converts the packed 16-bit MS-DOS date and time integers into a .NET DateTime structure.
    /// </summary>
    private static DateTime ParseMsDosDateTime(ushort packedDate, ushort packedTime)
    {
        if (packedDate == 0 && packedTime == 0)
            return MsdosEpoch;

        // Extract Date fields
        int day = packedDate & 0x1F;
        int month = (packedDate >> 5) & 0x0F;
        int year = ((packedDate >> 9) & 0x7F) + 1980;

        // Extract Time fields
        int second = (packedTime & 0x1F) * 2;
        int minute = (packedTime >> 5) & 0x3F;
        int hour = (packedTime >> 11) & 0x1F;

        // Constrain field anomalies common to raw disk corruptions to safely construct the object
        day = Math.Clamp(day, 1, 31);
        month = Math.Clamp(month, 1, 12);
        hour = Math.Clamp(hour, 0, 23);
        minute = Math.Clamp(minute, 0, 59);
        second = Math.Clamp(second, 0, 59);

        try
        {
            return new DateTime(year, month, day, hour, minute, second);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Fallback scenario for logically un-parseable values
            return MsdosEpoch;
        }
    }



    /// <summary>
    /// Represents the standard MS-DOS file attribute flags located at offset 11 (0x0B) 
    /// within a 32-byte directory entry structure.
    /// </summary>
    [Flags]
    public enum MsdosFileAttributes : byte
    {
        /// <summary>
        /// A normal file with no special attributes set.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// The file is read-only. Standard applications cannot modify or delete it.
        /// </summary>
        ReadOnly = 0x01,

        /// <summary>
        /// The file is hidden. Standard directory listings (like a raw 'DIR' command) skip it.
        /// </summary>
        Hidden = 0x02,

        /// <summary>
        /// The file is an operating system file (e.g., IO.SYS, MSDOS.SYS).
        /// </summary>
        System = 0x04,

        /// <summary>
        /// The entry represents the Volume Label for the storage media. 
        /// Should only appear once in the root directory.
        /// </summary>
        VolumeId = 0x08,

        /// <summary>
        /// The entry describes a hierarchical sub-directory cluster pointer rather than a file.
        /// </summary>
        Directory = 0x10,

        /// <summary>
        /// The file has been modified since the last backup or archive operation.
        /// </summary>
        Archive = 0x20,

        /// <summary>
        /// Special mask combination (ReadOnly | Hidden | System | VolumeId) 
        /// indicating this entry contains a VFAT Long File Name (LFN) fragment.
        /// </summary>
        LongFileName = ReadOnly | Hidden | System | VolumeId
    }
}

public sealed class MsdosDirectory : ArchiveDirectoryEntry
{
    public MsdosDirectory(string name, List<ArchiveDirectoryEntry> entries, ArchiveDirectoryEntry? parent = null)
    {
        this.Name = name;
        this.Parent = parent;
        this.Entries = entries;
    }
    public string Name { get; }
    public ArchiveDirectoryEntry? Parent { get; }
    public List<ArchiveDirectoryEntry> Entries { get; }
}
