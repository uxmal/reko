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

using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Represents a reference to an external symbol from another module.
    /// </summary>
    public abstract class ImportReference : IComparable<ImportReference>
    {
        public Address ReferenceAddress;
        public string ModuleName;
        public string EntryName;

        public ImportReference(Address addr, string moduleName, SymbolType symType)
        {
            this.ReferenceAddress = addr;
            this.ModuleName = moduleName;
            this.SymbolType = symType;
        }

        public SymbolType SymbolType { get; }

        public abstract Expression ResolveImport(IDynamicLinker dynamicLinker, IPlatform platform, AddressContext ctx);

        public abstract ExternalProcedure ResolveImportedProcedure(IDynamicLinker dynamicLinker, IPlatform platform, AddressContext ctx);

        public abstract int CompareTo(ImportReference that);
    }

    public class NamedImportReference : ImportReference
    {
        public string ImportName;

        public NamedImportReference(Address addr, string moduleName, string importName, SymbolType symType)
            : base(addr, moduleName, symType)
        {
            this.ImportName = importName;
            this.EntryName = importName;
        }

        public override int CompareTo(ImportReference that)
        {
            if (this.GetType() != that.GetType())
            {
                return this.GetType().FullName.CompareTo(this.GetType().FullName);
            }
            if (this.ModuleName == null)
            {
                if (that.ModuleName != null)
                    return -1;
            }
            else if (this.ModuleName != null)
            {
                if (that.ModuleName == null)
                    return 1;
            }
            else
            {
                int cmp = this.ModuleName.CompareTo(that.ModuleName);
                if (cmp != 0)
                    return cmp;
            }
            return string.Compare(
                this.ImportName,
                ((NamedImportReference)that).ImportName,
                StringComparison.InvariantCulture);
        }

        public override Expression ResolveImport(
            IDynamicLinker resolver,
            IPlatform platform,
            AddressContext ctx)
        {
            return resolver.ResolveImport(ModuleName, ImportName, platform);
        }

        public override ExternalProcedure ResolveImportedProcedure(
            IDynamicLinker resolver, 
            IPlatform platform, 
            AddressContext ctx)
        {
            var ep = resolver.ResolveProcedure(ModuleName, ImportName, platform);
            if (ep != null)
            {
                if (!ep.Signature.ParametersValid)
                {
                    ctx.Warn("Unable to guess parameters of {0}.", this);
                }
                return ep;
            }
            ctx.Warn("Unable to resolve imported reference {0}.", this);
            return new ExternalProcedure(this.ToString(), new FunctionType());
        }

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
        public int Ordinal;

        public OrdinalImportReference(Address addr, string moduleName, int ordinal, SymbolType symType)
            : base(addr, moduleName, symType)
        {
            this.Ordinal = ordinal;
            this.EntryName = string.Format("{0}_{1}", moduleName, ordinal);
        }

        public override int CompareTo(ImportReference that)
        {
            if (this.GetType() != that.GetType())
            {
                return this.GetType().FullName.CompareTo(this.GetType().FullName);
            }
            int cmp = this.ModuleName.CompareTo(that.ModuleName);
            if (cmp != 0)
                return cmp;
            cmp = this.Ordinal.CompareTo(((OrdinalImportReference)that).Ordinal);
            return cmp;
        }

        public override Expression ResolveImport(IDynamicLinker dynamicLinker, IPlatform platform, AddressContext ctx)
        {
            var imp = dynamicLinker.ResolveImport(ModuleName, Ordinal, platform);
            if (imp != null)
                return imp;
            ctx.Warn("Unable to resolve imported reference {0}.", this);
            return null;
        }

        public override ExternalProcedure ResolveImportedProcedure(IDynamicLinker resolver, IPlatform platform, AddressContext ctx)
        {
            var ep = resolver.ResolveProcedure(ModuleName, Ordinal, platform);
            if (ep != null)
                return ep;
            ctx.Warn("Unable to resolve imported reference {0}.", this);
            return new ExternalProcedure(this.ToString(), new FunctionType());
        }

        public override string ToString()
        {
            return string.Format("{0}!Ordinal_{1}", ModuleName, Ordinal);
        }
    }
}
