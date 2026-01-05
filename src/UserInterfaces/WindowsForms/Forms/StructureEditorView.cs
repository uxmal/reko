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

using Reko.Gui.ViewModels;
using Reko.Gui.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class StructureEditorView : UserControl, IWindowPane
    {
        public StructureEditorView()
        {
            InitializeComponent();
            UiTools.AddSelectAllHandler(this.txtStructureName);
            UiTools.AddSelectAllHandler(this.txtPreview);
            this.gridFields.AutoGenerateColumns = false;
        }

        public StructureEditorViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                this.viewModel = value;
                CreateBindings();
            }
        }
        private StructureEditorViewModel viewModel;

        public IWindowFrame Frame { get; set; }

        public void Close()
        {
        }

        public void SetSite(IServiceProvider services)
        {
        }

        object IWindowPane.CreateControl()
        {
            return this;
        }

        private void CreateBindings()
        {
            this.gridFields.DataSource = ViewModel.StructFields;

            this.txtPreview.DataBindings.Add(
                nameof(txtPreview.Text),
                viewModel,
                nameof(viewModel.RenderedType));

            this.txtStructureName.DataBindings.Add(
                nameof(txtStructureName.Text),
                viewModel,
                nameof(viewModel.StructureName),
                false,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void btnAddField_Click(object sender, EventArgs e)
        {
            ViewModel.AddField();
        }

        private void btnRemoveField_Click(object sender, EventArgs e)
        {

        }
    }
}
