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

        private readonly TabPage tabPage;
        private readonly TextBox txtProcedureFilter;
        private readonly ListView listProcedures;
        private bool showHelpText;
        private List<Procedure> procs;

        public ProcedureListService(TabPage tabPage, TextBox txtProcedureFilter, ListView listProcedures)
        {
            this.tabPage = tabPage;
            this.txtProcedureFilter = txtProcedureFilter;
            this.listProcedures = listProcedures;
            this.showHelpText = true;
            txtProcedureFilter.GotFocus += TxtProcedureFilter_GotFocus;
            txtProcedureFilter.LostFocus += TxtProcedureFilter_LostFocus;
            txtProcedureFilter.TextChanged += TxtProcedureFilter_TextChanged;
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
            procs = new List<Procedure>();
            this.showHelpText = true;
            this.txtProcedureFilter.Text = HelpText;
            this.txtProcedureFilter.ForeColor = System.Drawing.SystemColors.GrayText;
            ShowProcedures(new (string, string)[0]);
        }

        public void Load(Project project)
        {
            this.procs = project.Programs
                .SelectMany(program => program.Procedures.Values)
                .ToList();
            if (procs.Count == 0)
            {
                Clear();
                return;
            }
            ShowProcedures(procs.Select(StringizeProcedure));
        }

        private (string,string) StringizeProcedure(Procedure proc)
        {
            return (proc.EntryAddress.ToString(), proc.Name);
        }

        private void ShowProcedures(IEnumerable<(string addr, string name)> sProcs)
        {
            var items = sProcs.Select(CreateListItem).ToArray();
            if (items.Length == 0)
            {
                items = new[] { new ListViewItem(new[] { "(None)", "" }) };
            }
            this.listProcedures.Items.Clear();
            this.listProcedures.Items.AddRange(items);
        }

        private ListViewItem CreateListItem((string addr, string name) procStrings)
        {
            return new ListViewItem(new[] { procStrings.addr, procStrings.name });
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
            var stringizedItems = this.procs.Select(StringizeProcedure);
            if (!string.IsNullOrEmpty(searchText))
            {
                stringizedItems = stringizedItems.Where(sItem => FilterProcedure(searchText, sItem));
            }
            ShowProcedures(stringizedItems);
        }

        private bool FilterProcedure(string searchtext, (string addr, string name) sItem)
        {
            return (sItem.addr.Contains(searchtext) || sItem.name.Contains(searchtext));
        }
    }
}
