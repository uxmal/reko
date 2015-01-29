#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Services;
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public class DiagnosticsInteractor : IDiagnosticsService
    {
        private ListView listView;
        private List<KeyValuePair<ICodeLocation, Diagnostic>> pending;

        public void Attach(ListView listView)
        {
            if (listView == null)
                throw new ArgumentNullException("listView");
            this.listView = listView;
            listView.DoubleClick += listView_DoubleClick;
            listView.HandleCreated += listView_HandleCreated;
        }

        void listView_HandleCreated(object sender, EventArgs e)
        {
            if (pending != null)
            {
                foreach (var d in pending)
                    AddDiagnostic(d.Key, d.Value);
            }
        }

        #region IDiagnosticsService Members

        public void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            if (!listView.IsHandleCreated)
            {
                if (pending == null)
                    pending = new List<KeyValuePair<ICodeLocation, Diagnostic>>();
                pending.Add(new KeyValuePair<ICodeLocation, Diagnostic>(location, d));
                return;
            }

            // This may be called from a worker thread, so we have to be careful to 
            // call the listView on the UI thread.
            listView.Invoke(new Action(() =>
            {
                var li = new ListViewItem();
                li.Text = location.Text;
                li.Tag = location;
                li.ImageKey = d.ImageKey;
                li.SubItems.Add(d.Message);
                listView.Items.Add(li);
            }));
        }

        public void Error(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new ErrorDiagnostic(message));
        }

        public void Warn(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new WarningDiagnostic(message));
        }

        public void ClearDiagnostics()
        {
            listView.Items.Clear();
        }
        #endregion

        public virtual ListViewItem FocusedListItem
        {
            get { return listView.FocusedItem; }
            set { listView.FocusedItem = value; }
        }

        protected void listView_DoubleClick(object sender, EventArgs e)
        {
            var item = FocusedListItem;
            if (item == null)
                return;
            var location = item.Tag as ICodeLocation;
            if (location != null)
                location.NavigateTo();
        }
    }
}
