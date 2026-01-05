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

using Reko.Core.Loading;
using Reko.ImageLoaders.Archives.Ar;
using System;
using System.Collections.Generic;

namespace Reko.ImageLoaders.Archives.Lbr
{
    public class LbrArchive : IArchive
    {
        private readonly byte[] rawBytes;
        private readonly List<ArchiveDirectoryEntry> directoryEntries;

        public LbrArchive(ImageLocation location, byte[] rawBytes, List<ArchiveDirectoryEntry> directoryEntries)
        {
            this.Location = location;
            this.rawBytes = rawBytes;
            this.directoryEntries = directoryEntries;
        }

        public ArchiveDirectoryEntry? this[string path]
        {
            get
            {
                foreach (DirectoryEntry entry in directoryEntries)
                {
                    if (entry.DottedFilename == path)
                        return entry;
                }
                return null;
            }
        }

        public List<ArchiveDirectoryEntry> RootEntries => directoryEntries;

        public ImageLocation Location { get; }

        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitArchive(this, context);
        }

        public string GetRootPath(ArchiveDirectoryEntry? entry)
        {
            if (entry is null)
                return "";
            if (entry is not DirectoryEntry direntry)
                throw new ArgumentException(
                    $"Expected a {nameof(DirectoryEntry)}.",
                    nameof(entry));
            return direntry.DottedFilename;
        }

        public byte[] GetBytes(DirectoryEntry entry)
        {
            var result = new byte[((ArchivedFile) entry).Length];
            Array.Copy(rawBytes, entry.Index * DirectoryEntry.SectorSize, result, 0, result.Length);
            return result;
        }
    }
}
