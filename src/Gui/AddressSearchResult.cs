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

using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// An address search result is the ... well, result of a search of raw memory. 
    /// Navigable items take you to the current memory view (if one is topmost) 
    /// opens a new memory view.
    /// </summary>
    public class AddressSearchResult : ISearchResult
    {
        protected readonly IServiceProvider services;
        private List<ProgramAddress> addresses;
        private AddressSearchDetails details;

        public AddressSearchResult(
            IServiceProvider services,
            IEnumerable<ProgramAddress> addresses,
            AddressSearchDetails details)
        {
            this.services = services;
            this.addresses = addresses.ToList();
            this.details = details;
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
            View.AddColumn("Details", 70);
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
            program.ImageMap.TryFindItem(addr, out item);
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

            string sData = "";
            switch (details)
            {
            case AddressSearchDetails.Code: sData = RenderCode(hit); break;
            case AddressSearchDetails.Strings: sData = RenderString(hit); break;
            case AddressSearchDetails.Data: sData = RenderData(hit); break;
            }

            return new SearchResultItem
            {
                Items = new string[] {
                        program.Name ?? "<Program>",
                        addr.ToString(),
                        item.DataType != null ? item.DataType.ToString() : "<null>",
                        sData,
                    },
                ImageIndex = 0,
                BackgroundColor = bgColor,
            };
        }

        public string RenderCode(ProgramAddress hit)
        {
            try
            {
                var dasm = hit.Program.CreateDisassembler(hit.Address);
                return string.Join("; ", dasm.Take(4).Select(inst => inst.ToString().Replace('\t', ' ')));
            }
            catch
            {
                return "<invalid>";
            }
        }

        public string RenderString(ProgramAddress hit)
        {
            var rdr = hit.Program.CreateImageReader(hit.Address);
            var sb = new StringBuilder();
            while (rdr.IsValid)
            {
                var ch = rdr.ReadByte();
                if (ch == 0 || sb.Length > 80)
                    break;
                sb.Append(0x20 <= ch && ch < 0x7F
                    ? (char)ch
                    : '.');
            }
            return sb.ToString();
        }

        public string RenderData(ProgramAddress hit)
        {
            var rdr = hit.Program.CreateImageReader(hit.Address);
            var sb = new StringBuilder();
            int cb = 0;
            while (rdr.IsValid)
            {
                var ch = rdr.ReadByte();
                if (ch == 0 || cb >= 16)
                    break;
                sb.AppendFormat("{0:X2} ", (uint)ch);
                ++cb;
            }
            return sb.ToString();
        }

        private int SelectBgColor(ImageMapItem item)
        {
            if (item.DataType is UnknownType)
                return -1;
            //$TODO: colors should come from settings.
            if (item.DataType is CodeType) 
                return System.Drawing.Color.Pink.ToArgb();
            return 0x00C0C0FF;
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
                case CmdIds.ViewFindWhatPointsHere:
                    status.Status = MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.ViewAsCode:
                    status.Status = details == AddressSearchDetails.Code
                        ? MenuStatus.Enabled | MenuStatus.Visible | MenuStatus.Checked
                        : MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.ViewAsStrings:
                    status.Status = details == AddressSearchDetails.Strings
                        ? MenuStatus.Enabled | MenuStatus.Visible | MenuStatus.Checked
                        : MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.ViewAsData:
                    status.Status = details == AddressSearchDetails.Data
                        ? MenuStatus.Enabled | MenuStatus.Visible | MenuStatus.Checked
                        : MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.ActionMarkProcedure:
                    status.Status = details == AddressSearchDetails.Code
                        ? MenuStatus.Enabled | MenuStatus.Visible
                        : MenuStatus.Visible;
                    return true;
                case CmdIds.ActionMarkType:
                    status.Status = details != AddressSearchDetails.Code
                        ? MenuStatus.Enabled | MenuStatus.Visible
                        : MenuStatus.Visible;
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
            case CmdIds.ViewFindWhatPointsHere: ViewFindWhatPointsHere(); return true;
            case CmdIds.ViewAsCode: details = AddressSearchDetails.Code; View.Invalidate(); return true;
            case CmdIds.ViewAsStrings: details = AddressSearchDetails.Strings; View.Invalidate(); return true;
            case CmdIds.ViewAsData: details = AddressSearchDetails.Data; View.Invalidate(); return true;
            case CmdIds.ActionMarkProcedure: MarkProcedures(); return true;
            case CmdIds.ActionMarkType: MarkType(); return true;
            }
            return false;
        }

        public void MarkProcedures()
        {
            services.RequireService<ICommandFactory>().MarkProcedures(SelectedHits()).Do();
        }

        public void MarkType()
        {
            View.ShowTypeMarker(userText =>
            {
                var parser = new HungarianParser();
                var dataType = parser.Parse(userText);
                if (dataType == null)
                    return;
                foreach (var pa in SelectedHits())
                {
                    pa.Program.AddUserGlobalItem(pa.Address, dataType);
                }
                View.Invalidate();
            });
        }

        /// <summary>
        /// Returns the addresses the user has selected.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProgramAddress> SelectedHits()
        {
            return View.SelectedIndices.Select(i => addresses[i]);
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

    public enum AddressSearchDetails
    {
        Code,
        Strings,
        Data,
    }
}

