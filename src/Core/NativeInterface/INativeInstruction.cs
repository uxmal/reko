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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    [ComVisible(true)]
    [Guid("2CAF9227-76D6-4DED-BC74-B95801E1524E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    [NativeInterop]
    public interface INativeInstruction
    {
        [PreserveSig] void GetInfo(out NativeInstructionInfo info);
        [PreserveSig] void Render(INativeInstructionWriter writer, MachineInstructionWriterOptions options);
    }

    [StructLayout(LayoutKind.Sequential)]
    [NativeInterop]
    public struct NativeInstructionInfo
    {
        public ulong LinearAddress;
        public uint Length;
        public uint InstructionClass;
        public int Mnemonic;
    }
}
