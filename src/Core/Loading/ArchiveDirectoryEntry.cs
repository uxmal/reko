#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Text;

namespace Reko.Core.Loading
{
    public interface ArchiveDirectoryEntry
    {
        /// <summary>
        /// Name of this directory entry.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parent of this entry, or null if there is no parent.
        /// </summary>
        ArchiveDirectoryEntry? Parent { get; }
    }

    public interface ArchivedFile : ArchiveDirectoryEntry
    {
        long Length { get; }

        byte[] GetBytes();

        /// <summary>
        /// Attempt to load the archived file as an <see cref="ILoadedImage"/>.
        /// </summary>
        /// <param name="services">Provides access to services the loader may need.</param>
        /// <param name="addrPreferred">Optional preferred loading address.</param>
        /// <returns>An <see cref="ILoadedImage"/> if the image was loadable, otherwise
        /// a <see cref="Blob" />.</returns>
        ILoadedImage LoadImage(IServiceProvider services, Address? addrPreferred);
    }

    public interface ArchivedFolder : ArchiveDirectoryEntry
    {
        ICollection<ArchiveDirectoryEntry> Entries { get; }
    }
}
