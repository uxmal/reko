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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Gui
{
    /// <summary>
    /// An address search result is the ... well, result of a search of raw memory. Navigable items take you
    /// to the current memory view (if one is topmost) or opens a new memory view.
    /// </summary>
    public class AddressSearchResult : ISearchResult
    {
        private IServiceProvider services;
        private List<AddressSearchHit> addresses;

        public AddressSearchResult(IServiceProvider services, IEnumerable<AddressSearchHit> linearAddresses)
        {
            this.services = services;
            this.addresses = linearAddresses.ToList();
        }

        public int Count
        {
            get { return addresses.Count; }
        }

        public int ContextMenuID
        {
            get { return 0; }
        }

        public void CreateColumns(ISearchResultView view)
        {
            view.AddColumn("Program", 30);
            view.AddColumn("Address", 10);
            view.AddColumn("Data", 70);
        }

        public int GetItemImageIndex(int i)
        {
            return 0;
        }

        public SearchResultItem GetItem(int i)
        {
            var hit = addresses[i];
            var program = hit.Program;
            var addr = program.ImageMap.MapLinearAddressToAddress(addresses[i].LinearAddress);
            if (program.Architecture == null)
            {
                return new SearchResultItem
                {
                    Items = new string[] { 
                    "",
                    addr.ToString(),
                    ""
                },
                    ImageIndex = 0,
                    BackgroundColor = -1,
                };
            }
            var dasm = program.Architecture.CreateDisassembler(program.Architecture.CreateImageReader(program.Image, addr));
            try
            {
                var instr = string.Join("; ", dasm.Take(4).Select(inst => inst.ToString().Replace('\t', ' ')));
                return new SearchResultItem
                {
                    Items = new string[] {
                    program.Name ?? "<Program>",
                    addr.ToString(),
                    instr,
                    },
                    ImageIndex = 0,
                    BackgroundColor = -1,
                };
            }
            catch
            {
                return new SearchResultItem
                {
                    Items = new string[] {
                    addr.ToString(),
                    "<invalid>"
                    },
                    ImageIndex = 0,
                    BackgroundColor = -1,
                };
            }
        }

        public void NavigateTo(int i)
        {
            var memSvc = services.RequireService<ILowLevelViewService>();
            var hit = addresses[i];
            memSvc.ShowMemoryAtAddress(hit.Program, hit.Program.ImageMap.MapLinearAddressToAddress(hit.LinearAddress));
        }
    }

    public class AddressSearchHit
    {
        public Program Program;
        public uint LinearAddress;

        public AddressSearchHit()
        {
        }

        public AddressSearchHit(Program program, uint linearAddress)
        {
            this.Program = program;
            this.LinearAddress = linearAddress;
        }
    }
}
