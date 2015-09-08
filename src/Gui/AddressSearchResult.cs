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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// An address search result is the ... well, result of a search of raw memory. Navigable items take you
    /// to the current memory view (if one is topmost) or opens a new memory view.
    /// </summary>
    public class AddressSearchResult : ISearchResult
    {
        protected readonly IServiceProvider services;
        private List<ProgramAddress> addresses;

        public AddressSearchResult(
            IServiceProvider services,
            IEnumerable<ProgramAddress> addresses)
        {
            this.services = services;
            this.addresses = addresses.ToList();
        }

        public ISearchResultView View { get; set; }

        public int Count
        {
            get { return addresses.Count; }
        }

        public int ContextMenuID
        {
            get { return MenuIds.CtxAddressSearch; }
        }

        public int SortedColumn { get { return -1; } }

        public bool IsColumnSortable(int iColumn)
        {
            return false;
        }

        public SortDirection GetSortDirection(int iColumn)
        {
            return SortDirection.None;
        }

        public void CreateColumns()
        {
            View.AddColumn("Program", 30);
            View.AddColumn("Address", 10);
            View.AddColumn("Type", 10);
            View.AddColumn("Data", 70);
        }

        public int GetItemImageIndex(int i)
        {
            return 0;
        }

        public SearchResultItem GetItem(int i)
        {
            var hit = addresses[i];
            var program = hit.Program;
            var addr = addresses[i].Address;
            ImageMapItem item;
            var type = program.ImageMap.TryFindItem(addr, out item);
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
            int bgColor = SelectBgColor(item);
            var dasm = program.CreateDisassembler(addr);
            try
            {
                var instr = string.Join("; ", dasm.Take(4).Select(inst => inst.ToString().Replace('\t', ' ')));
                return new SearchResultItem
                {
                    Items = new string[] {
                        program.Name ?? "<Program>",
                        addr.ToString(),
                        item.DataType.ToString(),
                        instr,
                    },
                    ImageIndex = 0,
                    BackgroundColor = bgColor,
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

        private int SelectBgColor(ImageMapItem item)
        {
            if (item.DataType is UnknownType)
                return -1;
            if (item.DataType is CodeType)  //$TODO: colors should come from settings.
                return System.Drawing.Color.Pink.ToArgb();
            throw new NotImplementedException();
        }

        public void SortByColumn(int iColumn, SortDirection dir)
        {
        }

        public void NavigateTo(int i)
        {
            var memSvc = services.RequireService<ILowLevelViewService>();
            var hit = addresses[i];
            memSvc.ShowMemoryAtAddress(hit.Program, hit.Address);
        }

        public virtual bool QueryStatus(CommandID cmdID, CommandStatus status, CommandText txt)
        {
            if (cmdID.Guid == CmdSets.GuidReko)
            {
                switch (cmdID.ID)
                {
                case CmdIds.ActionMarkProcedure:
                case CmdIds.ViewFindWhatPointsHere:
                    status.Status = MenuStatus.Enabled|MenuStatus.Visible;
                    return true;
                }
            }
            return false;
        }

        public virtual bool Execute(CommandID cmdID)
        {
            if (!View.IsFocused)
                return false;
            switch (cmdID.ID)
            {
            case CmdIds.ActionMarkProcedure: MarkProcedures(); return true;
            case CmdIds.ViewFindWhatPointsHere: ViewFindWhatPointsHere(); return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the addresses the user has selected.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProgramAddress> SelectedHits()
        {
            return View.SelectedIndices.Select(i => addresses[i]);
        }

        public void MarkProcedures()
        {
            services.RequireService<ICommandFactory>().MarkProcedures(SelectedHits()).Do();
        }

        public void ViewFindWhatPointsHere()
        {
            var cmdFactory = services.RequireService<ICommandFactory>();
            var grs =
                from hit in SelectedHits()
                group hit by hit.Program into g
                select new { Program = g.Key, Addresses = g.Select(gg => gg.Address) };
            foreach (var gr in grs)
            {
                cmdFactory.ViewWhatPointsHere(gr.Program, gr.Addresses).Do();
            }
        }
    }
}
