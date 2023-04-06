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
using Reko.Gui.Components;
using Reko.Gui.ViewModels.Tools;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class CallGraphNavigatorView : UserControl
    {
        private Font boldFont;
        private NavigationInteractor<Procedure> navInteractor;

        public CallGraphNavigatorView()
        {
            InitializeComponent();
            MakeBoldFont();
            this.navInteractor = new NavigationInteractor<Procedure>();
            this.btnBack.Click += delegate { ViewModel.NavigateTo(navInteractor.NavigateBack()); };
            this.btnForward.Click += delegate { ViewModel.NavigateTo(navInteractor.NavigateForward()); };
            this.navInteractor.PropertyChanged += delegate
            {
                this.btnBack.Enabled = navInteractor.BackEnabled;
                this.btnForward.Enabled = navInteractor.ForwardEnabled;
            };
        }


        public CallGraphNavigatorViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                if (this.viewModel == value)
                    return;
                DestroyBindings();
                this.viewModel = value;
                if (this.viewModel is not null)
                {
                    PopulateList(this.viewModel.Predecessors, this.listCallers);
                    PopulateList(this.viewModel.Successors, this.listCallees);
                }
                CreateBindings();
                this.btnBack.Enabled = false;
                this.btnForward.Enabled = false;
            }
        }
        private CallGraphNavigatorViewModel viewModel;

        private void CreateBindings()
        {
            this.linkProcedure.DataBindings.Add(
                nameof(linkProcedure.Text),
                viewModel,
                nameof(viewModel.NodeTitle));

            this.lblAddress.DataBindings.Add(
                nameof(lblAddress.Text),
                viewModel,
                nameof(viewModel.NodeDescription));

            this.lblSignature.DataBindings.Add(
                nameof(lblSignature.Text),
                viewModel,
                nameof(viewModel.NodeDetails));

            if (this.viewModel is not null)
            {
                this.viewModel.Predecessors.CollectionChanged += predecessors_CollectionChanged;
                this.viewModel.Successors.CollectionChanged += successors_CollectionChanged;
            }
        }

        private void DestroyBindings()
        {
            this.linkProcedure.DataBindings.Clear();
            this.lblAddress.DataBindings.Clear();
            this.lblSignature.DataBindings.Clear();

            if (this.viewModel is not null)
            {
                this.viewModel.Predecessors.CollectionChanged -= predecessors_CollectionChanged;
                this.viewModel.Successors.CollectionChanged -= successors_CollectionChanged;
            }
        }


        private void PopulateList(ObservableCollection<CallGraphViewModelItem> vmList, ListView listView)
        {
            foreach (var vmItem in vmList)
            {
                listView.Items.Add(MakeListItem(vmItem));
            }
        }


        private void successors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HandleListEvent(viewModel.Successors, this.listCallees, e);
        }

        private void predecessors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HandleListEvent(viewModel.Predecessors, this.listCallers, e);
        }

        private void HandleListEvent(ObservableCollection<CallGraphViewModelItem> list, ListView listView, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
                int i = e.NewStartingIndex;
                foreach (CallGraphViewModelItem vmItem in e.NewItems)
                {
                    var listItem = MakeListItem(vmItem);
                    listView.Items.Insert(i, listItem);
                    ++i;
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                listView.Items.Clear();
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private ListViewItem MakeListItem(CallGraphViewModelItem vmItem)
        {
            return new ListViewItem(new string[]
            {
                vmItem.Title,
                vmItem.Description,
            })
            {
                Font = vmItem.IsVisited ? this.Font : this.boldFont,
                Tag = vmItem
            };
        }

        private void MakeBoldFont()
        {
            this.boldFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);
        }

        private void NavigateTo(ListViewItem item)
        {
            if (item is not null && item.Tag is CallGraphViewModelItem vmItem)
            {
                if (ViewModel.NodeObject is not null)
                {
                    navInteractor.RememberLocation(ViewModel.NodeObject.NodeObject);
                }
                ViewModel.NavigateTo(vmItem);
            }
        }


        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            MakeBoldFont();
        }

        private void listCallees_DoubleClick(object sender, EventArgs e)
        {
            NavigateTo(listCallees.FocusedItem);
        }

        private void listCallers_DoubleClick(object sender, EventArgs e)
        {
            NavigateTo(listCallers.FocusedItem);
        }
    }
}
