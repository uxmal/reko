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
using Reko.Core.Memory;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.DiskImages.Prodos
{
    // https://github.com/ksherlock/profuse/blob/5cc4d44292a9715a70075899e69bcc554a0374d6/ProDOS/Disk.cpp
    public class DiskImage : IArchive
    {
        public const int BlockSize = 512;

        internal DiskImage(ImageLocation location, List<ArchiveDirectoryEntry> rootEntries)
        {
            this.Location = location;
            this.RootEntries = rootEntries;
        }

        public List<ArchiveDirectoryEntry> RootEntries { get; }

        public ImageLocation Location { get; }

        public ArchiveDirectoryEntry? this[string path] => GetEntry(path);

        public static IArchive Load(byte[] bytes, int offset, ImageLocation location)
        {
            var loader = new DiskImageLoader(bytes, offset);
            return loader.Load(location);
        }


        private ArchiveDirectoryEntry? GetEntry(string path)
        {
            var components = path.Split(':');
            var curList = RootEntries;
            ArchiveDirectoryEntry? result = null;
            foreach (var component in components)
            {
                var e = curList.FirstOrDefault(f => f.Name == component);
                if (e is null)
                    return null;
                result = e;
            }
            return result;
        }







        public string GetRootPath(ArchiveDirectoryEntry? entry)
        {
            if (entry is null)
                return "";
            var components = new List<string>();
            while (entry is not null)
            {
                components.Add(entry.Name);
                entry = entry.Parent;
            }
            components.Reverse();
            return string.Join(':', components);
        }

        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context) =>
            visitor.VisitArchive(this, context);

        public class FileEntry : ArchivedFile
        {
            private readonly DiskImage disk;
            private FileDescriptiveEntry e;
            private byte[] bytes;

            public FileEntry(DiskImage disk, ArchivedFolder? parent, in FileDescriptiveEntry e,string name, byte[] bytes)
            {
                this.disk = disk;
                this.Parent = parent;
                this.e = e;
                this.Name = name;
                this.bytes = bytes;
            }

            public string Name { get; }

            public ArchiveDirectoryEntry? Parent { get; }

            public long Length => bytes.LongLength;

            public byte[] GetBytes() => this.bytes;

            public ILoadedImage LoadImage(IServiceProvider services, Address? addrPreferred)
            {
                var path = disk.GetRootPath(this);
                var imageLocation = disk.Location.AppendFragment(path);
                return new Blob(imageLocation, GetBytes());
            }
        }

        public class FolderEntry : ArchivedFolder
        {
            public ICollection<ArchiveDirectoryEntry> Entries => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public ArchiveDirectoryEntry? Parent => throw new NotImplementedException();
        }

        internal enum StorageType
        {
            Deleted = 0x0,
            Seedling = 0x1,
            Sapling = 0x2,
            Tree = 0x3,
            PascalArea = 0x4,
            Extended = 0x5,
            Directory = 0xD,
            SubdirHeader = 0xE,
            Subdir = 0xF,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VolumeDirectoryHeader
        {
            public byte StorageType_NameLength;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] VolumeName;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Reserved;

            public ushort CreationDate;
            public ushort CreationTyime;

            public byte Version;
            public byte MinVersion;
            public byte Access;

            public byte EntryLength;    // Typically 0x27

            public byte EntriesPerBlock;    // Typically 0xD

            public ushort EntryCount;       // doesn't include the volume directory header

            public ushort BitmapBlock;      // block # for the volume bit map (usually 6)

            public ushort TotalBlocks;      // The total number of blocks in the volume.
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileDescriptiveEntry
        {
            public byte StorageType_NameLength;
            internal StorageType StorageType => (StorageType) (StorageType_NameLength >> 4);
            internal int NameLength => (int) (StorageType_NameLength & 0xF);


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] Name;
            public string GetName() => Encoding.ASCII.GetString(Name, 0, NameLength);

            public byte FileType;

            public ushort KeyBlock;

            public ushort BlocksUsed;

            public ushort Eof;
            public byte EofHi;
            public int Length => Eof + ((int)EofHi << 16);

            public ushort CreationDate;
            public ushort CreationTyime;

            public byte Version;
            public byte MinVersion;
            
            public byte Access;

            public ushort AuxType;

            public ushort ModifiedDate;
            public ushort ModifiedTime;

            public ushort HeaderBlock;

            public string Format()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("{0,-12}", FormatStorageType(this.StorageType_NameLength >> 4));
                sb.AppendFormat(" {0:X4}", KeyBlock);
                sb.AppendFormat(" {0:X4}", BlocksUsed);
                sb.AppendFormat(" {0:X6} ", Length);
                var n = Encoding.ASCII.GetString(this.Name, 0, this.StorageType_NameLength & 0xF);
                sb.Append(n.TrimEnd('\0'));
                return sb.ToString();
            }

            private string FormatStorageType(int st)
            {
                return st switch
                {
                    0x0 => "(Deleted)",
                    0x1 => "Seedling",
                    0x2 => "Sapling",
                    0x3 => "Tree",
                    0x4 => "Pascal area",
                    0x5 => "Extended",
                    0xD => "Directory",
                    0xE => "Subdir hdr",
                    0xF => "Subdir",
                    _ => "???"
                };
            }
        }
    }
}
