#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

namespace Decompiler.Core
{
    public abstract class ImportReference
    {
        public Address ReferenceAddress;
        public string ModuleName;

        public ImportReference(Address addr, string moduleName)
        {
            this.ReferenceAddress = addr;
            this.ModuleName = moduleName;
        }

        public abstract ExternalProcedure ResolveImportedProcedure(IImportResolver resolver, Platform platform);
    }

    public class NamedImportReference : ImportReference
    {
        public string ImportName;

        public NamedImportReference(Address addr, string moduleName, string importName)
            : base(addr, moduleName)
        {
            this.ImportName = importName;
        }

        public override ExternalProcedure ResolveImportedProcedure(IImportResolver resolver, Platform platform)
        {
            var ep = resolver.ResolveProcedure(ModuleName, ImportName, platform);
            if (ep != null)
                return ep;
            // Can we guess at the signature?
            var sig = platform.SignatureFromName(ImportName);
            if (sig != null)
            {
                ep = new ExternalProcedure(ImportName, sig);   //$BUGBUG: mangled name!
            }
            return ep;
        }

        public override string ToString()
        {
            return string.Format("{0}!{1}", ModuleName, ImportName);
        }
    }

    public class OrdinalImportReference : ImportReference
    {
        public int Ordinal;

        public OrdinalImportReference(Address addr, string moduleName, int ordinal)
            : base(addr, moduleName)
        {
            this.Ordinal = ordinal;
        }

        public override ExternalProcedure ResolveImportedProcedure(IImportResolver resolver, Platform platform)
        {
            return resolver.ResolveProcedure(ModuleName, Ordinal, platform);
        }

        public override string ToString()
        {
            return string.Format("{0}!Ordinal_{1}", ModuleName, Ordinal);
        }
    }
}
