#region License
/* 
 * Copyright (C) 2020 Natalia Portillo <claunia@claunia.com>.
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
using Reko.Core;
using Reko.Core.Services;

namespace Reko.ImageLoaders.MzExe
{
    public class LeImageLoader : ImageLoader
    {
        private readonly SortedList<Address, ImageSymbol> imageSymbols;
        private readonly Dictionary<uint, Tuple<Address, ImportReference>> importStubs;
        private readonly IDiagnosticsService diags;
        private readonly uint lfaNew;

        public LeImageLoader(IServiceProvider services, string filename, byte[] imgRaw, uint e_lfanew) : base(services, filename, imgRaw)
        {
            diags = Services.RequireService<IDiagnosticsService>();
            lfaNew = e_lfanew;
            importStubs = new Dictionary<uint, Tuple<Address, ImportReference>>();
            imageSymbols = new SortedList<Address, ImageSymbol>();
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }
    }
}