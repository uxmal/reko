#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Loading;
using Reko.Gui.ViewModels;
using Reko.Gui.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class BaseAddressFinderView : UserControl, IWindowPane
    {
        private BaseAddressFinderViewModel viewModel;

        public BaseAddressFinderView()
        {
            InitializeComponent();
        }

        public IWindowFrame Frame { get; set; }

        public BaseAddressFinderViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                this.viewModel = value;
                CreateBindings();
            }
        }


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
            this.btnStartStop.DataBindings.Add(
                nameof(btnStartStop.Text),
                viewModel,
                nameof(viewModel.StartStopButtonText));

            this.btnChangeBaseAddress.DataBindings.Add(
                nameof(btnStartStop.Enabled),
                viewModel,
                nameof(viewModel.ChangeBaseAddressEnabled));

            this.txtBaseAddress.DataBindings.Add(
                nameof(txtBaseAddress.Text),
                viewModel,
                nameof(viewModel.BaseAddress));

            viewModel.Results.CollectionChanged += Results_CollectionChanged;
        }

        private void Results_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            listCandidates.Invoke(() =>
            {
                switch (e.Action)
                {
                case NotifyCollectionChangedAction.Reset:
                    listCandidates.Items.Clear();
                    break;
                case NotifyCollectionChangedAction.Add:
                    int i = e.NewStartingIndex;
                    foreach (BaseAddressResult item in e.NewItems)
                    {
                        listCandidates.Items.Insert(i, new ListViewItem
                        {
                            Text = item.Address,
                            SubItems =
                        {
                            item.Confidence.ToString()
                        }
                        });
                        ++i;
                    }
                    break;
                default:
                    throw new NotImplementedException();
                }
            });
        }

        private async void btnStartStop_Click(object sender, EventArgs e)
        {
            await viewModel?.StartStopFinder();
        }

        private void listCandidates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (viewModel is null)
                return;
            var item = listCandidates.FocusedItem;
            viewModel.BaseAddress = item.Text;
        }

        private void btnChangeBaseAddress_Click(object sender, EventArgs e)
        {
            viewModel?.ChangeBaseAddress();
        }
    }
}
