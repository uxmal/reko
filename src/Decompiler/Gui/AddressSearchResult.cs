#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using System.Linq;
using System.Text;

namespace Decompiler.Gui
{
    /// <summary>
    /// An address search result is the ... well, result of a search of raw memory. Navigable items take you
    /// to the current memory view (if one is topmost) or opens a new memory view.
    /// </summary>
    public class AddressSearchResult : ISearchResult
    {
        private IServiceProvider services;

        public AddressSearchResult(IServiceProvider services, LoadedImage image, ImageMap map, List<uint> linearAddresses)
        {
            this.services = services;
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public int ContextMenuID
        {
            get { throw new NotImplementedException(); }
        }

        public void CreateColumns(ISearchResultView view)
        {
            throw new NotImplementedException();
        }

        public int GetItemImageIndex(int i)
        {
            throw new NotImplementedException();
        }

        public string[] GetItemStrings(int i)
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(int i)
        {
            throw new NotImplementedException();
        }
    }
}
