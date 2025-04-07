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

using System.ComponentModel;

namespace Reko.Core
{
    /// <summary>
    /// Represents a file that only used for the metdata it contains.
    /// </summary>
    [Designer("Reko.Gui.Design.MetadataFileDesigner,Reko.Gui")]
    public class MetadataFile 
    {
        /// <summary>
        /// Executable module associated with this metadata file.
        /// </summary>
        public string? ModuleName { get; set; }

        /// <summary>
        /// The location from which the metadat file was loaded.
        /// </summary>
        public ImageLocation? Location { get; set; }
    }
}
