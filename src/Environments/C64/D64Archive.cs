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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.C64
{
    public class D64Archive : IArchive
    {
        private IServiceProvider services;

        public D64Archive(IServiceProvider services, ImageLocation archiveUri, List<ArchiveDirectoryEntry> entries)
        {
            this.services = services;
            this.Location = archiveUri;
            this.RootEntries = entries;
        }

        /// <summary>
        /// Retrieve the file whose name is <paramref name="path"/>.
        /// </summary>
        /// <remarks>
        /// C64 disks have no tree structure, so the path has to be the actual
        /// file name.</remarks>
        /// <param name="path">Name of the file.</param>
        /// <returns></returns>
        public ArchiveDirectoryEntry? this[string path]
        {
            get => RootEntries.Where(e => e.Name == path).FirstOrDefault();
        }

        public List<ArchiveDirectoryEntry> RootEntries { get; }

        public ImageLocation Location { get; }

        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
            => visitor.VisitArchive(this, context);

        public string GetRootPath(ArchiveDirectoryEntry? entry)
        {
            if (entry is null)
                return "";
            if (entry is not D64Loader.D64FileEntry file)
                throw new ArgumentException(string.Format(
                    "Invalid entry type {0} for {1}.",
                    entry.GetType().FullName,
                    nameof(D64Archive)));
            return file.Name;
        }
    }
}
