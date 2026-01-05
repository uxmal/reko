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

using Reko.Core.Configuration;
using Reko.Core.Types;

namespace Reko.Core.Loading;

/// <summary>
/// Represents the header of a binary object file.
/// </summary>
public interface IBinaryHeader
{
    /// <summary>
    /// The CPU architecture of this file.
    /// </summary>
    /// <remarks>
    /// The string is in a format suitable for direct
    /// consumption by Reko (e.g. <see cref="Reko.Core.Configuration.IConfigurationService.GetArchitecture(System.String)"/>).
    /// </remarks>
    string Architecture { get; }

    /// <summary>
    /// Type of binary object.
    /// </summary>
    BinaryFileType BinaryFileType { get; }

    /// <summary>
    /// Format-specific flags for this file.
    /// </summary>
    ulong Flags { get; }

    /// <summary>
    /// Preferred base address of this image.
    /// </summary>
    Address BaseAddress { get; }

    /// <summary>
    /// Executable start address.
    /// </summary>
    Address StartAddress { get; }

    /// <summary>
    /// The size of a pointer. 
    /// </summary>
    /// <remarks>
    /// This property not only specifies the size of pointers,
    /// but also whether they are segmented or not.
    /// </remarks>
    PrimitiveType PointerType { get; }
}

/// <summary>
/// Type of binary file.
/// </summary>
public enum BinaryFileType
{
    /// <summary>
    /// Unknown file type.
    /// </summary>
    Unknown,

    /// <summary>
    /// An executable file.
    /// </summary>
    Executable,

    /// <summary>
    /// A shared library or dynamic-link library.
    /// </summary>
    SharedLibrary,

    /// <summary>
    /// An object file.
    /// </summary>
    ObjectFile,

    /// <summary>
    /// A core dump.
    /// </summary>
    CoreImage,
}
