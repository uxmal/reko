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
    public interface ISearchResult
    {
        int Count { get; }

        void CreateColumns(ISearchResultView view);

        int GetItemImageIndex(int i);

        string[] GetItemStrings(int i);

        void NavigateTo(int i);
    }

    public interface ISearchResultView
    {
        void AddColumn(string p, int p_2);
    }
}