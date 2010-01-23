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

namespace Decompiler.Gui
{
    /// <summary>
    /// Represents the results of a search as a list of items which are navigatable.
    /// The class is used by the SearchResultService to
    /// display items and to navigate to items when requested by the user. Implementations of derived
    /// classes should behave as caches if possible.
    /// </summary>
    public abstract class SearchResult
    {
        public abstract int Count { get; }

        public abstract SearchResultItem this[int i] { get;}

        public abstract IEnumerable<SearchResultColumn> GetColumns();
    }

    public abstract class SearchResultItem
    {
        public ListViewItem CreateListViewItem()
        {
            var item = new ListViewItem();
            item.Text = "Yo, sup?";
            return item;
        }
    }

    public class SearchResultColumn
    {
        private string text;
        private int width;

        public SearchResultColumn(string text, int widthInCharacters)
        {
            this.text = text;
            this.width = widthInCharacters;
        }

        public int WidthInCharacters
        {
            get { return width; }
        }

        public string Text
        {
            get { return text; }
        }
    }
}