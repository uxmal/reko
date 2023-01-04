#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    public interface IOdbgScriptHost
    {
        SegmentMap SegmentMap { get; }

        Address AllocateMemory(ulong size);
        int Assemble(string asm, Address addr);
        bool DialogASK(string title, out string returned);
        bool DialogMSG(string msg, out int input);
        bool DialogMSGYN(string msg, out int dialogResult);
        string Disassemble(byte[] buffer, Address addr, out int opsize);
        MachineInstruction? Disassemble(Address addr);
        ulong FindHandle(ulong var, string sClassName, ulong x, ulong y);
        bool FreeMemory(Address addr);
        bool FreeMemory(Address addr, ulong size);
        object TE_GetCurrentThreadHandle();
        uint TE_GetCurrentThreadId();
        List<string> getlines_file(string p);
        ulong TE_GetMainThreadId();
        ulong TE_GetMainThreadHandle();
        bool TE_GetMemoryInfo(Address addr, out MEMORY_BASIC_INFORMATION MemInfo);
        bool TE_GetModules(List<MODULEENTRY32> Modules);
        object TE_GetProcessHandle();
        ulong TE_GetProcessId();
        string? TE_GetTargetDirectory();
        string? TE_GetTargetPath();
        string? TE_GetOutputPath();
        int LengthDisassemble(byte[] membuf, int i);
        int LengthDisassembleBackEx(Address addr);
        uint LengthDisassembleEx(Address addr);
        void TE_Log(string message);
        void MsgError(string message);
        void SetOriginalEntryPoint(Address ep);
        bool TryReadBytes(Address addr, int memlen, byte[] membuf);
        bool WriteMemory(Address addr, int length, byte[] membuf);
        bool WriteMemory(Address addr, ulong qw);
        bool WriteMemory(Address addr, uint dw);
        bool WriteMemory(Address addr, ushort w);
        bool WriteMemory(Address addr, byte b);
        bool WriteMemory(Address target, double value);
        void AddSegmentReference(Address addr, ushort seg);
    }
}
