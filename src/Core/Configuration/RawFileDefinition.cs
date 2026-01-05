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

using System.Diagnostics;

namespace Reko.Core.Configuration
{
    /// <summary>
    /// Definition of a raw file. Since raw files have no headers, we need one
    /// or more hints from the user, but some details may be available, or 
    /// some reasonable defaults may be provided.
    /// </summary>
    /// <remarks>
    /// <code>
    ///&lt;RawFiles&gt;
    ///  &lt;RawFile Name="ms-dos-com" Arch="x86-real-16" Env="ms-dos" Base="0C00:0100"&gt;
    ///    &lt;Entry Addr="0C00:0000" Name="MsDosCom_Start"&gt;
    ///      &lt;Register Name="ax" Value="0" /&gt;
    ///    &lt;/Entry&gt;
    ///  &lt;/RawFile&gt;
    ///&lt;/RawFiles&gt;
    /// </code>
    /// </remarks>

    [DebuggerDisplay("{Name}")]
    public class RawFileDefinition
    {
        /// <summary>
        /// Creates an instance of the <see cref="RawFileDefinition"/> class.
        /// </summary>
        public RawFileDefinition()
        {
            this.EntryPoint = new EntryPointDefinition();
        }

        /// <summary>
        /// The identifier for this raw file definition
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// A human-friendly description of this raw file format.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Default architecture to use, if any.
        /// </summary>
        public string? Architecture { get; set; }

        /// <summary>
        /// Default platform to use, if any.
        /// </summary>
        public string? Environment { get; set; }

        /// <summary>
        /// Default loading address to use, if any.
        /// </summary>
        public string? BaseAddress { get; set; }

        /// <summary>
        /// Type name of the loader of this class.
        /// </summary>
        public string? Loader { get; set; }

        /// <summary>
        /// Entry point, if any.
        /// </summary>
        public EntryPointDefinition EntryPoint { get; set; }
    }

    /// <summary>
    /// Defines an entry point in a binary file.
    /// </summary>
    //    <Entry Addr="0C00:0000" Name="MsDosCom_Start">
    //      <Register Name="ax" Value="0" />
    //    </Entry>
    public class EntryPointDefinition
    {
        /// <summary>
        /// Optional name of the entry point.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The address of the entry point serialized as a string.
        /// The architecture is responsible for converting the string
        /// to a <see cref="Reko.Core.Address"/>.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// If true, the <see cref="Address"/> property indicates an 
        /// indirect entry point. Reko will use the address located at
        /// <see cref="Address"/> as the actual entry point.
        /// </summary>
        public bool Follow { get; set; }
    }
}