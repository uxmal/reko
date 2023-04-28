#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using Keys = System.Windows.Forms.Keys;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Reko.Gui.Services;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class MainForm :
        Form,
        IMainForm
    {
        private MainFormInteractor interactor;
        private ToolStrip toolBar;
        private ToolStrip projectToolBar;
        private DocumentWindowCollection docWindows;
        private DecompilerMenus dm;
        private DecompilerShellUiService uiSvc;

        public MainForm()
        {
            InitializeComponent();
            docWindows = new DocumentWindowCollection(this);
            ProjectBrowser = new TreeViewWrapper(treeBrowser);
            ProjectBrowserTab = new TabPageWrapper(tabProject);
            CallGraphNavigatorView = this.callGraphNavigatorView;

            this.Load += MainForm_Load;
            this.ProcessCommandKey += this.MainForm_ProcessCommandKey;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var mainToolbar = dm.GetToolStrip(MenuIds.MainToolbar);
            mainToolbar.Text = "";
            mainToolbar.ImageList = this.ImageList;
            var projectToolbar = dm.GetToolStrip(MenuIds.ProjectBrowserToolbar);
            projectToolbar.ImageList = this.ImageList;
            this.AddToolbar(mainToolbar);
            this.AddMenuStrip(dm.GetMenu(MenuIds.MainMenu));
            this.AddProjectBrowserToolbar(projectToolbar);

            this.ToolBar.ItemClicked += toolBar_ItemClicked;
            this.ProjectBrowserToolbar.ItemClicked += toolBar_ItemClicked;

            this.treeBrowser.DragEnter += projectBrowser_DragEnter;
            this.treeBrowser.DragOver += projectBrowser_DragOver;
            this.treeBrowser.DragDrop += projectBrowser_DragDrop;

            interactor.Attach(this);
        }

        public void Attach(IServiceContainer services)
        {
            this.interactor = new MainFormInteractor(services);
            this.dm = new DecompilerMenus(new CommandDefinitions(), interactor);
            this.uiSvc = new DecompilerShellUiService(this, dm, this.OpenFileDialog, this.SaveFileDialog, services);
            services.AddService(typeof(IDecompilerShellUiService), this.uiSvc);

            this.DataBindings.Add(nameof(Text), interactor, nameof(MainFormInteractor.TitleText));
        }

        private async void MainForm_ProcessCommandKey(object sender, KeyEventArgs e)
        {
            var frame = uiSvc.ActiveFrame;
            if (frame is not null)
            {
                if (frame.Pane is ICommandTarget ct)
                {
                    e.Handled = await dm.ProcessKey(ct.GetType().FullName, ct, e.KeyData);
                    if (e.Handled)
                        return;
                }
            }
            e.Handled = await dm.ProcessKey("", this.interactor, e.KeyData);
        }

        private async void toolBar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag is not MenuCommand cmd)
                throw new NotImplementedException("Button not hooked up.");
            await interactor.ExecuteAsync(cmd.CommandID);
        }

        #region IMainForm Members

        public void AddMenuStrip(MenuStrip menu)
        {
            this.Controls.Add(menu);
            this.MainMenuStrip = menu;
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
            get { return tabFindResults; }
        }

        public TabPage DiagnosticsPage
        {
            get { return tabDiagnostics; }
        }


        public TabPage ConsolePage
        {
            get { return tabConsole; }
        }

        public ITabPage ProjectBrowserTab { get; }

        public TabPage ProcedureListTab
        {
            get { return tabProcedures; }
        }

        public ProcedureListPanel ProcedureListPanel => procedureListPanel;

        public ListView DiagnosticsList
        {
            get { return listDiagnostics; }
        }

        public ToolStripButton DiagnosticsFilter
        {
            get { return btnDiagnosticFilter; }
        }

        public new Gui.Forms.FormWindowState WindowState
        {
            get { return (Gui.Forms.FormWindowState) base.WindowState; }
            set { base.WindowState = (System.Windows.Forms.FormWindowState) value; }
        }

        public ITreeView ProjectBrowser { get; set; }

        public CallGraphNavigatorView CallGraphNavigatorView { get; set; }

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
            LayoutMdi((MdiLayout) ((int) layout));
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

        public ToolStripComboBox OutputWindowSources => ddlOutputWindowSources;

        public Panel OutputWindowPanel => this.outputWindowPanel;

        public Gui.Services.DialogResult ShowDialog(CommonDialog dialog)
        {
            return (Gui.Services.DialogResult) dialog.ShowDialog(this);
        }

        public Gui.Services.DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return (Gui.Services.DialogResult) MessageBox.Show(this, message, caption, buttons, icon);
        }

        public void UpdateToolbarState()
        {
            var status = new CommandStatus();
            var text = new CommandText("&");
            foreach (ToolStripItem item in ToolBar.Items)
            {
                if (item.Tag is MenuCommand cmd)
                {
                    text.Text = null;
                    var st = interactor.QueryStatus(cmd.CommandID, status, text);
                    item.Enabled = st && (status.Status & MenuStatus.Enabled) != 0;
                    if (!string.IsNullOrEmpty(text.Text))
                        item.Text = text.Text;
                }
            }
        }

        public ToolStripLabel SelectedAddressLabel => this.statusLblSelection;

        public StatusStrip StatusStrip => statusStrip;

        public event KeyEventHandler ProcessCommandKey;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var eh = ProcessCommandKey;
            if (eh is not null)
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

        private void projectBrowser_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = e.AllowedEffect & System.Windows.Forms.DragDropEffects.Copy;
            else
                e.Effect = System.Windows.Forms.DragDropEffects.None;
        }

        private void projectBrowser_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = e.AllowedEffect & System.Windows.Forms.DragDropEffects.Copy;
            else
                e.Effect = System.Windows.Forms.DragDropEffects.None;
        }

        private async void projectBrowser_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var o = e.Data.GetData(DataFormats.FileDrop);
                var filenames = (string[]) o;
                if (filenames is null || filenames.Length == 0)
                    return;
                await interactor.OpenBinary(filenames[0]);
                System.Diagnostics.Debug.WriteLine("Done");
            }
        }

        private void tree_MouseWheel(object sender, Gui.Controls.MouseEventArgs e)
        {
            //model.MoveTo(model.CurrentPosition, (e.Delta < 0 ? 1 : -1));
            //RecomputeLayout();
            //OnScroll();
            //tree,Invalidate();
        }

        private class DocumentWindowCollection : ICollection<IWindowFrame>
        {
            private readonly MainForm mainForm;

            public DocumentWindowCollection(MainForm mainForm)
            {
                this.mainForm = mainForm;
            }

            public void Add(IWindowFrame item)
            {
                ((Form) item).MdiParent = mainForm;
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
                    if (item == (IWindowFrame) form)
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
