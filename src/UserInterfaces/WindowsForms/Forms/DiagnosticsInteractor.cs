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
using Reko.Core.Services;
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    /// <summary>
    /// Interactor that controls the Diagnostics pane.
    /// </summary>
    public class DiagnosticsInteractor : IDiagnosticsService, IWindowPane, ICommandTarget
    {
        private IServiceProvider services;
        private ListView listView;
        private List<KeyValuePair<ICodeLocation, Diagnostic>> pending;
        private SynchronizationContext syncCtx;

        public IWindowFrame Frame { get; set; }

        public void Attach(ListView listView)
        {
            if (listView == null)
                throw new ArgumentNullException("listView");
            syncCtx = SynchronizationContext.Current;
            this.listView = listView;
            listView.DoubleClick += listView_DoubleClick;
            listView.HandleCreated += listView_HandleCreated;
        }

        public object CreateControl()
        {
            return this.listView;
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

        public void Close()
        {
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
            if (listView.InvokeRequired)
            {
                this.syncCtx.Post((a) =>
                {
                    AddDiagnosticImpl(location, d);
                }, null);
            }
            else
            {
                AddDiagnosticImpl(location, d);
            }
        }

        private void AddDiagnosticImpl(ICodeLocation location, Diagnostic d)
        {
            var li = new ListViewItem();
            li.Text = location.Text;
            li.Tag = location;
            li.ImageKey = d.ImageKey;
            li.SubItems.Add(d.Message);
            listView.Items.Add(li);
        }

        public void Error(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new ErrorDiagnostic(message));
        }

        public void Error(Exception ex, string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new ErrorDiagnostic(message, ex));
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new ErrorDiagnostic(message));
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            AddDiagnostic(location, new ErrorDiagnostic(string.Format(message, args)));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            AddDiagnostic(location, new ErrorDiagnostic(message, ex));
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            AddDiagnostic(location, new ErrorDiagnostic(string.Format(message, args), ex));
        }

        public void Warn(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new WarningDiagnostic(message));
        }

        public void Warn(string message, params object[] args)
        {
            Warn(string.Format(message, args));
        }

        public void Warn(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new WarningDiagnostic(message));
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Warn(location, string.Format(message, args));
        }

        public void Inform(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new InformationalDiagnostic(message));
        }

        public void Inform(string message, params object[] args)
        {
            AddDiagnostic(
                new NullCodeLocation(""),
                new InformationalDiagnostic(string.Format(message, args)));
        }

        public void Inform(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new InformationalDiagnostic(message));
        }

        public void Inform(ICodeLocation location, string message, params object[] args)
        {
            AddDiagnostic(
                location,
                new InformationalDiagnostic(string.Format(message, args)));
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

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy:
                    status.Status = listView.SelectedIndices.Count > 0
                        ? MenuStatus.Visible | MenuStatus.Enabled
                        : MenuStatus.Visible;
                    return true;
                case CmdIds.EditSelectAll:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                }
            }
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy: return CopyDiagnosticsToClipboard();
                case CmdIds.EditSelectAll: return SelectAll();
                }
            }
            return false;
        }

        public bool CopyDiagnosticsToClipboard()
        {
            var sb = new StringBuilder();
            foreach (int i in listView.SelectedIndices)
            {
                var item = listView.Items[i];
                sb.AppendFormat("{0},{1},{2}", item.ImageKey, item.Text, item.SubItems[1].Text);
                sb.AppendLine();
            }
            Clipboard.SetText(sb.ToString());
            return true;
        }

        public bool SelectAll()
        {
            for (int i = 0; i < listView.Items.Count; ++i)
                listView.SelectedIndices.Add(i);
            return true;
        }
    }
}
