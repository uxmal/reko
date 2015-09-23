#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Commands
{
    public class Cmd_ViewWhatPointsHere : Command
    {
        private Program program;
        private IEnumerable<Address> addresses;

        public Cmd_ViewWhatPointsHere(IServiceProvider services, Program program, IEnumerable<Address> addresses)
           : base(services)
        {
            this.program = program;
            this.addresses = addresses;
        }

        public override void DoIt()
        {
            var resultSvc = Services.RequireService<ISearchResultService>();
            var arch = program.Architecture;
            var image = program.Image;
            var rdr = program.Architecture.CreateImageReader(program.Image, 0);
            var addrControl = program.Platform.CreatePointerScanner(
                program.ImageMap,
                rdr,
                addresses,
                PointerScannerFlags.All);
            resultSvc.ShowSearchResults(
                new AddressSearchResult(
                    Services, 
                    addrControl.Select(lin => new ProgramAddress(program, lin)),
                    AddressSearchDetails.Code));
        }
    }
}
