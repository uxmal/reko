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
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Reko.ImageLoaders.DiskImages.Prodos.DiskImage;

namespace Reko.ImageLoaders.DiskImages.Prodos
{
    public class DiskImageLoader
    {
        private readonly byte[] bytes;
        private readonly int offset;

        public DiskImageLoader(byte[] image, int offset)
        {
            this.bytes = image;
            this.offset = offset;
        }

        internal DiskImage Load(ImageLocation location)
        {
            var rdr = SeekToBlock(2);        // Skip the boot loader

            // We're now positioned at the volume directory.
            var dir = LoadVolumeDirectory(rdr, offset);
            if (dir is null)
                throw new BadImageFormatException("Cannot load image as a Prodos volume.");
            var entries = new List<ArchiveDirectoryEntry>();
            var disk = new DiskImage(location, entries);
            
            ArchivedFolder? parent = null;
            for (int i = 0; i < dir.Value.EntryCount; ++i)
            {
                var e = rdr.ReadStruct<FileDescriptiveEntry>();
                Debug.WriteLine($"Entry: {e.Format()}");
                var entry = CreateEntry(e, disk, parent);
                if (entry is null)
                    continue;
                entries.Add(entry);
            }
            return disk; 
        }

        private ArchiveDirectoryEntry? CreateEntry(in FileDescriptiveEntry rawEntry, DiskImage disk, ArchivedFolder? parent)
        {
            var key = rawEntry.KeyBlock;
            var eof = rawEntry.Eof;
            var mem = new MemoryStream();
            return rawEntry.StorageType switch
            {
                StorageType.Deleted => null,
                StorageType.Seedling => CreateEntryFromBlock(rawEntry, disk, parent, mem),
                StorageType.Sapling => CreateEntryFromSapling(rawEntry, disk, parent, mem),
                _ => null,
            };
        }

        private ArchivedFile CreateEntryFromBlock(in FileDescriptiveEntry e, DiskImage disk, ArchivedFolder? parent, MemoryStream mem)
        {
            var rdr = SeekToBlock(e.KeyBlock);
            var bytes = rdr.Read(e.Eof);
            mem.Write(bytes);
            return new DiskImage.FileEntry(disk, parent, e, e.GetName(), mem.ToArray());
        }

        private ArchivedFile CreateEntryFromSapling(in FileDescriptiveEntry e, DiskImage disk, ArchivedFolder? parent,  MemoryStream mem)
        {
            var indexBlock = new byte[DiskImage.BlockSize];
            ReadBlock(e.KeyBlock, indexBlock);
            int cbLeft = e.Eof;
            int iIndex = 0;
            byte[] blockBuffer = new byte[DiskImage.BlockSize];
            while (cbLeft > 0)
            {
                var iBlock = indexBlock[iIndex] + indexBlock[iIndex + 256] * 256;
                if (iBlock == 0)
                {
                    Array.Clear(blockBuffer);
                }
                else
                {
                    ReadBlock(iBlock, blockBuffer);
                }
                var bytesToCopy = Math.Min(cbLeft, DiskImage.BlockSize);
                mem.Write(blockBuffer, 0, Math.Min(DiskImage.BlockSize, bytesToCopy));
                cbLeft -= bytesToCopy;
            }
            return new DiskImage.FileEntry(disk, parent, e, e.GetName(), mem.ToArray());
        }

        private void ReadBlock(int iBlock, byte[] blockBuffer)
        {
            Array.Copy(bytes, this.offset + iBlock * DiskImage.BlockSize, blockBuffer, 0, DiskImage.BlockSize);
        }


        private DiskImage.VolumeDirectoryHeader? LoadVolumeDirectory(ByteImageReader rdr, int offset)
        {
            if (!rdr.TryReadLeUInt16(out var blockPrev))
                return null;
            if (!rdr.TryReadLeUInt16(out var blockNext))
                return null;
            return rdr.ReadStruct<DiskImage.VolumeDirectoryHeader>();
        }


        private ByteImageReader SeekToBlock(int blockNo)
        {
            var rdr = new ByteImageReader(bytes, offset + blockNo * DiskImage.BlockSize);
            return rdr;
        }
    }
}
