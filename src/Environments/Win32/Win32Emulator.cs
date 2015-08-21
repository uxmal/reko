﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.Win32
{
    using Reko.Core.Serialization;        // May need this for Win64 support.
    using System.Diagnostics;
    using TWord = System.UInt32;

    /// <summary>
    /// Emulates the Win32 operating environment. In particular, intercepts calls to GetProcAddress
    /// so that the procedures used by the decompiled program can be gleaned. 
    /// </summary>
    public class Win32Emulator : IPlatformEmulator, IImportResolver
    {
        private Dictionary<string, Module> modules;
        private TWord uPseudoFn;
        private LoadedImage img;
        private Platform platform;

        public Win32Emulator(LoadedImage img, Platform platform, Dictionary<Address, ImportReference> importReferences)
        {
            this.img = img;
            this.platform = platform;
            this.uPseudoFn = 0xDEAD0000u;   // unlikely to be a real pointer to a function
            this.InterceptedCalls = new Dictionary<uint, ExternalProcedure>();

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
            Module module;
            if (!modules.TryGetValue(moduleName, out module))
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
            ProcedureCharacteristics chars = null)
        {
            SimulatedProc proc;
            if (!module.Procedures.TryGetValue(procName, out proc))
            {
                var extProc = platform.LookupProcedureByName(module.Name, procName);
                proc = new SimulatedProc(procName, emulator);
                proc.Signature = extProc.Signature;
                if (chars != null)
                    proc.Characteristics = chars;
                proc.uFakedAddress = ++this.uPseudoFn;
                InterceptedCalls[proc.uFakedAddress] = proc;
                module.Procedures.Add(procName, proc);
            }
            return proc;
        }

        public Dictionary<TWord, ExternalProcedure> InterceptedCalls { get; private set; }

        private void InterceptCallsToImports(Dictionary<Address, ImportReference> importReferences)
        {
            foreach (var imp in importReferences)
            {
                uint pseudoPfn = ((SimulatedProc)imp.Value.ResolveImportedProcedure(this, null, null)).uFakedAddress;
                img.WriteLeUInt32(imp.Key, pseudoPfn);
            }
        }

        ExternalProcedure IImportResolver.ResolveProcedure(string moduleName, string importName, Platform platform)
        {
            Module module = EnsureModule(moduleName);
            return EnsureProc(module, importName, NYI);
        }

        ExternalProcedure IImportResolver.ResolveProcedure(string moduleName, int ordinal, Platform platform)
        {
            throw new NotImplementedException();
        }

        void LoadLibraryA(IProcessorEmulator emulator)
        {
            // M[Esp] is return address.
            // M[Esp + 4] is pointer to DLL name.
            uint esp = (uint)emulator.ReadRegister(Registers.esp);
            uint pstrLibName = img.ReadLeUInt32(esp + 4u - img.BaseAddress.ToUInt32());
            string szLibName = ReadMbString(img, pstrLibName);
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
            uint hmodule = img.ReadLeUInt32(esp + 4u - img.BaseAddress.ToUInt32());
            uint pstrFnName = img.ReadLeUInt32(esp + 8u - img.BaseAddress.ToUInt32());
            if ((pstrFnName & 0xFFFF0000) != 0)
            {
                string importName = ReadMbString(img, pstrFnName);
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
            uint arg1 = img.ReadLeUInt32(esp + 4u -  img.BaseAddress.ToUInt32());
            uint arg2 = img.ReadLeUInt32(esp + 8u -  img.BaseAddress.ToUInt32());
            uint arg3 = img.ReadLeUInt32(esp + 12u - img.BaseAddress.ToUInt32());
            uint arg4 = img.ReadLeUInt32(esp + 16u - img.BaseAddress.ToUInt32());
            Debug.Print("VirtualProtect({0:X8},{1:X8},{2:X8},{3:X8})", arg1, arg2, arg3, arg4);

            emulator.WriteRegister(Registers.eax, 1u);
            emulator.WriteRegister(Registers.esp, esp + 20);
        }

        void NYI(IProcessorEmulator emulator)
        {
            throw new NotImplementedException();
        }

        private string ReadMbString(LoadedImage img, TWord pstrLibName)
        {
            int iStart = (int)(pstrLibName - img.BaseAddress.ToLinear());
            int i = iStart;
            for (i = iStart; img.Bytes[i] != 0; ++i)
            {
            }
            return Encoding.ASCII.GetString(img.Bytes, iStart, i - iStart);
        }

        public bool InterceptCall(IProcessorEmulator emu, TWord l)
        {
            ExternalProcedure epProc;
            if (!this.InterceptedCalls.TryGetValue(l, out epProc))
                return false;
            ((SimulatedProc)epProc).Emulator(emu);
            return true;
        }

        public class SimulatedProc : ExternalProcedure
        {
            public SimulatedProc(string name, Action<IProcessorEmulator> emulator) : base(name, null) { Emulator = emulator; }

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
