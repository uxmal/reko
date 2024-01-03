#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
 .
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

using Reko.Gui;
using Reko.Gui.ViewModels.Dialogs;
using Reko.Scanning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class SearchAreaDialog : Form, IDialog<List<SearchArea>>
    {
        private SearchAreaViewModel viewModel;

        public SearchAreaDialog()
        {
            InitializeComponent();
        }

        public SearchAreaViewModel DataContext
        {
            get { return viewModel; }
            set
            {
                this.viewModel = value;
                this.lblFreeFormError.DataBindings.Add(
                    nameof(lblFreeFormError.Text),
                    viewModel,
                    nameof(viewModel.FreeFormError));
                OnDataContextChanged();
            }
        }

        public List<SearchArea> Value { get; set; }

        private void OnDataContextChanged()
        {
            foreach (var segment in viewModel.SegmentList)
            {
                var item = new ListViewItem(new[]
                {
                    Text = segment.Name,
                    Text = segment.Address,
                    Text = segment.Access,
                })
                {
                    Tag = segment,
                };
                listSegments.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Value = viewModel.Areas;
        }

        private void listSegments_ItemChecked(object sender, EventArgs e)
        {
            IEnumerable en = listSegments.CheckedItems
                .OfType<ListViewItem>()
                .Select(i => i.Tag);
            viewModel.ChangeAreas(en);
        }

        private void txtFreeForm_TextChanged(object sender, EventArgs e)
        {
            viewModel.FreeFormAreas = txtFreeForm.Text;
        }
    }
}
