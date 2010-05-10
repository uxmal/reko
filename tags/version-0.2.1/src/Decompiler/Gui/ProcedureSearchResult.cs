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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Decompiler.Gui
{
    public class ProcedureSearchResult : ISearchResult
    {
        private IServiceProvider sp;
        private SortedList<Address, Procedure> procs;

        public ProcedureSearchResult(IServiceProvider sp, SortedList<Address, Procedure> procs)
        {
            this.sp = sp;
            this.procs = procs;
        }


        public int Count
        {
            get { return procs.Count; }
        }

        public void CreateColumns(ISearchResultView view)
        {
            view.AddColumn("Address", 8);
            view.AddColumn("Procedure Name", 20);
        }

        public int GetItemImageIndex(int i)
        {
            return -1;
        }

        public string[] GetItemStrings(int i)
        {
            return new string[] {
                procs.Keys[i].ToString(),
                procs.Values[i].Name
            };
        }

        public void NavigateTo(int i)
        {
            var mvs = (IMemoryViewService)sp.GetService(typeof(IMemoryViewService));
            if (mvs == null)
                return;
            mvs.ShowMemoryAtAddress(procs.Keys[i]);
        }
    }
}
