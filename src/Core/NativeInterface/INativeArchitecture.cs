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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Native code representation of a <see cref="RegisterStorage"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [NativeInterop]
    public struct NativeRegister
    {
#pragma warning disable CS1591
        public string Name;
        public int Number;
        public int Domain;
        public int BitSize;
        public int BitOffset;
#pragma warning restore CS1591
    }

    /// <summary>
    /// Native interface equivalent of <see cref="IProcessorArchitecture"/>.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("09FFCC1F-60C8-4058-92C2-C90DAF115250")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeArchitecture
    {
        /// <summary>
        /// Retrieves a pointer to a list of registers.
        /// </summary>
        /// <param name="registerType">Optional filter on register type.</param>
        /// <param name="n">Number of registers retrieved.</param>
        /// <param name="aregs">Pointer to an array of <see cref="NativeRegister"/> 
        /// structures.</param>
        [PreserveSig]
        void GetAllRegisters(int registerType, out int n, out IntPtr aregs);

        /// <summary>
        /// Creates a native disassembler object.
        /// </summary>
        /// <param name="bytes">A handle to an array of bytes to disassemble.</param>
        /// <param name="length">The length of the array of bytes.</param>
        /// <param name="offset">Offset within the array at which to start.</param>
        /// <param name="uAddr">The address corresponding to that offset.</param>
        /// <returns>An implementation of <see cref="INativeDisassembler"/> if successful;
        /// otherwise null.
        /// </returns>
        [PreserveSig]
        INativeDisassembler CreateDisassembler(IntPtr bytes, int length, int offset, ulong uAddr);

        /// <summary>
        /// Creates a processor specific implementation of <see cref="INativeRewriter"/>.
        /// </summary>
        /// <param name="rawBytes">A handle to an array of bytes to disassemble.</param>
        /// <param name="length">The length of the array of bytes.</param>
        /// <param name="offset">Offset within the array at which to start.</param>
        /// <param name="uAddress">The address corresponding to that offset.</param>
        /// <param name="m">Factory interface to generate Reko RTL.</param>
        /// <param name="typeFactory">Factory interface to generate Reko data types.</param>
        /// <param name="host">General context for lifting code to RTL.</param>
        /// <returns>An implementation of <see cref="INativeRewriter"/> if successful;
        /// otherwise null.
        /// </returns>

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
