/* 
 * Copyright (C) 1999-2010 John Källén.
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class SearchResultServiceImpl : ISearchResultService
    {
        private ListView listView;
        private SearchResult result;

        public SearchResultServiceImpl(ListView listView)
        {
            this.listView = listView;
            listView.VirtualMode = true;
            listView.View = View.Details;
            listView.VirtualListSize = 0;
            this.listView.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(listView_RetrieveVirtualItem);
            this.listView.CacheVirtualItems += new CacheVirtualItemsEventHandler(listView_CacheVirtualItems);

        }


        public void ShowSearchResults(SearchResult result)
        {
            this.result = result;
            listView.Clear();
            listView.VirtualListSize = result.Count;
            foreach (SearchResultColumn column in result.GetColumns())
            {
                var colHeader = new ColumnHeader();
                colHeader.Text = column.Text;
                colHeader.Width = listView.Font.Height * column.WidthInCharacters;
                listView.Columns.Add(colHeader);
            }
        }

        private ListViewItem GetItem(int i)
        {
            if (0 <= i && i < result.Count)
                return result[i].CreateListViewItem();
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
    }
}
