#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Reko.Core;

namespace Reko.Gui
{
    public class ProcedureListService : IProcedureListService
    {
        private const string HelpText = "Search procedures";

        private readonly IServiceProvider services;
        private readonly TabPage tabPage;
        private readonly TextBox txtProcedureFilter;
        private readonly ListView listProcedures;
        private bool showHelpText;
        private List<(Program, Procedure)> procs;

        public ProcedureListService(IServiceProvider services, TabPage tabPage, TextBox txtProcedureFilter, ListView listProcedures)
        {
            this.services = services;
            this.tabPage = tabPage;
            this.txtProcedureFilter = txtProcedureFilter;
            this.listProcedures = listProcedures;
            this.showHelpText = true;
            txtProcedureFilter.GotFocus += TxtProcedureFilter_GotFocus;
            txtProcedureFilter.LostFocus += TxtProcedureFilter_LostFocus;
            txtProcedureFilter.TextChanged += TxtProcedureFilter_TextChanged;
            listProcedures.DoubleClick += ListProcedures_DoubleClick;
            Clear();
        }


        public Procedure SelectedProcedure
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ContainsFocus
        {
            get { return this.txtProcedureFilter.Focused || this.listProcedures.Focused;  }
        }

        public void Clear()
        {
            procs = new List<(Program,Procedure)>();
            this.showHelpText = true;
            this.txtProcedureFilter.Text = HelpText;
            this.txtProcedureFilter.ForeColor = System.Drawing.SystemColors.GrayText;
            ShowProcedures(new (Program, Procedure)[0]);
        }

        public void Load(Project project)
        {
            this.procs = project.Programs
                .SelectMany(program => program.Procedures.Values.Select(p => (program, p)))
                .ToList();
            if (procs.Count == 0)
            {
                Clear();
                return;
            }
            ShowProcedures(procs);
        }

        private (string,string) StringizeProcedure(Procedure proc)
        {
            return (proc.EntryAddress.ToString(), proc.Name);
        }

        private void ShowProcedures(IEnumerable<(Program program, Procedure proc)> sProcs)
        {
            var items = sProcs.Select(CreateListItem).ToArray();
            if (items.Length == 0)
            {
                items = new[] { new ListViewItem(new[] { "(None)", "" }) };
            }
            this.listProcedures.Items.Clear();
            this.listProcedures.Items.AddRange(items);
        }

        private ListViewItem CreateListItem((Program program, Procedure proc) item)
        {
            return new ListViewItem(new[] { item.proc.EntryAddress.ToString(), item.proc.Name })
            {
                Tag = item
            };
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            return false;
        }

        public void Show()
        {
            ((TabControl) tabPage.Parent).SelectedTab = tabPage;
        }

        private void TxtProcedureFilter_LostFocus(object sender, EventArgs e)
        {
            if (!this.showHelpText && txtProcedureFilter.Text.Length == 0)
            {
                this.showHelpText = true;
                this.txtProcedureFilter.Text = HelpText;
                this.txtProcedureFilter.ForeColor = SystemColors.GrayText;
            }
        }

        private void TxtProcedureFilter_GotFocus(object sender, EventArgs e)
        {
            if (this.showHelpText)
            {
                this.txtProcedureFilter.Text = "";
                this.showHelpText = false;
                this.txtProcedureFilter.ForeColor = SystemColors.ControlText;
            }
        }

        private void TxtProcedureFilter_TextChanged(object sender, EventArgs e)
        {
            if (showHelpText)
                return;
            var searchText = txtProcedureFilter.Text.Trim();
            IEnumerable<(Program, Procedure)> procs = this.procs;
            if (!string.IsNullOrEmpty(searchText))
            {
                procs = procs.Where(sItem => FilterProcedure(searchText, sItem));
            }
            ShowProcedures(procs);
        }

        private bool FilterProcedure(string searchtext, (Program program, Procedure proc) sItem)
        {
            return (sItem.proc.EntryAddress.ToString().Contains(searchtext) || 
                sItem.proc.Name.Contains(searchtext));
        }

        private void ListProcedures_DoubleClick(object sender, EventArgs e)
        {
            var item = listProcedures.FocusedItem;
            if (item == null || !item.Selected || item.Tag == null)
                return;
            var (program, proc) = ((Program, Procedure)) item.Tag;
            services.RequireService<ICodeViewerService>().DisplayProcedure(program, proc, program.NeedsScanning);
        }
    }
}
