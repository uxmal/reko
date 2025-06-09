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

using System;
using System.Collections.Generic;
using System.Linq;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Core.Operators;
using Reko.Core.Loading;

namespace Reko.Core
{
    /// <summary>
    /// The dynamic linker tries to resolve a reference to external code or
    /// data by consulting the current project first hand, and the platform
    /// in second hand. Doing it that way allows users to override platform
    /// definitions as the need arises.
    /// </summary>
    public interface IDynamicLinker
    {
        /// <summary>
        /// Find an external procedure, based on its name. The optional
        /// <paramref name="moduleName"/> allows specifying a particular
        /// code module hosting the external procedure.
        /// </summary>
        /// <param name="moduleName">
        /// Optional name of the module hosting the external procedure.
        /// </param>
        /// <param name="importName">
        /// The name of the external procedure.
        /// </param>
        /// <param name="platform">
        /// The operating environment hosting the executable program.
        /// </param>
        /// <returns>
        /// If an external procedure matching <paramref name="importName"/>
        /// and <paramref name="moduleName"/> can be located, it is returned,
        /// otherwise null.
        /// </returns>
        ExternalProcedure? ResolveProcedure(string? moduleName, string importName, IPlatform platform);

        /// <summary>
        /// Find an external procedure, based on a module name and an ordinal. The optional
        /// <paramref name="moduleName"/> allows specifying a particular
        /// code module hosting the external procedure.
        /// </summary>
        /// <param name="moduleName">
        /// Optional name of the module hosting the externa procedure.
        /// </param>
        /// <param name="ordinal">
        /// The name of the external procedure.
        /// </param>
        /// <param name="platform">
        /// The operating environment hosting the executable program.
        /// </param>
        /// <returns>
        /// If an external procedure matching <paramref name="ordinal"/>
        /// and <paramref name="moduleName"/> can be located, it is returned,
        /// otherwise null.
        /// </returns>
        ExternalProcedure? ResolveProcedure(string moduleName, int ordinal, IPlatform platform);


        /// <summary>
        /// Resolves an imported global variable by name, and optionally by <paramref name="moduleName"/>
        /// </summary>
        /// <param name="moduleName">Optional name of module in which a global variable is defined.
        /// </param>
        /// <param name="globalName">The name of a global variable.
        /// </param>
        /// <param name="platform">
        /// The operating environment hosting the executable program.
        /// </param>
        /// <returns>
        /// If a global variable matching the <paramref name="globalName"/> and optionally 
        /// the <paramref name="moduleName"/>, returns an expression referring to the 
        /// global variable, otherwise null.
        /// </returns>
        Expression? ResolveImport(string? moduleName, string globalName, IPlatform platform);

        /// <summary>
        /// Resolves an imported global variable by the given <paramref name="moduleName"/> and 
        /// the <paramref name="ordinal"/>.
        /// </summary>
        /// <param name="moduleName">Name of module in which a global variable is defined.
        /// </param>
        /// <param name="ordinal">The ordinal of a global variable.
        /// </param>
        /// <param name="platform">
        /// The operating environment hosting the executable program.
        /// </param>
        /// <returns>
        /// If a global variable matching the <paramref name="ordinal"/> and 
        /// the <paramref name="moduleName"/>, returns an expression referring to the 
        /// global variable, otherwise null.
        /// </returns>

        Expression? ResolveImport(string moduleName, int ordinal, IPlatform platform);

        /// <summary>
        /// $TODO: write me!
        /// </summary>
        /// <param name="stm"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Expression? ResolveToImportedValue(Statement stm, Constant c);
    }

    /// <summary>
    /// The <see cref="DynamicLinker"/> class implements the <see cref="IDynamicLinker"/>.
    /// </summary>
    public class DynamicLinker : IDynamicLinker
    {
        private readonly Project project;
        private readonly Program program;
        private readonly IEventListener eventListener;
        private readonly Dictionary<string, ImageSymbol> localProcs;

        /// <summary>
        /// Creates a new instance of the <see cref="DynamicLinker"/> class.
        /// </summary>
        /// <param name="project">Project in which the program is hosted.</param>
        /// <param name="program">Current program.</param>
        /// <param name="eventListener"><see cref="IEventListener" /> interface to report errors to.
        /// </param>
        public DynamicLinker(Project project, Program program, IEventListener eventListener)
        {
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            this.program = program;
            this.eventListener = eventListener;
            this.localProcs = program.ImageSymbols.Values
                .Where(sym => sym.Name is not null && sym.Type == SymbolType.Procedure)
                .GroupBy(sym => sym.Name)
                .ToDictionary(g => g.Key!, g => g.First());
        }

