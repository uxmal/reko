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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Core.Operators;

namespace Reko.Core
{
    public interface IDynamicLinker
    {
        ExternalProcedure ResolveProcedure(string moduleName, string importName, IPlatform platform);
        ExternalProcedure ResolveProcedure(string moduleName, int ordinal, IPlatform platform);
        Expression ResolveImport(string moduleName, string globalName, IPlatform platform);
        Expression ResolveImport(string moduleName, int ordinal, IPlatform platform);
        Expression ResolveToImportedValue(Statement stm, Constant c);
    }

    /// <summary>
    /// The dynamic linker tries to resolve a reference to external code or
    /// data by consulting the current project first hand, and the platform
    /// in second hand. Doing it that way allows users to override platform
    /// definitions as the need arises.
    /// </summary>
    public class DynamicLinker : IDynamicLinker
    {
        private readonly Project project;
        private readonly Program program;
        private readonly DecompilerEventListener eventListener;
        private readonly Dictionary<string, ImageSymbol> localProcs;

        public DynamicLinker(Project project, Program program, DecompilerEventListener eventListener)
        {
            this.project = project ?? throw new ArgumentNullException("project");
            this.program = program;
            this.eventListener = eventListener;
            this.localProcs = program.ImageSymbols.Values
                .Where(sym => sym.Name != null && sym.Type == SymbolType.Procedure)
                .GroupBy(sym => sym.Name)
                .ToDictionary(g => g.Key, g => g.First());
        }

        private void EnsureSignature(Program program, SystemService svc)
        {
            if (svc.Signature == null)
            {
                if (program.EnvironmentMetadata.Signatures.TryGetValue(svc.Name, out var fnc)) {
                    svc.Signature = fnc;
                }
            }
        }

        public ExternalProcedure ResolveProcedure(string moduleName, string importName, IPlatform platform)
        {
            var ep = LookupProcedure(moduleName, importName, platform);
            if (ep != null)
                return ep;
            // Can we guess at the signature?
            var sProc = platform.SignatureFromName(importName);
            if (sProc != null)
            {
                var loader = program.CreateTypeLibraryDeserializer();
                ep = loader.LoadExternalProcedure(sProc);
                if (!ep.Signature.ParametersValid)
                {
                    // We found a imported procedure but couldn't find its signature.
                    // Perhaps it has been mangled, and we can use the stripped name.
                    var epNew = LookupProcedure(null, ep.Name, platform);
                    if (epNew != null)
                    {
                        ep = epNew;
                    }
                }
                return ep;
            }
            return null;
        }

        private ExternalProcedure LookupProcedure(string moduleName, string importName, IPlatform platform)
        {
            if (!string.IsNullOrEmpty(moduleName))
            {
                foreach (var program in project.Programs)
                {
                    if (program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out var mod) &&
                        mod.ServicesByName.TryGetValue(importName, out var svc))
                    {
                        return new ExternalProcedure(svc.Name, svc.Signature, svc.Characteristics);
                    }
                }
            }

            foreach (var program in project.Programs)
            {
                if (program.EnvironmentMetadata.Signatures.TryGetValue(importName, out var sig))
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
                if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out var mod))
                    continue;

