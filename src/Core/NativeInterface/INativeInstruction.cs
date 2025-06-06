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

using Reko.Core.Machine;
using System;
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Interface used to represent an instruction.
    /// </summary>
    [ComVisible(true)]
    [Guid("2CAF9227-76D6-4DED-BC74-B95801E1524E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    [NativeInterop]
    public interface INativeInstruction
    {
        /// <summary>
        /// Get instruction information.
        /// </summary>
        /// <param name="info">Returned value.</param>
        [PreserveSig] void GetInfo(out NativeInstructionInfo info);

        /// <summary>
        /// Renders the instruction using the specified renderer.
        /// </summary>
        /// <param name="renderer">Output sink.</param>
        /// <param name="options">Options to control the rendering.</param>
        [PreserveSig] void Render(INativeInstructionRenderer renderer, MachineInstructionRendererFlags options);
    }

    /// <summary>
    /// Instruction details.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [NativeInterop]
    public struct NativeInstructionInfo
    {
        /// <summary>
        /// The address of the instruction.
        /// </summary>
        public ulong LinearAddress;

        /// <summary>
        /// The size of the instruction.
        /// </summary>
        public uint Length;

        /// <summary>
        /// Instruction class.
        /// </summary>
        public uint InstructionClass;

        /// <summary>
        /// Mnemonic.
        /// </summary>
        public int Mnemonic;
    }
}