        private FunctionType EnsureSignature(Program program, SystemService svc)
        {
            // If there is a signature in 'Signatures', it should override the signature in the service.
            if (program.EnvironmentMetadata.Signatures.TryGetValue(svc.Name!, out var fnc))
            {
                return fnc;
            }
            return svc.Signature ?? new FunctionType();
        }

        /// <inheritdoc/>
        public ExternalProcedure? ResolveProcedure(string? moduleName, string importName, IPlatform platform)
        {
            var ep = LookupProcedure(moduleName, importName, platform);
            if (ep is { })
                return ep;
            // Can we guess at the signature?
            var sProc = platform.SignatureFromName(importName);
            if (sProc is { })
            {
                var loader = program.CreateTypeLibraryDeserializer();
                var chr = program.LookupCharacteristicsByName(importName);
                ep = loader.LoadExternalProcedure(sProc, chr);
                if (ep is null)
                    return null;
                if (!ep.Signature.ParametersValid)
                {
                    // We found a imported procedure but couldn't find its signature.
                    // Perhaps it has been mangled, and we can use the stripped name.
                    var epNew = LookupProcedure(null, ep.Name, platform);
                    if (epNew is { })
                    {
                        ep = epNew;
                    }
                }
                else if (sProc.Signature is not null)
                {
                    program.EnsureExternalProcedure(
                        moduleName, importName, sProc.Signature.Convention,
                        ep);
                }
                return ep;
            }
            return null;
        }

        private ExternalProcedure? LookupProcedure(string? moduleName, string importName, IPlatform platform)
        {
            if (!string.IsNullOrEmpty(moduleName))
            {
                foreach (var program in project.Programs)
                {
                    if (program.EnvironmentMetadata.Modules.TryGetValue(moduleName!, out var mod) &&
                        mod.ServicesByName.TryGetValue(importName, out var svc))
                    {
                        return new ExternalProcedure(svc.Name!, svc.Signature!, svc.Characteristics);
                    }
                }
                if (project.LoadedMetadata.Modules.TryGetValue(moduleName!, out var module) &&
                    module.ServicesByName.TryGetValue(importName, out var service))
                {
                    return new ExternalProcedure(service.Name!, service.Signature!, service.Characteristics);
                }
            }

            foreach (var program in project.Programs)
            {
                if (program.EnvironmentMetadata.Signatures.TryGetValue(importName, out var sig))
                {
                    var chr = program.LookupCharacteristicsByName(importName);
                    return new ExternalProcedure(importName, sig, chr);
                }
            }
            if (project.LoadedMetadata.Signatures.TryGetValue(importName, out var signature))
            {
                return new ExternalProcedure(importName, signature);
            }
            return platform.LookupProcedureByName(moduleName, importName);
        }

        /// <inheritdoc/>
        public ExternalProcedure? ResolveProcedure(string moduleName, int ordinal, IPlatform platform)
        {
            foreach (var program in project.Programs)
            {
                if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out var mod))
                    continue;

