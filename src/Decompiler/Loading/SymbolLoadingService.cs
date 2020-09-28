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

using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Configuration;

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
                var symSrc = LoadSymbolSource(symSrcDef, bytes, filename);
                if (symSrc != null)
                    return symSrc;
            }
            return null;
        }

        public ISymbolSource LoadSymbolSource(SymbolSourceDefinition symSrcDef, byte [] bytes, string filename)
        {
            var type = Type.GetType(symSrcDef.TypeName, false);
            if (type == null)
            {
                var eventListener = services.RequireService<DecompilerEventListener>();
                eventListener.Error(new NullCodeLocation(""), "Symbol source {0} in the Reko configuration failed to load.", symSrcDef.Name);
                return null;
            }
            var symSrc = (ISymbolSource)Activator.CreateInstance(type);
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
