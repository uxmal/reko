#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.ImageLoaders.OdbgScript
{
    public class OdbgScriptHost : IOdbgScriptHost
    {
        public const int Cancel = 0;
        public const int OK = 1;

        private readonly OdbgScriptLoader loader;
        private readonly Program program;
        private ImageSegment heap;
        private ulong heapAlloc;

		public OdbgScriptHost(OdbgScriptLoader loader, Program program)
        {
            this.loader = loader;
            this.program = program;
			this.SegmentMap = program.SegmentMap;
            this.heap = null!;
        }

        public SegmentMap SegmentMap { get; set; }

        public virtual Address AllocateMemory(ulong size)
        {
            if (heap == null)
            {
                // Find an available spot in the address space & align it up to a 16-byte boundary.
                var maxSegment = SegmentMap.Segments.Values
                    .OrderByDescending(s => s.Address.ToLinear() + s.Size)
                    .First();
                var addrHeap = (maxSegment.Address + maxSegment.Size).Align(0x10);

                // Make a 1 MiB heap. We want as simple an implementation as possible,
                // since OllyDebug scripts are not expected to be running very long.
                this.heap = SegmentMap.AddSegment(new ByteMemoryArea(addrHeap, new byte[1024 * 1024]), ".Emulated_heap", AccessMode.ReadWrite);
                this.heapAlloc = 0;
            }
            var newHeapAlloc = heapAlloc + size;
            if ((uint) heap.MemoryArea.Length <= newHeapAlloc)
                return null!;
            var addrChunk = (heap.MemoryArea.BaseAddress + heapAlloc).Align(0x10);
            this.heapAlloc = newHeapAlloc;
            return addrChunk;
        }

        public virtual bool FreeMemory(Address addr)
        {
            // We leak memory
            return true;
        }

        public virtual bool FreeMemory(Address addr, ulong size)
        {
            // We leak memory
            return true;
        }

        public virtual void AddSegmentReference(Address addr, ushort seg)
        {
            loader.AddSegmentReference(addr, seg);
        }

        public virtual bool DialogMSG(string msg, out int input)
        {
            loader.Services.RequireService<IEventListener>().Info(msg);
            input = 0;
            return true;
        }

        public virtual bool DialogMSGYN(string msg, out int dialogResult)
        {
            throw new NotImplementedException();
        }

        public virtual bool DialogASK(string title, out string returned)
        {
            throw new NotImplementedException();
        }

        public virtual int Assemble(string asmText, Address addr)
        {
            var asm = program.Architecture.CreateAssembler(null);
            return asm.AssembleFragmentAt(program, addr, asmText);
        }

        public virtual List<string> getlines_file(string p)
        {
            throw new NotImplementedException();
        }

        public virtual string TE_GetTargetDirectory()
        {
            throw new NotImplementedException();
        }

        public virtual object TE_GetCurrentThreadHandle()
        {
            throw new NotImplementedException();
        }

        public virtual void MsgError(string message)
        {
            loader.Services.RequireService<IEventListener>().Error(message);
        }

        public virtual bool TE_GetMemoryInfo(Address addr, out MEMORY_BASIC_INFORMATION MemInfo)
        {
            SegmentMap map = loader.ImageMap;
            if (map.TryFindSegment(addr, out ImageSegment? segment))
            {
                MemInfo = new MEMORY_BASIC_INFORMATION
                {
                    AllocationBase = segment.Address.ToLinear(),
                    BaseAddress = segment.Address,
                    RegionSize = segment.Size,
                };
                return true;
            }
            else
            {
                MemInfo = null!;
                return false;
            }
        }

        public virtual bool TryReadBytes(Address addr, int memlen, byte[] membuf)
        {
            if (!SegmentMap.TryFindSegment(addr, out ImageSegment? seg))
                return false;
            if (seg.MemoryArea is ByteMemoryArea bmem)
            {
                return bmem.TryReadBytes(addr, (int) memlen, membuf);
            }
            else
            {
                return false;
            }
        }

        public virtual object TE_GetProcessHandle()
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_GetModules(List<MODULEENTRY32> Modules)
        {
            throw new NotImplementedException();
        }

        public virtual uint LengthDisassembleEx(Address addr)
        {
            throw new NotImplementedException();
        }

        public virtual string? TE_GetTargetPath()
        {
            //$REVIEW: this may not be what was intended.
            return loader?.ImageLocation.FilesystemPath;
        }

        public virtual ulong TE_GetProcessId()
        {
            throw new NotImplementedException();
        }

        public virtual ulong TE_GetMainThreadHandle()
        {
            throw new NotImplementedException();
        }

        public virtual ulong TE_GetMainThreadId()
        {
            throw new NotImplementedException();
        }

        public virtual void TE_Log(string message)
        {
            Debug.WriteLine(message);
        }

        public virtual bool WriteMemory(Address addr, int p, byte[] membuf)
        {
            throw new NotImplementedException();
        }

        public virtual int LengthDisassemble(byte[] membuf, int i)
        {
            throw new NotImplementedException();
        }

        public virtual string TE_GetOutputPath()
        {
            return "";
        }

        public virtual bool TE_WriteMemory(ulong addr, ulong len, byte[] membuf)
        {
            throw new NotImplementedException();
        }

        public virtual MachineInstruction? Disassemble(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out ImageSegment? segment))
                throw new AccessViolationException();
            var rdr = loader.Architecture.CreateImageReader(segment.MemoryArea, addr);
            var dasm = (X86Disassembler)loader.Architecture.CreateDisassembler(rdr);
            return dasm.DisassembleInstruction();
        }

        public virtual uint TE_GetCurrentThreadId()
        {
            throw new NotImplementedException();
        }

        public virtual string Disassemble(byte[] buffer, Address addr, out int opsize)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteMemory(Address target, double d)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteMemory(Address target, ulong qw)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteMemory(Address target, uint dw)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteMemory(Address target, ushort w)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteMemory(Address target, byte b)
        {
            throw new NotImplementedException();
        }

        public virtual ulong FindHandle(ulong var, string sClassName, ulong x, ulong y)
        {
            throw new NotImplementedException();
        }

        public virtual int LengthDisassembleBackEx(Address addr)
        {
            throw new NotImplementedException();
        }

        public virtual void SetOriginalEntryPoint(Address ep)
        {
            loader.OriginalEntryPoint = ep;
        }
    }
}