                if (mod.ServicesByOrdinal.TryGetValue(ordinal, out var svc))
                {
                    var signature = EnsureSignature(program, svc);
                    if (signature is not null)
                    {
                        return new ExternalProcedure(svc.Name!, signature, svc.Characteristics);
                    }
                    else
                    {
                        // We have a name for the external procedure, but can't find a proper signature for it. 
                        // So we make a "dumb" one. It's better than nothing.
                        eventListener.Warn(
                            new NullCodeLocation(moduleName),
                            "Unable to find a signature for {0}", svc.Name!);
                        return new ExternalProcedure(svc.Name!, new FunctionType());
                    }
                }
            }
            if (project.LoadedMetadata.Modules.TryGetValue(moduleName, out var module) &&
                module.ServicesByOrdinal.TryGetValue(ordinal, out var service))
            {
                var signature = EnsureSignature(program, service);
                if (signature is not null)
                {
                    return new ExternalProcedure(service.Name!, signature, service.Characteristics);
                }
                else
                {
                    // We have a name for the external procedure, but can't find a proper signature for it. 
                    // So we make a "dumb" one. It's better than nothing.
                    eventListener.Warn(
                        new NullCodeLocation(moduleName),
                        "Unable to find a signature for {0}", service.Name!);
                    return new ExternalProcedure(service.Name!, new FunctionType());
                }
            }
            return platform.LookupProcedureByOrdinal(moduleName, ordinal);
        }

        /// <inheritdoc/>
        public Expression? ResolveImport(string? moduleName, string name, IPlatform platform)
        {
            var global = LookupImport(moduleName, name, platform);
            if (global is not null)
                return global;
            var t = platform.DataTypeFromImportName(name);
            //$REVIEW: the way imported symbols are resolved as 
            // globals or functions needs a revisit.
            if (t is not null && t.Value.Item2 is not SerializedSignature)
            {
                var dSer = program.CreateTypeLibraryDeserializer();
                var dt = (t.Value.Item2 is null) ?
                    new UnknownType() :
                    t.Value.Item2.Accept(dSer);
                return Identifier.Global(t.Value.Item1, dt);
            }
            else
                return null;
        }

        private Expression? LookupImport(string? moduleName, string name, IPlatform platform)
        {
            if (!string.IsNullOrEmpty(moduleName))
            {
                foreach (var program in project.Programs)
                {
                    // Try to find the imported procedure based on module + name.
                    if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName!, out ModuleDescriptor? mod))
                        continue;

                    if (mod.ServicesByName.TryGetValue(name, out SystemService? svc))
                    {
                        var ep = new ExternalProcedure(svc.Name!, svc.Signature!, svc.Characteristics);
                        return new ProcedureConstant(platform.PointerType, ep);
                    }

                    if (mod.GlobalsByName.TryGetValue(name, out ImageSymbol? sym))
                    {
                        return CreateReferenceToImport(sym);
                    }
                }
            }

            foreach (var program in project.Programs)
            {
                if (program.EnvironmentMetadata.ImportedGlobals.TryGetValue(name, out var dt))
                {
                    return Identifier.Global(name, dt);
                }
            }
            return platform.ResolveImportByName(moduleName, name);
        }

        /// <inheritdoc/>
        public SystemService? ResolveService(string moduleName, int ordinal)
        {
            foreach (var program in project.Programs)
            {
                if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out var mod))
                    continue;

                if (mod.ServicesByOrdinal.TryGetValue(ordinal, out var svc))
                {
                    return svc;
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public Expression? ResolveImport(string moduleName, int ordinal, IPlatform platform)
        {
            foreach (var program in project.Programs)
            {
                if (!program.EnvironmentMetadata.Modules.TryGetValue(moduleName, out var  mod))
                    continue;

                if (mod.ServicesByOrdinal.TryGetValue(ordinal, out var svc))
                {
                    var signature = EnsureSignature(program, svc);
                    var ep = new ExternalProcedure(svc.Name!, signature!, svc.Characteristics);
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
                var id = Identifier.Global(sym.Name!, sym.DataType!);
                return new UnaryExpression(Operator.AddrOf, program.Platform.PointerType, id);
            }
        }

        /// <inheritdoc/>
        public Expression? ResolveToImportedValue(Statement stm, Constant c)
        {
            var addrInstruction = stm.Address;
            var addrImportThunk = program.Platform.MakeAddressFromConstant(c, true);
            if (addrImportThunk is null)
                return null;
            if (!program.ImportReferences.TryGetValue(addrImportThunk.Value, out var impref))
                return null;

            if (impref.SymbolType == SymbolType.Procedure)
            {
                if (!this.localProcs.TryGetValue(impref.EntryName, out var sym))
                    return null;
                if (! program.Procedures.TryGetValue(sym.Address!, out var proc))
                    return null;
                return new ProcedureConstant(program.Platform.PointerType, proc);
            }
            else if (impref.SymbolType == SymbolType.Data)
            {
                // Read an address sized value at the given address.
                var dt = PrimitiveType.CreateWord(impref.ReferenceAddress.DataType.BitSize);
                if (!program.Architecture.TryRead(program.Memory, impref.ReferenceAddress, dt, out Constant? cIndirect))
                    return InvalidConstant.Create(program.Architecture.WordWidth);
                return Constant.Create(program.Platform.PointerType, cIndirect.ToInt64());
            }
            else
            {
                //$TODO: need to work on imported data symbols
                // for now, treat them as imported procedures.

                var extProc = impref.ResolveImportedProcedure(
                    this,
                    program.Platform,
                    new ProgramAddress(program, addrInstruction),
                    this.eventListener);
                return new ProcedureConstant(program.Platform.PointerType, extProc);
            }
        }
    }
}
