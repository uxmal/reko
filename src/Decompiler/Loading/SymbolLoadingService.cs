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

using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Services;
using System;
using System.Collections.Generic;

namespace Reko.Loading
{
    /// <summary>
    /// Standard implementation of <see cref="ISymbolLoadingService"/>.
    /// </summary>
    public class SymbolLoadingService : ISymbolLoadingService
    {
        private readonly IServiceProvider services;

        /// <summary>
        /// Constructs an instance of the <see cref="SymbolLoadingService"/> class.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance.</param>
        public SymbolLoadingService(IServiceProvider services)
        {
            this.services = services;
        }

        /// <inheritdoc/>
        public ISymbolSource? GetSymbolSource(string filename)
        {
            var cfgSvc = services.RequireService<IConfigurationService>();
            var fsSvc = services.RequireService<IFileSystemService>();
            var bytes = fsSvc.ReadAllBytes(filename);
            foreach (var symSrcDef in cfgSvc.GetSymbolSources())
            {
                var symSrc = LoadSymbolSource(symSrcDef, bytes, filename);
                if (symSrc is not null)
                    return symSrc;
            }
            return null;
        }

        public ISymbolSource? LoadSymbolSource(SymbolSourceDefinition symSrcDef, byte [] bytes, string filename)
        {
            if (symSrcDef.TypeName is null)
                return null;
            var svc = services.RequireService<IPluginLoaderService>();
            //$TODO: fail softly here.
            Type type;
            try
            {
                type = svc.GetType(symSrcDef.TypeName);
            }
            catch
            {
                var eventListener = services.RequireService<IEventListener>();
                eventListener.Error("Symbol source {0} in the Reko configuration failed to load.", symSrcDef.Name!);
                return null;
            }
            var symSrc = (ISymbolSource)Activator.CreateInstance(type)!;
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
