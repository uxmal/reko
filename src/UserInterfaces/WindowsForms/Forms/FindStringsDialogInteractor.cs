#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Dialogs;
using Reko.Scanning;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class FindStringsDialogInteractor
    {
        private FindStringsDialog dlg;
        private readonly FindStringsViewModel viewModel;

        public FindStringsDialogInteractor(Program program, IDecompilerShellUiService uiSvc, ISettingsService settingsSvc, IDialogFactory dialogFactory)
        {
            this.viewModel = new FindStringsViewModel(program, uiSvc, settingsSvc, dialogFactory);
        }

        public void Attach(FindStringsDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += Dialog_Load;
            dlg.FormClosed += Dialog_FormClosed;
            dlg.SearchAreaButton.Click += SearchAreaButton_Click;
            this.viewModel.SearchAreasMru.CollectionChanged += SearchAreasMru_CollectionChanged;
            this.viewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.PopulateSearchAreas();
        }

        private void Dialog_Load(object sender, EventArgs e)
        {
            dlg.StringKindList.SelectedIndex = 0;
            dlg.CharacterSizeList.SelectedIndex = 0;
            dlg.SearchAreas.SelectedIndex = 0;
        }

        private void Dialog_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (dlg.DialogResult != System.Windows.Forms.DialogResult.Cancel)
            {
                viewModel.SaveMruToSettings();
            }
        }

        private async void SearchAreaButton_Click(object sender, EventArgs e)
        {
            await viewModel.SelectSearchArea();
        }

        public StringFinderCriteria GetCriteria() => viewModel.GetCriteria(
            dlg.CharacterSizeList.SelectedIndex,
            dlg.StringKindList.SelectedIndex,
            dlg.MinLength);


        private void SearchAreasMru_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PopulateSearchAreas();
        }


        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
            case nameof(FindStringsViewModel.SelectedMruIndex):
                dlg.SearchAreas.SelectedIndex = viewModel.SelectedMruIndex;
                break;
            }
        }

        private void PopulateSearchAreas()
        {
            dlg.SearchAreas.Items.Clear();
            dlg.SearchAreas.Items.AddRange(viewModel.SearchAreasMru.ToArray());
        }
    }
}