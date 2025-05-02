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
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Interface used by <see cref="INativeInstruction"/> implementations to render
    /// disassembled instructions as text.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("62AB8B0D-8181-4F6A-8896-4833D695265A")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeInstructionRenderer
    {
        /// <summary>
        /// Annotations are displayed as comments at the end of the line.
        /// </summary>
        /// <param name="a"></param>
        [PreserveSig] void AddAnnotation([MarshalAs(UnmanagedType.LPStr)] string? a);

        /// <summary>
        /// Renders a mnemonic.
        /// </summary>
        /// <param name="sMnemonic">Mnemonic to render.</param>
        [PreserveSig] void WriteMnemonic([MarshalAs(UnmanagedType.LPStr)]string sMnemonic);

        /// <summary>
        /// Renders an address.
        /// </summary>
        /// <param name="formattedAddress">Formatted address.</param>
        /// <param name="uAddr">Address value.</param>
        [PreserveSig] void WriteAddress([MarshalAs(UnmanagedType.LPStr)]string formattedAddress, ulong uAddr);

        /// <summary>
        /// Skips to the next tab stop.
        /// </summary>
        [PreserveSig] void Tab();

        /// <summary>
        /// Renders a string.
        /// </summary>
        /// <param name="s">String to render.</param>
        [PreserveSig] void WriteString([MarshalAs(UnmanagedType.LPStr)] string? s);

        /// <summary>
        /// Renders a character.
        /// </summary>
        /// <param name="c">Character to render.</param>
        [PreserveSig] void WriteChar(char c);

        /// <summary>
        /// Renders an unsigned integer.
        /// </summary>
        /// <param name="n">Integer to render.</param>
        [PreserveSig] void WriteUInt32(uint n);
    }
}
