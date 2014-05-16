#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class SearchResultServiceImpl : ISearchResultService
    {
        private ListView listView;
        private IServiceProvider services;
        private ISearchResult result;
        private IWindowFrame frame;

        public SearchResultServiceImpl(IServiceProvider services, IWindowFrame frame, ListView listView)
        {
            if (services == null)
                throw new ArgumentNullException("services");
            if (frame == null)
                throw new ArgumentNullException("frame");
            this.services = services;
            this.frame = frame;
            this.listView = listView;
            listView.VirtualMode = true;
            listView.View = View.Details;
            listView.VirtualListSize = 0;
            this.listView.RetrieveVirtualItem += listView_RetrieveVirtualItem;
            this.listView.CacheVirtualItems += listView_CacheVirtualItems;
            this.listView.DoubleClick += listView_DoubleClick;

            ShowSearchResults(new EmptyResult());
        }

        public void ShowSearchResults(ISearchResult result)
        {
            this.result = result;
			if(!listView.VirtualMode)
            	listView.Clear();
            listView.VirtualListSize = result.Count;
            var searchResultView = new SearchResultView(this.listView);
            result.CreateColumns(searchResultView);
            var ctxMenuID = result.ContextMenuID;
            if (ctxMenuID > 0)
            {
                listView.ContextMenu = services
                    .RequireService<IDecompilerShellUiService>()
                    .GetContextMenu(ctxMenuID);
            }
            frame.Show();
        }

        private ListViewItem GetItem(int i)
        {
            if (0 <= i && i < result.Count)
            {
                var item = new ListViewItem(result.GetItemStrings(i));
                item.ImageIndex = result.GetItemImageIndex(i);
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

        void listView_DoubleClick(object sender, EventArgs e)
        {
            if (listView.FocusedItem == null)
                return;
            DoubleClickItem((int)listView.FocusedItem.Tag);
        }


        private class SearchResultView : ISearchResultView
        {
            private ListView listView;

            public SearchResultView(ListView lv)
            {
                this.listView = lv;
            }

            #region ISearchResultView Members

            public void AddColumn(string columnText, int widthInCharacters)
            {
                    var colHeader = new ColumnHeader();
                    colHeader.Text = columnText;
                    colHeader.Width = listView.Font.Height * widthInCharacters;
                    listView.Columns.Add(colHeader);
            }

            #endregion
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

            public void CreateColumns(ISearchResultView view)
            {
                view.AddColumn("",40);
            }

            public int GetItemImageIndex(int i)
            {
                return -1;
            }

            public string[] GetItemStrings(int i)
            {
                return new string[] { "No items found." };
            }

            public void NavigateTo(int i)
            {
            }
        }
    }
}
