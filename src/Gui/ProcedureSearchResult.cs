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

using Reko.Core;
using Reko.Core.Services;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Reko.Gui
{
    public class ProcedureSearchResult : ISearchResult
    {
        private readonly IServiceProvider sp;
        private readonly List<ProcedureSearchHit> hits;

        public ProcedureSearchResult(IServiceProvider sp, List<ProcedureSearchHit> procs)
        {
            this.sp = sp;
            this.hits = procs;
        }

        public ISearchResultView? View { get; set; }

        public int Count
        {
            get { return hits.Count; }
        }

        public int ContextMenuID { get { return MenuIds.CtxProcedure; } }

        public void CreateColumns()
        {
            if (View is not null)
            {
                View.AddColumn("Program", 10);
                View.AddColumn("Address", 8);
                View.AddColumn("Procedure Name", 20);
            }
        }

        public SearchResultItem GetItem(int i)
        {
            return new SearchResultItem(
                new[] {
                    hits[i].Program.Name,
                    hits[i].Address.ToString(),
                    hits[i].Procedure.Name
                },
                ImageIndex:-1,
                BackgroundColor: -1
            );
        }

        public void NavigateTo(int i)
        {
            var codeSvc = sp.GetService<ICodeViewerService>();
            var hit = hits[i];
            if (codeSvc is not null)
                codeSvc.DisplayProcedure(hit.Program, hit.Procedure, hit.Program.NeedsScanning);

            var mvs = sp.GetService<ILowLevelViewService>();
            if (mvs is null)
                return;
            mvs.ShowMemoryAtAddress(hit.Program, hit.Address);
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }

        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            return ValueTask.FromResult(false);
        }

        public int SortedColumn
        {
            get { return -1; }
        }

        public bool IsColumnSortable(int iColumn)
        {
            return false;
        }

        public SortDirection GetSortDirection(int iColumn)
        {
            return SortDirection.None;
        }

        public void SortByColumn(int iColumn, SortDirection dir)
        {
        }
    }

    public class ProcedureSearchHit
    {
        public Program Program;
        public Address Address;
        public Procedure Procedure;

        public ProcedureSearchHit(Program program, Address address, Procedure proc)
        {
            this.Program = program;
            this.Address = address;
            this.Procedure = proc;
        }
    }
}
