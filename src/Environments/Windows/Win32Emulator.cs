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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Serialization;        // May need this for Win64 support.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TWord = System.UInt32;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Memory;
using Reko.Core.Emulation;
using Reko.Core.Machine;
using Reko.Core.Loading;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Emulates the Win32 operating environment. In particular, intercepts calls to GetProcAddress
    /// so that the procedures used by the decompiled program can be gleaned. 
    /// </summary>
    public class Win32Emulator : IPlatformEmulator, IDynamicLinker
    {
        private Dictionary<string, Module> modules;
        private TWord uPseudoFn;
        private SegmentMap map;
        private IPlatform platform;

        public Win32Emulator(SegmentMap map, IPlatform platform, Dictionary<Address, ImportReference> importReferences)
        {
            this.map = map;
            this.platform = platform;
            this.uPseudoFn = 0xDEAD0000u;   // unlikely to be a real pointer to a function
            this.InterceptedCalls = new Dictionary<Address, ExternalProcedure>();
            modules = new Dictionary<string, Module>(StringComparer.InvariantCultureIgnoreCase);
            AddWellKnownProcedures();
            InterceptCallsToImports(importReferences);
        }

        private void AddWellKnownProcedures()
        {
            var kernel32 = EnsureModule("kernel32.dll");
            EnsureProc(kernel32, "LoadLibraryA", LoadLibraryA);
            EnsureProc(kernel32, "GetProcAddress", GetProcAddress);
            EnsureProc(kernel32, "ExitProcess", ExitProcess, new ProcedureCharacteristics { Terminates = true });
            EnsureProc(kernel32, "VirtualProtect", VirtualProtect);
        }

        private Module EnsureModule(string moduleName)
        {
            if (!modules.TryGetValue(moduleName, out Module? module))
            {
                module = new Module(moduleName);
                modules.Add(module.Name, module);
                module.Handle = (TWord)modules.Count * 16u;
            }
            return module;
        }

        public SimulatedProc EnsureProc(
            Module module, 
            string procName,
            Action<IProcessorEmulator> emulator,
            ProcedureCharacteristics? chars = null)
        {
            if (!module.Procedures.TryGetValue(procName, out SimulatedProc? proc))
            {
                //$REVIEW: LookupProcedureByName could return null.
                var extProc = platform.LookupProcedureByName(module.Name, procName)!;
                proc = new SimulatedProc(procName, emulator)
                {
                    Signature = extProc.Signature
                };
                if (chars is not null)
                    proc.Characteristics = chars;
                proc.uFakedAddress = ++this.uPseudoFn;
                InterceptedCalls[Address.Ptr32(proc.uFakedAddress)] = proc;
                module.Procedures.Add(procName, proc);
            }
            return proc;
        }

        public Dictionary<Address, ExternalProcedure> InterceptedCalls { get; private set; }

        private void InterceptCallsToImports(Dictionary<Address, ImportReference> importReferences)
        {
            foreach (var imp in importReferences)
            {
                uint pseudoPfn = ((SimulatedProc)imp.Value.ResolveImportedProcedure(this, null!, null!, null!)).uFakedAddress;
                WriteLeUInt32(imp.Key, pseudoPfn);
            }
        }

        ExternalProcedure? IDynamicLinker.ResolveProcedure(string? moduleName, string importName, IPlatform platform)
        {
            if (moduleName is null)
                return null;
            Module module = EnsureModule(moduleName);
            return EnsureProc(module, importName, NYI);
        }

        ExternalProcedure IDynamicLinker.ResolveProcedure(string moduleName, int ordinal, IPlatform platform)
        {
            throw new NotImplementedException();
        }

        void LoadLibraryA(IProcessorEmulator emulator)
        {
            // M[Esp] is return address.
            // M[Esp + 4] is pointer to DLL name.
            uint esp = (uint)emulator.ReadRegister(Registers.esp);
            uint pstrLibName = ReadLeUInt32(esp + 4u);
            string szLibName = ReadMbString(pstrLibName);
            Module module = EnsureModule(szLibName);
            emulator.WriteRegister(Registers.eax, module.Handle);

            // Clean up the stack.
            emulator.WriteRegister(Registers.esp, esp + 8);
        }

        void GetProcAddress(IProcessorEmulator emulator)
        {
            // M[esp] is return address
            // M[esp + 4] is hmodule
            // M[esp + 4] is pointer to function name
            uint esp = (uint)emulator.ReadRegister(Registers.esp);
            uint hmodule = ReadLeUInt32(esp + 4u);
            uint pstrFnName = ReadLeUInt32(esp + 8u);
            if ((pstrFnName & 0xFFFF0000) != 0)
            {
                string importName = ReadMbString(pstrFnName);
                var module = modules.Values.First(m => m.Handle == hmodule);
                SimulatedProc fn = EnsureProc(module, importName, NYI);
                emulator.WriteRegister(Registers.eax, fn.uFakedAddress);
                emulator.WriteRegister(Registers.esp, esp + 12);
            }
            else
            {
                //$TODO: import by ordinal.
                throw new NotImplementedException();
            }
        }

        void ExitProcess(IProcessorEmulator emulator)
        {
            emulator.Stop();
        }

        void VirtualProtect(IProcessorEmulator emulator)
        {
            uint esp = (uint)emulator.ReadRegister(Registers.esp);
            uint uAddress = ReadLeUInt32(esp + 4u );
            uint dwSize = ReadLeUInt32(esp + 8u );
            uint newProtect = ReadLeUInt32(esp + 12u);
            uint pOldProtect = ReadLeUInt32(esp + 16u);
            Debug.Print("VirtualProtect({0:X8},{1:X8},{2:X8},{3:X8})", uAddress, dwSize, newProtect, pOldProtect);
            //$TODO: to make this work we have to keep a mapping (page -> permissions)
            // For now, return the protection of the segment.
            if (!this.map.TryFindSegment(Address.Ptr32(uAddress), out var seg))
            {
                //$BUG seg is null here. How is this expected to work?
                var prot = MapProtectionToWin32(seg!.Access);
                WriteLeUInt32(Address.Ptr32(pOldProtect), prot);
                emulator.WriteRegister(Registers.eax, 0u);
            }
            else
            {
                var oldAccess = seg.Access;
                seg.Access = MapProtectionToAccess(newProtect);
                // RWX = 0x70
                WriteLeUInt32(Address.Ptr32(pOldProtect), 0x70);
                emulator.WriteRegister(Registers.eax, 1u);
            }
            emulator.WriteRegister(Registers.esp, esp + 20);
        }

        const uint PAGE_READONLY = 0x02;
        const uint PAGE_EXECUTE = 0x10;
        const uint PAGE_EXECUTE_READ = 0x20;
        const uint PAGE_EXECUTE_READWRITE = 0x40;
        const uint PAGE_READWRITE = 0x04;

        private uint MapProtectionToWin32(AccessMode access)
        {
            if (access == AccessMode.Read)
                return PAGE_READONLY;
            if (access == AccessMode.ReadExecute)
                return PAGE_EXECUTE;
            if (access == AccessMode.ReadWriteExecute)
                return PAGE_EXECUTE_READWRITE;
            if (access == AccessMode.ReadWrite)
                return PAGE_READWRITE;
            throw new NotImplementedException($"Unimplemented protection {access}.");
        }

        private AccessMode MapProtectionToAccess(uint protection)
        {
            if (protection == PAGE_READONLY)
                return AccessMode.Read;
            if (protection == (PAGE_EXECUTE | PAGE_EXECUTE_READ | PAGE_EXECUTE_READWRITE))
                return AccessMode.ReadWriteExecute;
            if (protection == PAGE_EXECUTE)
                return AccessMode.Execute;
            if (protection == PAGE_EXECUTE_READ)
                return AccessMode.ReadExecute;
            if (protection == PAGE_READWRITE)
                return AccessMode.ReadWrite;
            throw new NotImplementedException($"Unimplemented protection 0x{protection:X}.");
        }

        void NYI(IProcessorEmulator emulator)
        {
            throw new NotImplementedException();
        }

        private uint ReadLeUInt32(uint ea)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            var addr = Address.Ptr32(ea);
            if (!map.TryFindSegment(addr, out ImageSegment? segment))
                throw new AccessViolationException();
            return ((ByteMemoryArea)segment.MemoryArea).ReadLeUInt32(addr);
        }

        private void WriteLeUInt32(uint ea, uint value)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            var addr = Address.Ptr32(ea);
            if (!map.TryFindSegment(addr, out ImageSegment? segment))
                throw new AccessViolationException();
            segment.MemoryArea.WriteLeUInt32(addr, value);
        }

        private void WriteLeUInt32(Address ea, uint value)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            if (!map.TryFindSegment(ea, out ImageSegment? segment))
                throw new AccessViolationException();
            segment.MemoryArea.WriteLeUInt32(ea, value);
        }

        private string ReadMbString(TWord pstrLibName)
        {
            var addr = Address.Ptr32(pstrLibName);
            if (!map.TryFindSegment(addr, out ImageSegment? segment))
                throw new AccessViolationException();
            var rdr = segment.MemoryArea.CreateLeReader(addr);
            var ab = new List<byte>();
            for (;;)
            {
                byte b = rdr.ReadByte();
                if (b == 0)
                    break;
                ab.Add(b);
            }
            return Encoding.ASCII.GetString(ab.ToArray());
        }

        public ImageSegment InitializeStack(IProcessorEmulator emu, ProcessorState state)
        {
            var stack = new ByteMemoryArea(Address.Ptr32(0x7FE00000), new byte[1024 * 1024]);
            var stackSeg = this.map.AddSegment(stack, "stack", AccessMode.ReadWrite);
            emu.WriteRegister(Registers.esp, (uint) stack.BaseAddress.ToLinear() +  (uint)(stack.Length - this.platform.Architecture.PointerType.Size));
            return stackSeg;
        }

        public void TearDownStack(ImageSegment? stackSeg)
        {
            if (stackSeg is not null)
            {
                this.map.Segments.Remove(stackSeg.Address);
            }
        }

        public bool InterceptCall(IProcessorEmulator emu, ulong l)
        {
            if (!this.InterceptedCalls.TryGetValue(Address.Ptr32((uint)l), out ExternalProcedure? epProc))
                return false;
            ((SimulatedProc)epProc).Emulator(emu);
            return true;
        }

        public bool EmulateSystemCall(IProcessorEmulator emulator, params MachineOperand[] operands)
        {
            return false;
        }


        public Expression ResolveToImportedValue(Statement stm, Constant c)
        {
            throw new NotImplementedException();
        }

        public Expression ResolveImport(string? moduleName, string name, IPlatform platform)
        {
            throw new NotImplementedException();
        }

        public Expression ResolveImport(string moduleName, int ordinal, IPlatform platform)
        {
            throw new NotImplementedException();
        }

        public class SimulatedProc : ExternalProcedure
        {
            public SimulatedProc(string name, Action<IProcessorEmulator> emulator)
                : base(name, new FunctionType())
            {
                Emulator = emulator;
            }

            public TWord uFakedAddress;
            public Action<IProcessorEmulator> Emulator;
        }

        public class Module
        {
            public string Name;
            public TWord Handle;
            public Dictionary<string, SimulatedProc> Procedures;

            public Module(string p)
            {
                this.Name = p;
                Procedures = new Dictionary<string, SimulatedProc>();
            }
        }
    }
}
