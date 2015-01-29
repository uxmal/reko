#region License
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.Win32
{
    using TWord = System.UInt32;        // May need this for Win64 support.

    public class Win32Emulator : IPlatformEmulator, IImportResolver
    {
        private List<string> modules;
        private Dictionary<string, Dictionary<string, Action<IProcessorEmulator>>> wellKnownFunctions;
        private ExternalProcedure epDummy;
        private TWord uPseudoFn;
        private LoadedImage img;

        public Win32Emulator(LoadedImage img, Dictionary<Address, ImportReference> importReferences)
        {
            this.img = img;
            this.uPseudoFn = 0xDEAD0000u;   // unlikely to be a real pointer to a function
            this.InterceptedCalls = new Dictionary<uint, ImportReference>();
            InterceptCallsToImports(importReferences);

            modules = new List<string>
            {
                "kernel32.dll",
                "user32.dll"
            };

            wellKnownFunctions = new Dictionary<string, Dictionary<string, Action<IProcessorEmulator>>>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "kernel32.dll", new Dictionary<string, Action<IProcessorEmulator>>
                    {
                        { "LoadLibraryA", LoadLibraryA },
                        { "GetProcAddress", GetProcAddress },
                        { "ExitProcess", ExitProcess },
                        { "VirtualProtect", VirtualProtect }
                    }
                    },
                    { "user32.dll", new Dictionary<string, Action<IProcessorEmulator>>
                    {
                        { "MessageBoxA", NYI }
                    }
                    }
            };
            epDummy = new ExternalProcedure(">Dummy<", null);
        }

        public Dictionary<TWord, ImportReference> InterceptedCalls { get; private set; }


        private void InterceptCallsToImports(Dictionary<Address, ImportReference> importReferences)
        {
            foreach (var imp in importReferences)
            {
                uint pseudoPfn = AddInterceptedCall(imp.Value);
                img.WriteLeUInt32(imp.Key, pseudoPfn);
            }
        }

        void LoadLibraryA(IProcessorEmulator emulator)
        {
            // M[Esp] is return address.
            // M[Esp + 4] is pointer to DLL name.
            uint esp = (uint)emulator.ReadRegister(Registers.esp);
            uint pstrLibName = img.ReadLeUInt32(esp + 4u - img.BaseAddress.Linear);
            string szLibName = ReadMbString(img, pstrLibName);
            uint hModule = (uint)modules.IndexOf(szLibName.ToLower());
            if ((int)hModule < 0)
                throw new NotImplementedException(string.Format("Unknown library {0}.", szLibName));
            hModule += 10;
            emulator.WriteRegister(Registers.eax, hModule);

            // Clean up the stack.
            emulator.WriteRegister(Registers.esp, esp + 8);
        }

        void GetProcAddress(IProcessorEmulator emulator)
        {
            // M[esp] is return address
            // M[esp + 4] is hmodule
            // M[esp + 4] is pointer to function name
            uint esp = (uint)emulator.ReadRegister(Registers.esp);
            uint hmodule = img.ReadLeUInt32(esp + 4u - img.BaseAddress.Linear);
            uint pstrFnName = img.ReadLeUInt32(esp + 8u - img.BaseAddress.Linear);
            if ((pstrFnName & 0xFFFF0000) != 0)
            {
                string importName = ReadMbString(img, pstrFnName);
                hmodule -= 10;
                var moduleName = modules[(int)hmodule];
                Dictionary<string, Action<IProcessorEmulator>> module;
                wellKnownFunctions.TryGetValue(moduleName, out module);
                Action<IProcessorEmulator> fn;
                if (!module.TryGetValue(importName, out fn))
                    throw new NotImplementedException();
                uint uIntercept = AddInterceptedCall(new NamedImportReference(null, moduleName, importName));
                emulator.WriteRegister(Registers.eax, uIntercept);
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
            emulator.WriteRegister(Registers.eax, 1u);
            emulator.WriteRegister(Registers.esp, esp + 20);
        }

        void NYI(IProcessorEmulator emulator)
        {
            throw new NotImplementedException();
        }

        private string ReadMbString(LoadedImage img, TWord pstrLibName)
        {
            int iStart = (int)(pstrLibName - img.BaseAddress.Linear);
            int i = iStart;
            for (i = iStart; img.Bytes[i] != 0; ++i)
            {
            }
            return Encoding.ASCII.GetString(img.Bytes, iStart, i - iStart);
        }

        internal void CallImportedProcedure(IProcessorEmulator emu, ImportReference impProc)
        {
            var proc = impProc.ResolveImportedProcedure(this, null, null);
            var simProc = proc as SimulatedProc;
            if (simProc == null)
                throw new NotImplementedException();
            simProc.Emulator(emu);
        }

        ExternalProcedure IImportResolver.ResolveProcedure(string moduleName, string importName, Platform platform)
        {
            Dictionary<string, Action<IProcessorEmulator>> module;
            if (!this.wellKnownFunctions.TryGetValue(moduleName, out module))
                return epDummy;
            Action<IProcessorEmulator> fn;
            if (!module.TryGetValue(importName, out fn))
                return epDummy;
            return new SimulatedProc(importName, fn);
        }

        ExternalProcedure IImportResolver.ResolveProcedure(string moduleName, int ordinal, Platform platform)
        {
            throw new NotImplementedException();
        }

        private TWord AddInterceptedCall(ImportReference importReference)
        {
            InterceptedCalls[++this.uPseudoFn] = importReference;
            return uPseudoFn;
        }

        public bool InterceptCall(IProcessorEmulator emu, TWord l)
        {
            ImportReference impProc;
            if (!this.InterceptedCalls.TryGetValue(l, out impProc))
                return false;
            // Called an intercepted procedure. 
            CallImportedProcedure(emu, impProc);
            return true;
        }

        public class SimulatedProc : ExternalProcedure
        {
            public SimulatedProc(string name, Action<IProcessorEmulator> emulator) : base(name, null) { Emulator = emulator; }

            public Action<IProcessorEmulator> Emulator;
        }
    }
}
