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

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// Represents the results of a search as a list of items which are navigatable.
    /// The class is used by the SearchResultService to
    /// display items and to navigate to items when requested by the user. Implementations of derived
    /// classes should behave as caches if possible.
    /// </summary>
    public interface ISearchResult : ICommandTarget
    {
        ISearchResultView View { get; set; }
        int Count { get; }
        int ContextMenuID { get; }      // Context menu to use.
        int SortedColumn { get; }

        void CreateColumns();
        SearchResultItem GetItem(int i);
        void NavigateTo(int i);
        bool IsColumnSortable(int iColumn);
        SortDirection GetSortDirection(int iColumn);
        void SortByColumn(int iColumn, SortDirection dir);
    }

    public enum SortDirection { None, Up, Down } 

    public class SearchResultItem
    {
        public string [] Items;
        public int ImageIndex;
        public int BackgroundColor;
    }

    public interface ISearchResultView
    {
        bool IsFocused { get; }
        IEnumerable<int> SelectedIndices { get; }

        void AddColumn(string columnTitle, int width);
        void Invalidate();
        void ShowTypeMarker(Action<string> action);
    }
}