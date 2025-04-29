#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System;
using System.Collections.Generic;

namespace Reko.Core.Loading
{
    /// <summary>
    /// Models a file archive or container; that is, a file whose contents are other files.
    /// </summary>
    /// <remarks>
    /// Examples of archives are floppy disk images, optical storage .iso files, AR and TAR files,
    /// and ZIP files.
    /// </remarks>
    public interface IArchive : ILoadedImage
    {
        /// <summary>
        /// Retrieves the <see cref="ArchiveDirectoryEntry"/> located at the
        /// <paramref name="path"/> from the root of the archive.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>A <see cref="ArchiveDirectoryEntry"/> if the path leads to a 
        /// valid entry, or null if not.
        /// </returns>
        ArchiveDirectoryEntry? this[string path] { get; }

        /// <summary>
        /// Get a list of the <see cref="ArchiveDirectoryEntry"/>s at the root
        /// level of the archive.
        /// </summary>
        List<ArchiveDirectoryEntry> RootEntries { get; }

        /// <summary>
        /// Given an <see cref="ArchiveDirectoryEntry"/> returns the path
        /// from the root of the archive to that entry.
        /// </summary>
        /// <param name="entry">The entry we wish to find the root for. A null entry
        /// results in the empty string.</param>
        /// <returns>The path to that entry as a string.</returns>
        /// <exception cref="InvalidOperationException" />
        string GetRootPath(ArchiveDirectoryEntry? entry);
    }
}
