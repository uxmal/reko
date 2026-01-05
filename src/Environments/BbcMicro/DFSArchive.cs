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
using System.Collections.Generic;

namespace Reko.Environments.BbcMicro
{
    public class DFSArchive : IArchive
    {
        private List<ArchiveDirectoryEntry> files;

        public DFSArchive(ArchiveDirectoryEntry[] dir, ImageLocation location)
        {
            this.files = new(dir);
            this.Location = location;
        }

        public ArchiveDirectoryEntry? this[string path] => throw new System.NotImplementedException();

        public List<ArchiveDirectoryEntry> RootEntries => files;

        public ImageLocation Location { get; }

        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitArchive(this, context);
        }

        /// <inheritdoc/>
        public string GetRootPath(ArchiveDirectoryEntry? entry)
        {
            return entry?.Name ?? "";
        }
    }
}