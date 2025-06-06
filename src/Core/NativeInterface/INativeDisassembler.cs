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
    /// Interface implemented by native code disassemblers.
    /// </summary>
    [ComVisible(true)]
    [Guid("10475E6B-D167-4DB3-B211-610F6073A313")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    [NativeInterop]
    public interface INativeDisassembler
    {
        /// <summary>
        /// Disassembles one instruction.
        /// </summary>
        /// <returns>An instance of <see cref="INativeInstruction"/> if all
        /// went well; otherwise null.
        /// </returns>
        [PreserveSig] INativeInstruction NextInstruction();
    }
}