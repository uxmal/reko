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

using Reko.Core.Loading;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Services
{
    /// <summary>
    /// Used by loaders that have encountered an archive and need the user to select 
    /// the file in the archive that is to be decompiled.
    /// </summary>
    public interface IArchiveBrowserService
    {
        /// <summary>
        /// Shows a dialog and waits for the user to select a file from the archive.
        /// </summary>
        /// <param name="archive">The archive to browse</param>
        /// <returns>An instance of <see cref="ArchivedFile"/> if the user made a selection,
        /// null otherwise.
        /// </returns>
        ValueTask<ArchivedFile?> SelectFileFromArchive(IArchive archive);
    }
}
