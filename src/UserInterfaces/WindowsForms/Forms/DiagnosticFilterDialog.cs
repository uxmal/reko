#region License
/* 
* Copyright (C) 1999-2026 John Källén.
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

using Reko.Gui.Forms;
using Reko.Gui.Services;
using System;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class DiagnosticFilterDialog : Form,
        IDiagnosticFilterDialog
    {
        public DiagnosticFilterDialog()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PopulateOptions();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            this.Value = ValueFromCheckboxList();
            base.OnClosed(e);
        }

        public DiagnosticFilters Value { get; set; }

        private void PopulateOptions()
        {
            this.chklistFilters.Items.Clear();
            AddItem("Errors", DiagnosticFilters.Errors);
            AddItem("Warnings", DiagnosticFilters.Warnings);
            AddItem("Information", DiagnosticFilters.Information);
        }

        private void AddItem(string label, DiagnosticFilters flag)
        {
            bool isChecked = this.Value.HasFlag(flag);
            this.chklistFilters.Items.Add(label, isChecked);
        }

        private DiagnosticFilters ValueFromCheckboxList()
        {
            DiagnosticFilters result = 0;
            if (chklistFilters.GetItemChecked(0)) result |= DiagnosticFilters.Errors;
            if (chklistFilters.GetItemChecked(1)) result |= DiagnosticFilters.Warnings;
            if (chklistFilters.GetItemChecked(2)) result |= DiagnosticFilters.Information;
            return result;
        }
    }
}
