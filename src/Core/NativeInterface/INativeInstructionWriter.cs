#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("62AB8B0D-8181-4F6A-8896-4833D695265A")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeInstructionWriter
    {
        /// <summary>
        /// Annotations are displayed as comments at the end of the line.
        /// </summary>
        /// <param name="a"></param>
        [PreserveSig] void AddAnnotation([MarshalAs(UnmanagedType.LPStr)] string a);

        [PreserveSig] void WriteMnemonic([MarshalAs(UnmanagedType.LPStr)]string sMnemonic);
        [PreserveSig] void WriteAddress([MarshalAs(UnmanagedType.LPStr)]string formattedAddress, ulong uAddr);
        [PreserveSig] void Tab();
        [PreserveSig] void WriteString([MarshalAs(UnmanagedType.LPStr)] string s);
        [PreserveSig] void WriteChar(char c);
        [PreserveSig] void WriteUInt32(uint n);
    }
}
