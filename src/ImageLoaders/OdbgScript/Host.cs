#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.OdbgScript
{
    public interface IHost
    {
        SegmentMap SegmentMap { get; }
        object TS_LOG_COMMAND { get; set; }

        ulong TE_AllocMemory(ulong size);
        int AssembleEx(string asm, ulong addr);
        bool DialogASK(string title, out string returned);
        bool DialogMSG(string msg, out int input);
        bool DialogMSGYN(string msg, out int input);
        string Disassemble(byte[] buffer, ulong addr, out int opsize);
        MachineInstruction DisassembleEx(Address addr);
        ulong FindHandle(ulong var, string sClassName, ulong x, ulong y);
        bool TE_FreeMemory(ulong pmemforexec);
        bool TE_FreeMemory(ulong addr, ulong size);
        object TE_GetCurrentThreadHandle();
        uint TE_GetCurrentThreadId();
        List<string> getlines_file(string p);
        ulong TE_GetMainThreadId();
        ulong TE_GetMainThreadHandle();
        bool TE_GetMemoryInfo(ulong addr, out MEMORY_BASIC_INFORMATION MemInfo);
        bool TE_GetModules(List<MODULEENTRY32> Modules);
        object TE_GetProcessHandle();
        ulong TE_GetProcessId();
        string TE_GetTargetDirectory();
        string TE_GetTargetPath();
        string TE_GetOutputPath();
        int LengthDisassemble(byte[] membuf, int i);
        int LengthDisassembleBackEx(ulong addr);
        uint LengthDisassembleEx(ulong addr);
        void TE_Log(string message);
        void TE_Log(string message, object p);
        void MsgError(string message);
        void SetOriginalEntryPoint(ulong ep);
        bool TryReadBytes(ulong addr, ulong memlen, byte[] membuf);
        bool WriteMemory(ulong addr, int length, byte[] membuf);
        bool WriteMemory(ulong addr, ulong qw);
        bool WriteMemory(ulong addr, uint dw);
        bool WriteMemory(ulong addr, ushort w);
        bool WriteMemory(ulong addr, byte b);
        bool WriteMemory(ulong target, double value);
    }

    public class Host : IHost
    {
        public const int Cancel = 0;
        public const int OK = 1;

        private OdbgScriptLoader loader;

		public Host(OdbgScriptLoader loader, SegmentMap segmentMap)
        {
            this.loader = loader;
			this.SegmentMap = segmentMap;
        }

        public SegmentMap SegmentMap { get; set; }
        public object TS_LOG_COMMAND { get; set; }

        public virtual ulong TE_AllocMemory(ulong size)
        {
            throw new NotImplementedException();
        }

        public virtual bool DialogMSG(string msg, out int input)
        {
            loader.Services.RequireService<IDiagnosticsService>().Warn(msg);
            input = 0;
            return true;
        }

        public virtual bool DialogMSGYN(string msg, out int input)
        {
            throw new NotImplementedException();
        }

        public virtual void TE_FreeMemory(ulong p1, uint p2)
        {
            throw new NotImplementedException();
        }

        public virtual bool DialogASK(string title, out string returned)
        {
            throw new NotImplementedException();
        }

        public virtual int AssembleEx(string p, ulong addr)
        {
            throw new NotImplementedException();
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
            loader.Services.RequireService<IDiagnosticsService>().Error(message);
        }

        public virtual bool TE_GetMemoryInfo(ulong addr, out MEMORY_BASIC_INFORMATION MemInfo)
        {
            SegmentMap map = loader.ImageMap;
            ImageSegment segment;
            if (map.TryFindSegment(Address.Ptr32((uint)addr), out segment))
            {
                MemInfo = new MEMORY_BASIC_INFORMATION
                {
                    AllocationBase = segment.Address.ToLinear(),
                    BaseAddress = segment.Address.ToLinear(),
                    RegionSize = segment.Size,
                };
                return true;
            }
            else
            {
                MemInfo = null;
                return false;
            }
        }

        public virtual bool TryReadBytes(ulong addr, ulong memlen, byte[] membuf)
        {
            ImageSegment seg;
            var ea= Address.Ptr32((uint)addr);
            if (!SegmentMap.TryFindSegment(ea, out seg))
                return false;
            return seg.MemoryArea.TryReadBytes(ea, (int)memlen, membuf);
        }

        public virtual object TE_GetProcessHandle()
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_GetModules(List<MODULEENTRY32> Modules)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_FreeMemory(ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_FreeMemory(ulong addr, ulong size)
        {
            throw new NotImplementedException();
        }

        public virtual uint LengthDisassembleEx(ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual string TE_GetTargetPath()
        {
            return loader.Filename;
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
            throw new NotImplementedException();
        }

        public virtual void TE_Log(string message, object p)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteMemory(ulong addr, int p, byte[] membuf)
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

        public virtual MachineInstruction DisassembleEx(Address addr)
        {
            ImageSegment segment;
            if (!SegmentMap.TryFindSegment(addr, out segment))
                throw new AccessViolationException();
            var rdr = loader.Architecture.CreateImageReader(segment.MemoryArea, addr);
            var dasm = (X86Disassembler)loader.Architecture.CreateDisassembler(rdr);
            return dasm.DisassembleInstruction();
        }

        public virtual uint TE_GetCurrentThreadId()
        {
            throw new NotImplementedException();
        }

        public virtual string Disassemble(byte[] buffer, ulong addr, out int opsize)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteMemory(ulong target, double d)
        {
            throw new NotImplementedException();
        }
        public virtual bool WriteMemory(ulong target, ulong qw)
        {
            throw new NotImplementedException();
        }
        public virtual bool WriteMemory(ulong target, uint dw)
        {
            throw new NotImplementedException();
        }
        public virtual bool WriteMemory(ulong target, ushort w)
        {
            throw new NotImplementedException();
        }
        public virtual bool WriteMemory(ulong target, byte b)
        {
            throw new NotImplementedException();
        }

        public virtual ulong FindHandle(ulong var, string sClassName, ulong x, ulong y)
        {
            throw new NotImplementedException();
        }

        public virtual int LengthDisassembleBackEx(ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual void SetOriginalEntryPoint(ulong ep)
        {
            loader.OriginalEntryPoint = Address.Ptr32((uint)ep);
        }
    }
}
