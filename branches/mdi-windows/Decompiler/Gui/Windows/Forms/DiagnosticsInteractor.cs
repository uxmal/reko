/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public class DiagnosticsInteractor : IDiagnosticsService
    {
        private ListView listView;

        public void Attach(ListView listView)
        {
            this.listView = listView;
        }

        #region IDiagnosticsService Members

        public void AddDiagnostic(Diagnostic d)
        {
            ListViewItem li = new ListViewItem();
            li.Text = d.ToString();
            ListViewItem.ListViewSubItem si = li.SubItems.Add(d.Address != null
                ? d.Address.ToString()
                : "");
            si.Tag = d.Address;
            li.SubItems.Add(d.Message);
            this.listView.Items.Add(li);
        }

        public void ClearDiagnostics()
        {
            listView.Items.Clear();
        }
        #endregion
    }
}
