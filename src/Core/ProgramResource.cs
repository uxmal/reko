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

using System.Collections.Generic;
using System.ComponentModel;

namespace Reko.Core
{
    /// <summary>
    /// A program resource extracted from an executable image.
    /// </summary>
    public class ProgramResource
    {
        /// <summary>
        /// Optional name of the resource.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Returns a string representation of the resource's name.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name ?? "(null)";
    }

    /// <summary>
    /// Represents a group of program resources.
    /// </summary>
    [Designer("Reko.Gui.Design.ProgramResourceGroupDesigner,Reko.Gui")]
    public class ProgramResourceGroup : ProgramResource
    {
        /// <summary>
        /// The sub-resources of this resource group.
        /// </summary>
        public List<ProgramResource> Resources { get; }

        /// <summary>
        /// Creates an instance of a resource group.
        /// </summary>
        public ProgramResourceGroup()
        {
            this.Resources = new List<ProgramResource>();
        }
    }

    /// <summary>
    /// A program resource extracted from a binary..
    /// </summary>

    [Designer("Reko.Gui.Design.ProgramResourceInstanceDesigner,Reko.Gui")]
    public class ProgramResourceInstance : ProgramResource
    {
        /// <summary>
        /// Raw bytes of the resource.
        /// </summary>
        public byte[]? Bytes { get; set; }

        /// <summary>
        /// Content type of the resource.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Optional file extension of the resource.
        /// </summary>
        public string? FileExtension { get; set; }

        /// <summary>
        /// Optional text encoding of the resource.
        /// </summary>
        public string? TextEncoding { get; set; }

        /// <summary>
        /// Returns a string representation of the resource.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"id:{this.Name}, type:{this.Type}, ext:{FileExtension}, enc:{TextEncoding}";
        }
    }
}
