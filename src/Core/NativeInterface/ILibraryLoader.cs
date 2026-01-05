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

using System;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Interface for loading and unloading dynamic libraries (e.g. <c>.dll</c>,
    /// <c>.so</c>).
    /// </summary>
	public interface ILibraryLoader
	{
        /// <summary>
        /// Loads the library from the give file system location.
        /// </summary>
        /// <param name="libPath">Path to dynamlic library.</param>
        /// <returns>A "handle" to the loaded library if successful; otherwise null.
        /// </returns>
		IntPtr LoadLibrary(string libPath);

        /// <summary>
        /// Unloads the library.
        /// </summary>
        /// <param name="handle">Handle obtained from a call to <see cref="LoadLibrary(string)"/>.</param>
        /// <returns></returns>
		int Unload(IntPtr handle);

        /// <summary>
        /// Perorms a lookup of the symbol <paramref name="symName"/> in the loaded 
        /// dynamic library.
        /// </summary>
        /// <param name="handle">Handle obtained from a call to <see cref="LoadLibrary(string)"/>.</param>
        /// <param name="symName">Name of symbol whose value caller wants.</param>
        /// <returns>A handle (typically a pointer) to the object denoted by <paramref name="symName"/> if 
        /// it could be found; otherwise null.
        /// </returns>
		IntPtr GetSymbol(IntPtr handle, string symName);
    }
}
