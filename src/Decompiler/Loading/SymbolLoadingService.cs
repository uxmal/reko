#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Configuration;
using System.Reflection;

namespace Reko.Loading
{
    public class SymbolLoadingService : ISymbolLoadingService
    {
        private IServiceProvider services;

        public SymbolLoadingService(IServiceProvider services)
        {
            this.services = services;
        }

        public ISymbolSource GetSymbolSource(string filename)
        {
            var cfgSvc = services.RequireService<IConfigurationService>();
            var fsSvc = services.RequireService<IFileSystemService>();
            var bytes = fsSvc.ReadAllBytes(filename);
            foreach (var symSrcDef in cfgSvc.GetSymbolSources())
            {
                var type = Type.GetType(symSrcDef.TypeName, false);
                if (type == null)
                {
                    var eventListener = services.RequireService<DecompilerEventListener>();
                    eventListener.Error(new NullCodeLocation(""), "Symbol source {0} in the Reko configuration failed to load.", symSrcDef.Name);
                    return null;
                }
                var symSrc = LoadSymbolSource(type, bytes, filename);
                if (symSrc != null)
                    return symSrc;
            }
            return null;
        }

        public ISymbolSource GetSymbolSource(SymbolSourceReference ssRef)
        {
            var cfgSvc = services.RequireService<IConfigurationService>();
            var fsSvc = services.RequireService<IFileSystemService>();
            var eventListener = services.RequireService<DecompilerEventListener>();
            Type type = null;
            if (!string.IsNullOrEmpty(ssRef.Name))
            {
                var def = cfgSvc.GetSymbolSource(ssRef.Name);
                type = Type.GetType(def.TypeName);
                if (type == null)
                {
                    eventListener.Error(new NullCodeLocation(def.Name),
                        "Symbol source from the Reko configuration failed to load.");
                    return null;
                }
            }
            else if (!string.IsNullOrEmpty(ssRef.AssemblyName) && !string.IsNullOrEmpty(ssRef.TypeName))
            {
                var ass = Assembly.LoadFrom(ssRef.AssemblyName);
                if (ass == null)
                {
                    eventListener.Error(new NullCodeLocation(ssRef.AssemblyName),
                        "Unable to load assembly file.");
                    return null;
                }
                type = ass.GetType(ssRef.TypeName);
                if (type == null)
                {
                    eventListener.Error(new NullCodeLocation(ssRef.AssemblyName),
                        "Unable to load type {0}.", ssRef.TypeName);
                    return null;
                }
            }
            var bytes = fsSvc.ReadAllBytes(ssRef.SymbolSourceUrl);
            return LoadSymbolSource(type, bytes, ssRef.SymbolSourceUrl);
        }

        public ISymbolSource LoadSymbolSource(Type type, byte [] bytes, string filename)
        {
            if (type == null)
                return null;
            var symSrc = (ISymbolSource)Activator.CreateInstance(type, filename);
            if (symSrc.CanLoad(filename, bytes))
            {
                return symSrc;
            }
            return null;
        }

        public List<SymbolSourceDefinition> GetSymbolSources()
        {
            throw new NotImplementedException();
        }
    }
}
