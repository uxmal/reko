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

using Reko.Core;
using Reko.Core.Types;
using Reko.Scanning;
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
        private List<AddressSearchHit> hits;
        private AddressSearchDetails details;

        public AddressSearchResult(
            IServiceProvider services,
            IEnumerable<AddressSearchHit> addresses,
            AddressSearchDetails details)
        {
            this.services = services;
            this.hits = addresses.ToList();
            this.details = details;
        }

        public ISearchResultView View { get; set; }

        public int Count
        {
            get { return hits.Count; }
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
            var hit = hits[i];
            var program = hit.Program;
            var addr = hits[i].Address;
            program.ImageMap.TryFindItem(addr, out var item);
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

            string sData = details.RenderHit(hit);

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
            var hit = hits[i];
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
                    status.Status = details is CodeSearchDetails
                        ? MenuStatus.Enabled | MenuStatus.Visible | MenuStatus.Checked
                        : MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.ViewAsStrings:
                    status.Status = details is StringSearchDetails
                        ? MenuStatus.Enabled | MenuStatus.Visible | MenuStatus.Checked
                        : MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.ViewAsData:
                    status.Status = details is DataSearchDetails
                        ? MenuStatus.Enabled | MenuStatus.Visible | MenuStatus.Checked
                        : MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.ActionMarkProcedure:
                    status.Status = details is CodeSearchDetails
                        ? MenuStatus.Enabled | MenuStatus.Visible
                        : MenuStatus.Visible;
                    return true;
                case CmdIds.ActionMarkType:
                    status.Status = details is CodeSearchDetails
                        ? MenuStatus.Visible
                        : details is StringSearchDetails
                            ? 0
                            : MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.ActionMarkStrings:
                    status.Status = details is StringSearchDetails
                        ? MenuStatus.Enabled | MenuStatus.Visible
                        : 0;
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
            case CmdIds.ViewAsCode: details = new CodeSearchDetails(); View.Invalidate(); return true;
            case CmdIds.ViewAsStrings: details = new StringSearchDetails(new StringFinderCriteria
            {
                Encoding = Encoding.ASCII,
                StringType = StringType.NullTerminated(PrimitiveType.Char),
            }); View.Invalidate(); return true;
            case CmdIds.ViewAsData: details = new DataSearchDetails(); View.Invalidate(); return true;
            case CmdIds.ActionMarkProcedure: MarkProcedures(); return true;
            case CmdIds.ActionMarkType: MarkType(); return true;
            case CmdIds.ActionMarkStrings: MarkStrings(); return true;
            }
            return false;
        }

        public void MarkProcedures()
        {
            //$TODO: if > 1 arch, pick one.
            var procAddrs = SelectedHits().Select(hit => new ProgramAddress(hit.Program, hit.Address));
            services.RequireService<ICommandFactory>().MarkProcedures(procAddrs).Do();
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
                    pa.Program.AddUserGlobalItem(pa.Program.Architecture, pa.Address, dataType);
                }
                View.Invalidate();
            });
        }

        public void MarkStrings()
        {
            foreach (var pa in SelectedHits())
            {
                var sDetails = (StringSearchDetails) details;
                var dt = sDetails.Criteria.StringType;
                pa.Program.AddUserGlobalItem(pa.Program.Architecture, pa.Address, dt);
            }
        }

        /// <summary>
        /// Returns the addresses the user has selected.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AddressSearchHit> SelectedHits()
        {
            return View.SelectedIndices.Select(i => hits[i]);
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


    public abstract class AddressSearchDetails
    {
        public abstract string RenderHit(AddressSearchHit hit);
    }

    public class CodeSearchDetails : AddressSearchDetails
    {
        public override string RenderHit(AddressSearchHit hit)
        {
            try
            {
                var dasm = hit.Program.CreateDisassembler(hit.Program.Architecture, hit.Address);
                return string.Join("; ", dasm.Take(4).Select(inst => inst.ToString().Replace('\t', ' ')));
            }
            catch
            {
                return "<invalid>";
            }
        }
    }

    public class DataSearchDetails : AddressSearchDetails
    {
        public override string RenderHit(AddressSearchHit hit)
        {
            var arch = hit.Program.Architecture;
            var rdr = hit.Program.CreateImageReader(arch, hit.Address);
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
    }

    public class StringSearchDetails : AddressSearchDetails
    {

        public StringSearchDetails(StringFinderCriteria criteria)
        {
            this.Criteria = criteria;
        }

        public StringFinderCriteria Criteria { get; }

        public override string RenderHit(AddressSearchHit hit)
        {
            var arch = hit.Program.Architecture;
            var rdr = hit.Program.CreateImageReader(arch, hit.Address);
            var bytes = rdr.ReadBytes(hit.Length);
            return Criteria.Encoding.GetString(bytes.ToArray());
        }
    }
}

