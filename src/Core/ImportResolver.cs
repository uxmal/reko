#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Core
{
    public interface IImportResolver
    {
        ExternalProcedure ResolveProcedure(string moduleName, string importName, IPlatform platform);
        ExternalProcedure ResolveProcedure(string moduleName, int ordinal, IPlatform platform);
        Identifier ResolveGlobal(string moduleName, string globalName, IPlatform platform);
        ProcedureConstant ResolveToImportedProcedureConstant(Statement stm, Constant c);
    }

    /// <summary>
    /// An import resolver tries to resolve a reference to external code or
    /// data by consulting the current project first hand, and the platform
    /// in second hand. Doing it that way allows users to override platform
    /// definitions as the need arises.
    /// </summary>
    public class ImportResolver : IImportResolver
    {
        private Project project;
        private Program program;
        private DecompilerEventListener eventListener;

        public ImportResolver(Project project, Program program, DecompilerEventListener eventListener)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            this.project = project;
            this.program = program;
            this.eventListener = eventListener;
        }

        public ExternalProcedure ResolveProcedure(string moduleName, string importName, IPlatform platform)
        {
            if (!string.IsNullOrEmpty(moduleName))
            {
                foreach (var program in project.Programs)
                {
                    ModuleDescriptor mod;
                    if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out mod))
                        continue;

                    SystemService svc;
                    if (mod.ServicesByName.TryGetValue(importName, out svc))
                    {
                        return new ExternalProcedure(svc.Name, svc.Signature, svc.Characteristics);
                    }
                }
            }

            foreach (var program in project.Programs)
            {
                FunctionType sig;
                if (program.EnvironmentMetadata.Signatures.TryGetValue(importName, out sig))
                {
                    var chr = platform.LookupCharacteristicsByName(importName);
                    if (chr != null)
                        return new ExternalProcedure(importName, sig, chr);
                    else
                        return new ExternalProcedure(importName, sig);
                }
            }

            return platform.LookupProcedureByName(moduleName, importName);
        }

        public ExternalProcedure ResolveProcedure(string moduleName, int ordinal, IPlatform platform)
        {
            foreach (var program in project.Programs)
            {
                ModuleDescriptor mod;
                if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out mod))
                    continue;

                SystemService svc;
                if (mod.ServicesByVector.TryGetValue(ordinal, out svc))
                {
                    return new ExternalProcedure(svc.Name, svc.Signature, svc.Characteristics);
                }
            }

            return platform.LookupProcedureByOrdinal(moduleName, ordinal);
        }

        public Identifier ResolveGlobal(string moduleName, string globalName, IPlatform platform)
        {
            foreach (var program in project.Programs)
            {
                ModuleDescriptor mod;
                if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out mod))
                    continue;

                DataType dt;
                if (mod.Globals.TryGetValue(globalName, out dt))
                {
                    return new Identifier(globalName, dt, new MemoryStorage());
                }
            }

            foreach (var program in project.Programs)
            {
                DataType dt;
                if (program.EnvironmentMetadata.Globals.TryGetValue(globalName, out dt))
                {
                    return new Identifier(globalName, dt, new MemoryStorage());
                }
            }
            return platform.LookupGlobalByName(moduleName, globalName);
        }

        public ProcedureConstant ResolveToImportedProcedureConstant(Statement stm, Constant c)
        {
            var addrInstruction = program.SegmentMap.MapLinearAddressToAddress(stm.LinearAddress);
            var addrImportThunk = program.Platform.MakeAddressFromConstant(c);
            ImportReference impref;
            if (!program.ImportReferences.TryGetValue(addrImportThunk, out impref))
                return null;

            var extProc = impref.ResolveImportedProcedure(
                this,
                program.Platform,
                new AddressContext(program, addrInstruction, this.eventListener));
            return new ProcedureConstant(program.Platform.PointerType, extProc);
        }

       
    }
}
