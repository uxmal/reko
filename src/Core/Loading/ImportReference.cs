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

using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.ComponentModel.Design;

namespace Reko.Core.Loading
{
    /// <summary>
    /// Represents a reference to an external symbol from another module.
    /// </summary>
    public abstract class ImportReference : IComparable<ImportReference>
    {
        /// <summary>
        /// Initializes the field of this instance.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="moduleName"></param>
        /// <param name="entryName"></param>
        /// <param name="symType"></param>
        public ImportReference(Address addr, string? moduleName, string entryName, SymbolType symType)
        {
            ReferenceAddress = addr;
            ModuleName = moduleName;
            EntryName = entryName;
            SymbolType = symType;
        }

        public Address ReferenceAddress { get; }
        public string? ModuleName { get; }
        public string EntryName { get; }


        public SymbolType SymbolType { get; }

        public abstract Expression? ResolveImport(IDynamicLinker dynamicLinker, IPlatform platform, ProgramAddress addr, IEventListener listener);

        public abstract ExternalProcedure ResolveImportedProcedure(IDynamicLinker dynamicLinker, IPlatform platform, ProgramAddress addr, IEventListener listener);

        public abstract int CompareTo(ImportReference? that);

        protected int CompareModuleNames(ImportReference that)
        {
            if (ModuleName != null && that.ModuleName != null)
            {
                return ModuleName.CompareTo(that.ModuleName);
            }
            else if (ModuleName != null)
                return 1;
            else if (that.ModuleName != null)
                return -1;
            else
                return 0;
        }
    }

    public class NamedImportReference : ImportReference
    {
        public NamedImportReference(Address addr, string? moduleName, string importName, SymbolType symType)
            : base(addr, moduleName, importName, symType)
        {
            ImportName = importName;
        }

        public string ImportName { get; }

        public override int CompareTo(ImportReference? that)
        {
            if (that is null)
                return 1;
            if (GetType() != that.GetType())
            {
                return GetType().FullName!.CompareTo(GetType().FullName);
            }
            System.Diagnostics.Debugger.Break();
            int cmp = CompareModuleNames(that);
            if (cmp != 0)
                return cmp;
            return string.Compare(
                ImportName,
                ((NamedImportReference) that).ImportName,
                StringComparison.InvariantCulture);
        }

        public override Expression? ResolveImport(
            IDynamicLinker resolver,
            IPlatform platform,
            ProgramAddress addr,
            IEventListener listener)
        {
            return resolver.ResolveImport(ModuleName, ImportName, platform);
        }

        public override ExternalProcedure ResolveImportedProcedure(
            IDynamicLinker resolver,
            IPlatform platform,
            ProgramAddress addr,
            IEventListener listener)
        {
            var ep = resolver.ResolveProcedure(ModuleName, ImportName, platform);
            if (ep is not null)
            {
                if (!ep.Signature.ParametersValid)
                {
                    listener.Warn(addr, $"Unable to guess parameters of {this}.");
                }
                return ep;
            }
            listener.Warn(addr, $"Unable to resolve imported reference {this}.");
            return new ExternalProcedure(ToString(), new FunctionType());
        }

        /// <summary>
        /// Returns a string representation of this import reference.
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                string.IsNullOrEmpty(ModuleName) ? "{1}" : "{0}!{1}",
                ModuleName,
                ImportName);
        }
    }

    /// <summary>
    /// Windows likes to use imports by ordinal number, especially Win16.
    /// </summary>
    public class OrdinalImportReference : ImportReference
    {
        public int Ordinal { get; }

        public OrdinalImportReference(Address addr, string moduleName, int ordinal, SymbolType symType)
            : base(addr, moduleName, MakeEntryName(moduleName, ordinal), symType)
        {
            Ordinal = ordinal;
        }

        private static string MakeEntryName(string moduleName, int ordinal)
        {
            return string.Format("{0}_{1}", moduleName, ordinal);
        }

        public override int CompareTo(ImportReference? that)
        {
            if (that is null)
                return 1;
            if (GetType() != that.GetType())
            {
                return GetType().FullName!.CompareTo(GetType().FullName);
            }
            int cmp = CompareModuleNames(that);
            if (cmp != 0)
                return cmp;
            cmp = Ordinal.CompareTo(((OrdinalImportReference) that).Ordinal);
            return cmp;
        }

        public override Expression? ResolveImport(
            IDynamicLinker dynamicLinker, 
            IPlatform platform, 
            ProgramAddress addr,
            IEventListener listener)
        {
            var imp = dynamicLinker.ResolveImport(ModuleName!, Ordinal, platform);
            if (imp != null)
                return imp;
            listener.Warn(addr, $"Unable to resolve imported reference {this}.");
            return null;
        }

        public override ExternalProcedure ResolveImportedProcedure(
            IDynamicLinker resolver,
            IPlatform platform,
            ProgramAddress addr,
            IEventListener listener)
        {
            var ep = resolver.ResolveProcedure(ModuleName!, Ordinal, platform);
            if (ep is not null)
                return ep;
            listener.Warn(addr, $"Unable to resolve imported reference {this}.");
            return new ExternalProcedure(ToString(), new FunctionType());
        }

        /// <summary>
        /// Returns a string representation of this import reference.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}!Ordinal_{1}", ModuleName, Ordinal);
        }
    }
}
