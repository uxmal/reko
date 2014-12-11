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
    public interface IImportResolver
    {
        ExternalProcedure ResolveProcedure(string moduleName, string importName, Platform platform);
        ExternalProcedure ResolveProcedure(string moduleName, int ordinal, Platform platform);
    }

    /// <summary>
    /// An import resolver tries to resolve a reference to external code or data  by consulting the 
    /// current project first hand, and the platform in second hand. Doing it that way allows users to
    /// override platform definitions as the need arises.
    /// </summary>
    public class ImportResolver : IImportResolver
    {
        private Project project;

        public ImportResolver(Project project)
        {
            this.project = project;
        }

        public ExternalProcedure ResolveProcedure(string moduleName, string importName, Platform platform)
        {
            foreach (var module in project.MetadataFiles.Where(m => m.LibraryName == moduleName && m.TypeLibrary != null))
            {
                SystemService svc;
                if (module.TypeLibrary.ServicesByName.TryGetValue(importName, out svc))
                {
                    return new ExternalProcedure(svc.Name, svc.Signature, svc.Characteristics);
                }
            }
            var sig = platform.LookupProcedureByName(moduleName, importName);
            if (sig == null)
                return null;
            return new ExternalProcedure(importName, sig);
        }

        public ExternalProcedure ResolveProcedure(string moduleName, int ordinal, Platform platform)
        {
            foreach (var module in project.MetadataFiles.Where(m => m.LibraryName == moduleName && m.TypeLibrary != null))
            {
                SystemService svc;
                if (module.TypeLibrary.ServicesByVector.TryGetValue(ordinal, out svc))
                {
                    return new ExternalProcedure(svc.Name, svc.Signature, svc.Characteristics);
                }
            }
            var sig = platform.LookupProcedureByOrdinal(moduleName, ordinal);
            if (sig == null)
                return null;
            return new ExternalProcedure("Ordinal_" + ordinal, sig);
        }
    }
}
