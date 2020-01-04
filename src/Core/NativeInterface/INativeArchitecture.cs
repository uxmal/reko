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
    [StructLayout(LayoutKind.Sequential)]
    [NativeInterop]
    public struct NativeRegister
    {
        public string Name;
        public int Number;
        public int Domain;
        public int BitSize;
        public int BitOffset;
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("09FFCC1F-60C8-4058-92C2-C90DAF115250")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeArchitecture
    {
        [PreserveSig]
        void GetAllRegisters(int registerType, out int n, out IntPtr aregs);

        [PreserveSig]
        INativeDisassembler CreateDisassembler(IntPtr bytes, int length, int offset, ulong uAddr);

        [PreserveSig]
        INativeRewriter CreateRewriter(
            IntPtr rawBytes,
            int length,
		    int offset,
            ulong uAddress,
		    INativeRtlEmitter m,
            INativeTypeFactory typeFactory,
		    INativeRewriterHost host);
    }
}