                if (mod.ServicesByOrdinal.TryGetValue(ordinal, out var svc))
                {
                    EnsureSignature(program, svc);
                    if (svc.Signature != null)
                    {
                        return new ExternalProcedure(svc.Name, svc.Signature, svc.Characteristics);
                    }
                    else
                    {
                        // We have a name for the external procedure, but can't find a proper signature for it. 
                        // So we make a "dumb" one. It's better than nothing.
                        eventListener.Warn(
                            new NullCodeLocation(moduleName),
                            "Unable to resolve signature for {0}", svc.Name);
                        return new ExternalProcedure(svc.Name, new FunctionType());
                    }
                }
            }
            return platform.LookupProcedureByOrdinal(moduleName, ordinal);
        }

        public Expression ResolveImport(string moduleName, string name, IPlatform platform)
        {
            var global = LookupImport(moduleName, name, platform);
            if (global != null)
                return global;
            var t = platform.DataTypeFromImportName(name);
            //$REVIEW: the way imported symbols are resolved as 
            // globals or functions needs a revisit.
            if (t != null && !(t.Item2 is SerializedSignature))
            {
                var dSer = program.CreateTypeLibraryDeserializer();
                var dt = (t.Item2 == null) ?
                    new UnknownType() :
                    t.Item2.Accept(dSer);
                return Identifier.Global(t.Item1, dt);
            }
            else
                return null;
        }

        private Expression LookupImport(string moduleName, string name, IPlatform platform)
        {
            if (!string.IsNullOrEmpty(moduleName))
            {
                foreach (var program in project.Programs)
                {
                    // Try to find the imported procedure based on module + name.
                    if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out ModuleDescriptor mod))
                        continue;

                    if (mod.ServicesByName.TryGetValue(name, out SystemService svc))
                    {
                        var ep = new ExternalProcedure(svc.Name, svc.Signature, svc.Characteristics);
                        return new ProcedureConstant(platform.PointerType, ep);
                    }

                    if (mod.GlobalsByName.TryGetValue(name, out ImageSymbol sym))
                    {
                        return CreateReferenceToImport(sym);
                    }
                }
            }

            foreach (var program in project.Programs)
            {
                if (program.EnvironmentMetadata.Globals.TryGetValue(name, out var dt))
                {
                    return Identifier.Global(name, dt);
                }
            }
            return platform.ResolveImportByName(moduleName, name);
        }

        public Expression ResolveImport(string moduleName, int ordinal, IPlatform platform)
        {
            foreach (var program in project.Programs)
            {
                if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out var  mod))
                    continue;

                if (mod.ServicesByOrdinal.TryGetValue(ordinal, out var svc))
                {
                    EnsureSignature(program, svc);
                    var ep = new ExternalProcedure(svc.Name, svc.Signature, svc.Characteristics);
                    return new ProcedureConstant(platform.PointerType, ep);
                }

                if (mod.GlobalsByOrdinal.TryGetValue(ordinal, out var sym))
                {
                    return CreateReferenceToImport(sym);
                }
            }
            return platform.ResolveImportByOrdinal(moduleName, ordinal);
        }

        private Expression CreateReferenceToImport(ImageSymbol sym)
        {
            if (sym.Type == SymbolType.ExternalProcedure)
            {
                throw new NotImplementedException();
            }
            else if (sym.Type == SymbolType.Procedure)
            {
                throw new NotImplementedException();
            }
            else
            {
                var id = Identifier.Global(sym.Name, sym.DataType);
                return new UnaryExpression(Operator.AddrOf, program.Platform.PointerType, id);
            }
        }

        public Expression ResolveToImportedValue(Statement stm, Constant c)
        {
            var addrInstruction = program.SegmentMap.MapLinearAddressToAddress(stm.LinearAddress);
            var addrImportThunk = program.Platform.MakeAddressFromConstant(c, true);
            if (!program.ImportReferences.TryGetValue(addrImportThunk, out var impref))
                return null;

            if (impref.SymbolType == SymbolType.Procedure)
            {
                if (!this.localProcs.TryGetValue(impref.EntryName, out var sym))
                    return null;
                if (! program.Procedures.TryGetValue(sym.Address, out var proc))
                    return null;
                return new ProcedureConstant(program.Platform.PointerType, proc);
            }
            else if (impref.SymbolType == SymbolType.Data)
            {
                // Read an address sized value at the given address.
                if (!program.SegmentMap.TryFindSegment(impref.ReferenceAddress, out ImageSegment seg))
                    return null;
                var dt = PrimitiveType.CreateWord(impref.ReferenceAddress.DataType.BitSize);
                if (!program.Architecture.TryRead(seg.MemoryArea, impref.ReferenceAddress, dt, out Constant cIndirect))
                    return Constant.Invalid;
                return Constant.Create(program.Architecture.WordWidth, cIndirect.ToInt64());
            }
            else
            {
                //$TODO: need to work on imported data symbols
                // for now, treat them as imported procedures.

                var extProc = impref.ResolveImportedProcedure(
                    this,
                    program.Platform,
                    new AddressContext(program, addrInstruction, this.eventListener));
                return new ProcedureConstant(program.Platform.PointerType, extProc);
            }
        }
    }
}
