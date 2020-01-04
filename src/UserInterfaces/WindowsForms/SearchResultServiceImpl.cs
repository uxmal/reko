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
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class SearchResultServiceImpl : ISearchResultService, IWindowPane, ICommandTarget
    {
        private ListView listView;
        private IServiceProvider services;
        private ISearchResult result;

        public SearchResultServiceImpl(IServiceProvider services, ListView listView)
        {
            if (services == null)
                throw new ArgumentNullException("services");
            this.services = services;
            this.listView = listView;
            listView.VirtualMode = true;
            listView.View = View.Details;
            listView.VirtualListSize = 0;
            this.listView.RetrieveVirtualItem += listView_RetrieveVirtualItem;
            this.listView.CacheVirtualItems += listView_CacheVirtualItems;
            this.listView.DoubleClick += listView_DoubleClick;

            SetSearchResults(new EmptyResult());
        }

        public IWindowFrame Frame { get; set; }

        public object CreateControl()
        {
            return this.listView;
        }

        public void Close()
        {
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
            if (services != null)
            {
                var uiUser = services.RequireService<IUiPreferencesService>();
                uiUser.UiPreferencesChanged += delegate { uiUser.UpdateControlStyle(UiStyles.List, listView); };
                uiUser.UpdateControlStyle(UiStyles.List, listView);
            }
        }

        public void ShowAddressSearchResults(IEnumerable<AddressSearchHit> hits, AddressSearchDetails details)
        {
            ShowSearchResults(new AddressSearchResult(services, hits, details));
        }

        public void ShowSearchResults(ISearchResult result)
        {
            SetSearchResults(result);
            services.RequireService<IWindowFrame>().Show();
        }

        private void SetSearchResults(ISearchResult result)
        {
            if (this.result != null)
                this.result.View = null;
            this.result = result;
            if (!listView.VirtualMode)
                listView.Clear();
            listView.VirtualListSize = result.Count;
            var searchResultView = new SearchResultView(this.listView);
            result.View = searchResultView;
            result.CreateColumns();
            var ctxMenuID = result.ContextMenuID;
            if (ctxMenuID > 0)
            {
                var uiSvc = services.RequireService<IDecompilerShellUiService>();
                uiSvc.SetContextMenu(listView, ctxMenuID);
            }
        }

        private ListViewItem GetItem(int i)
        {
            if (0 <= i && i < result.Count)
            {
                var sri = result.GetItem(i);
                var item = new ListViewItem(sri.Items);
                item.ImageIndex = sri.ImageIndex;
                if (sri.BackgroundColor != -1)
                {
                    item.BackColor = System.Drawing.Color.FromArgb(sri.BackgroundColor);
                }
                item.Tag = i;
                return item;
            }
            else
                return null;
        }

        void listView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = GetItem(e.ItemIndex);
        }

        void listView_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            ListViewItem[] cache = new ListViewItem[e.EndIndex - e.StartIndex + 1];
            for (int i = e.StartIndex; i <= e.EndIndex; ++i)
            {
                cache[i - e.StartIndex] = GetItem(i);
            }
        }

        public void DoubleClickItem(int i)
        {
            result.NavigateTo(i);
        }

        public void Advance(int distance)
        {
            int itemCount = this.result.Count;
            if (itemCount < 2)
                return;
            int i;
            if (listView.FocusedItem == null)
            {
                i = 0;
            }
            else
            {
                i = ((int) listView.FocusedItem.Tag + itemCount + distance) % itemCount;
            }
            listView.SelectedIndices.Clear();
            listView.SelectedIndices.Add(i);
            listView.FocusedItem = listView.Items[i];
            listView.FocusedItem.EnsureVisible();
            result.NavigateTo(i);
        }

        void listView_DoubleClick(object sender, EventArgs e)
        {
            if (listView.FocusedItem == null)
                return;
            DoubleClickItem((int)listView.FocusedItem.Tag);
        }

        private class SearchResultView : ISearchResultView
        {
            private ListView listView;
            private TypeMarker typeMarker;

            public SearchResultView(ListView lv)
            {
                this.listView = lv;
                this.listView.Columns.Clear();
                this.typeMarker = new TypeMarker(listView);
            }

            public bool IsFocused { get { return listView.Focused; } }

            public IEnumerable<int> SelectedIndices
            {
                get { return listView.SelectedIndices.Cast<int>(); }
            }

            public void AddColumn(string columnText, int widthInCharacters)
            {
                var colHeader = new ColumnHeader();
                colHeader.Text = columnText;
                colHeader.Width = listView.Font.Height * widthInCharacters;
                listView.Columns.Add(colHeader);
            }

            public void Invalidate()
            {
                listView.Invalidate();
            }

            public void ShowTypeMarker(Action<string> action)
            {
                var i = listView.TopItem;
                var rc = listView.DisplayRectangle;

                typeMarker.Show(i.Position, action);
            }
        }

        /// <summary>
        /// A null object used when "idle".
        /// </summary>
        private class EmptyResult : ISearchResult
        {
            public int Count
            {
                get { return 1; }
            }

            public int ContextMenuID { get { return 0; } }

            public int SortedColumn { get { return -1; } }

            public ISearchResultView View { get; set; }

            public void CreateColumns()
            {
                View.AddColumn("",40);
            }

            public SearchResultItem GetItem(int i)
            {
                return new SearchResultItem
                {
                    Items = new string[] { "No items found." },
                    ImageIndex = -1,
                    BackgroundColor = -1,
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

            public bool IsColumnSortable(int iColumn)
            {
                return false;
            }

            public SortDirection GetSortDirection(int iColumn)
            {
                return SortDirection.None;
            }

            public void NavigateTo(int i)
            {
            }

            public void SortByColumn(int iColumn, SortDirection dir)
            {
            }
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (result == null)
                return false;
            return result.QueryStatus(cmdId, status, text);
        }

        public bool Execute(CommandID cmdId)
        {
            if (result != null && result.Execute(cmdId))
                return true;
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ActionNextSearchHit: Advance(1); return true;
                case CmdIds.ActionPrevSearchHit: Advance(-1); return true;
                }
            }
            return false;
        }
    }
}
