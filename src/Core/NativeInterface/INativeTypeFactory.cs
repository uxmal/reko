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
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Native interface equivalent of <see cref="Types.TypeFactory"/>.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("22D115E1-E432-4FD1-86D6-F42225063768")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeTypeFactory
    {
        /// <summary>
        /// Creates an array of the specified data type.
        /// </summary>
        /// <param name="dt">Element data type.</param>
        /// <param name="length">Number elements.</param>
        /// <returns>A handle to the new array type.</returns>
        [PreserveSig] HExpr ArrayOf(HExpr dt, int length);

        /// <summary>
        /// Creates a pointer to the specified data type.
        /// </summary>
        /// <param name="dt">Pointee data type.</param>
        /// <param name="byte_size">Size of the pointer.</param>
        /// <returns>A handle to the created pointer.</returns>
        [PreserveSig] HExpr PtrTo(HExpr dt, int byte_size);

        /// <summary>
        /// Starts a new structure definition.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="byte_size">Size of the struct is known; or 0 if it isnt.
        /// </param>
        [PreserveSig] void BeginStruct(HExpr dt, int byte_size);

        /// <summary>
        /// Adds a field to the current structure definition.
        /// </summary>
        /// <param name="dt">Data type of the field.</param>
        /// <param name="offset">Offset within the structire of that field.</param>
        /// <param name="name">Optional name of the field.</param>
        [PreserveSig] void Field(HExpr dt, int offset, [MarshalAs(UnmanagedType.LPStr)]  string name);

        /// <summary>
        /// Finishes the current structure definition.
        /// </summary>
        /// <returns>A handle to the structure type.</returns>
        [PreserveSig] HExpr EndStruct();

        /// <summary>
        /// Starts a function type definition.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="byte_size">Size of a pointer.</param>
        [PreserveSig] void BeginFunc(HExpr dt, int byte_size);

        /// <summary>
        /// Adds a parameter to the current function definition.
        /// </summary>
        /// <param name="dt">Data type of the parameter.</param>
        /// <param name="name">Name of teh parameter.</param>
        [PreserveSig] void Parameter(HExpr dt, [MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// Finishes a function type definition.
        /// </summary>
        /// <returns>A handle wrapping <see cref="Types.FunctionType"/>.</returns>
        [PreserveSig] HExpr EndFunc();
    }
}
