#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Forms
{
    public partial class MainForm :
        Form,
        IMainForm
    {
        private ToolStrip toolBar;
        private ToolStrip projectToolBar;
        private DocumentWindowCollection docWindows;

        public MainForm()
        {
            InitializeComponent();
            docWindows = new DocumentWindowCollection(this);
            ProjectBrowser = new TreeViewWrapper(treeBrowser);
        }

        #region IMainForm Members

        public string TitleText
        {
            get { return Text; }
            set { Text = value; }
        }

        public void AddToolbar(ToolStrip toolBar)
        {
            this.Controls.Add(toolBar);
            this.toolBar = toolBar;
        }

        public void AddProjectBrowserToolbar(ToolStrip toolBar)
        {
            this.treeBrowser.Parent.Controls.Add(toolBar);
            this.projectToolBar = toolBar;
        }

        public TabControl DocumentTabs
        {
            get { return tabDocuments; }
        }

        public TabControl TabControl
        {
            get { return tabControl1; }
        }

        public ICollection<IWindowFrame> DocumentWindows { get { return docWindows; } }

        public ImageList ImageList
        {
            get { return imageList; }
        }

        public ListView FindResultsList
        {
            get { return listFindResults; }
        }

        public TabPage FindResultsPage
        {
            get {return tabFindResults; }
        }

        public TabPage DiagnosticsPage
        {
            get { return tabDiagnostics; }
        }

        public TabPage ConsolePage
        {
            get { return tabConsole; }
        }

        public ListView DiagnosticsList
        {
            get { return listDiagnostics; }
        }

        public ITreeView ProjectBrowser { get; set; }

        public OpenFileDialog OpenFileDialog
        {
            get { return ofd; }
        }

        public SaveFileDialog SaveFileDialog
        {
            get { return sfd; }
        }

        public void LayoutMdi(DocumentWindowLayout layout)
        {
            LayoutMdi ((MdiLayout) ((int) layout));
        }

        public void SetStatus(string txt)
        {
            this.StatusStrip.Items[0].Text = txt;
        }

        public void SetStatusDetails(string txt)
        {
            throw new NotImplementedException();
        }

        public ToolStrip ToolBar
        {
            get { return toolBar; }
        }

        public ToolStrip ProjectBrowserToolbar
        {
            get { return projectToolBar; }
        }

        public DialogResult ShowDialog(CommonDialog dialog)
        {
            return (DialogResult)dialog.ShowDialog(this);
        }

        public DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return (DialogResult)MessageBox.Show(this, message, caption, buttons, icon);
        }

        public void SetCurrentPage(object page)
        {
            throw new NotImplementedException();
        }

        public StatusStrip StatusStrip
        {
            get { return statusStrip; }
        }

        public event KeyEventHandler ProcessCommandKey;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var eh = ProcessCommandKey;
            if (eh != null)
            {
                var e = new KeyEventArgs(keyData);
                eh(this, e);
                if (e.Handled)
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        public void CloseAllDocumentWindows()
        {
            DocumentWindows.Clear();
        }

        private class DocumentWindowCollection : ICollection<IWindowFrame>
        {
            private MainForm mainForm;

            public DocumentWindowCollection(MainForm mainForm)
            {
                this.mainForm = mainForm;
            }

            public void Add(IWindowFrame item)
            {
                ((Form)item).MdiParent = mainForm;
            }

            public void Clear()
            {
                foreach (Form docWindow in mainForm.MdiChildren)
                {
                    docWindow.Close();
                }
            }

            public bool Contains(IWindowFrame item)
            {
                return mainForm.MdiChildren.OfType<IWindowFrame>().Contains(item);
            }

            public void CopyTo(IWindowFrame[] array, int arrayIndex)
            {
                
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return mainForm.MdiChildren.Length; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(IWindowFrame item)
            {
                foreach (var form in mainForm.MdiChildren)
                {
                    if (item == (IWindowFrame)form)
                    {
                        form.Close();
                        return true;
                    }
                }
                return false;
            }

            public IEnumerator<IWindowFrame> GetEnumerator()
            {
                return mainForm.MdiChildren.OfType<IWindowFrame>().GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
