#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class DecompilerShellUiService :
        DecompilerUiService,
        IDecompilerShellUiService,
        ICommandTarget
    {
        private readonly MainForm form;
        private readonly DecompilerMenus dm;
        private readonly Dictionary<string, WindowFrame> framesByName;
        private readonly Dictionary<TabPage, WindowFrame> framesByTab;
        private readonly IServiceProvider services;

        public DecompilerShellUiService(
            MainForm form,
            DecompilerMenus dm,
            OpenFileDialog ofd,
            SaveFileDialog sfd,
            IServiceProvider sp)
            : base(form, ofd, sfd)
        {
            this.form = form;
            this.dm = dm;
            this.services = sp;
            this.framesByName = new Dictionary<string, WindowFrame>();
            this.framesByTab = new Dictionary<TabPage, WindowFrame>();
            this.form.DocumentTabs.ControlRemoved += documentTabs_ControlRemoved;
            this.form.DocumentTabs.MouseUp += documentTabs_MouseUp;
        }

        public IEnumerable<IWindowFrame> DocumentWindows
        {
            get { return framesByTab.Values; }
        }

        public IEnumerable<IWindowFrame> ToolWindows
        {
            get { throw new NotImplementedException();  }
        }

        public Dictionary<string,Dictionary<int,CommandID>> KeyBindings
        {
            get { return dm.KeyBindings; }
            set { dm.KeyBindings = value; }
        }

        void CloseTab(TabPage page)
        {
            form.DocumentTabs.TabPages.Remove(page);
            framesByTab.Remove(page);
        }

        public IWindowFrame FindWindow(string windowType)
        {
            if (framesByName.TryGetValue(windowType, out WindowFrame frame))
                return frame;
            else
                return null;
        }

        public IWindowFrame CreateWindow(string windowType, string windowTitle, IWindowPane pane)
        {
            var tabPage = new TabPage
            {
                Text = windowTitle,
                ImageIndex = 7,
            };
            WindowFrame frame = new WindowFrame(this, windowType, tabPage, pane);
            framesByName.Add(windowType, frame);
            framesByTab.Add(tabPage, frame);
            this.form.DocumentTabs.TabPages.Add(tabPage);
            this.form.DocumentTabs.SelectedTab = tabPage;
            pane.Frame = frame;
            pane.SetSite(services);
            return frame;
        }

        public IWindowFrame FindDocumentWindow(string documentType, object docItem)
        {
            foreach (var frame in framesByTab.Values)
            {
                if (frame.WindowType == documentType && frame.DocumentItem == docItem)
                    return frame;
            }
            return null;
        }

        public IWindowFrame CreateDocumentWindow(string documentType, object docItem, string documentTitle, IWindowPane pane)
        {
            var tabPage = new TabPage
            {
                Text = documentTitle,
                ImageIndex = 8,
            };
            WindowFrame frame = new WindowFrame(this, documentType, docItem, tabPage, pane);
            framesByTab.Add(tabPage, frame);
            this.form.DocumentTabs.TabPages.Add(tabPage);
            this.form.DocumentTabs.SelectedTab = tabPage;
            pane.Frame = frame;
            pane.SetSite(services);
            return frame;   
        }

        private void RemoveFrame(WindowFrame windowFrame)
        {
            var name = framesByName
                .Where(de => de.Value == windowFrame)
                .Select(de => de.Key)
                .SingleOrDefault();
            if (name is not null)
                framesByName.Remove(name);
            var page = framesByTab
                .Where(de => de.Value == windowFrame)
                .Select(de => de.Key)
                .SingleOrDefault();
            framesByTab.Remove(page);
        }

        public IWindowFrame ActiveFrame
        {
            get
            {
                var activeTab = form.DocumentTabs.SelectedTab;
                if (activeTab is null)
                    return null;
                if (!framesByTab.TryGetValue(activeTab, out WindowFrame frame))
                    return null;
                return frame;
            }
        }

        private ICommandTarget ActiveCommandTarget()
        {
            if (ActiveFrame is not WindowFrame frame)
                return null;
            return frame.Pane as ICommandTarget;
        }

        #region ICommandTarget Members

        /// <summary>
        /// Forward the query to the active IWindowPane -- if it in turn supports ICommandTarget.
        /// </summary>
        /// <param name="cmdSet"></param>
        /// <param name="cmdId"></param>
        /// <param name="status"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            ICommandTarget ct = ActiveCommandTarget();
            if (ct is null)
                return false;
            return ct.QueryStatus(cmdId, status, text);
        }

        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            ICommandTarget ct = ActiveCommandTarget();
            if (ct is null)
                return ValueTask.FromResult(false);
            return ct.ExecuteAsync(cmdId);
        }

        public object GetContextMenu(int menuId)
        {
            var ctxMenu = dm.GetContextMenu(menuId);
            return ctxMenu;
        }

        public virtual void SetContextMenu(object control, int menuId)
        {
            var ctxMenu = dm.GetContextMenu(menuId);
            var ctxMenuProp = control.GetType().GetProperty("ContextMenuStrip");
            ctxMenuProp.SetValue(control, ctxMenu);
        }

        public void SetStatusForMenuItems(IList menuItems)
        {
            dm.SetStatusForMenuItems(menuItems);
        }

        public void WithWaitCursor(Action action)
        {
            var cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                action();
            }
            finally
            {
                Cursor.Current = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Window frames host IWindowPanes.
        /// </summary>
        public class WindowFrame : IWindowFrame
        {
            DecompilerShellUiService svc;
            string key;
            TabPage tabPage;
            IWindowPane pane;
            Control ctrl;

            public WindowFrame(DecompilerShellUiService svc, string key, TabPage tabPage, IWindowPane pane)
            {
                this.svc = svc;
                this.key = key;
                this.tabPage = tabPage;
                this.pane = pane;
            }

            public WindowFrame(DecompilerShellUiService svc, string key, object docItem, TabPage tabPage, IWindowPane pane)
            {
                this.svc = svc;
                this.key = key;
                this.DocumentItem = docItem;
                this.tabPage = tabPage;
                this.pane = pane;
            }

            public string WindowType { get { return key; } }
            public string Title { get { return tabPage.Text; } set { tabPage.Text = value; } }
            public object DocumentItem { get; private set; }

            public void Close()
            {
                svc.CloseTab(tabPage);
                pane.Close();
            }

            public void Show()
            {
                if (ctrl is null)
                {
                    ctrl = (Control) pane.CreateControl();
                    ctrl.Dock = DockStyle.Fill;
                    tabPage.Controls.Add(ctrl);
                }
                tabPage.Show();
                ((TabControl)tabPage.Parent).SelectedTab = tabPage;
            }

            public IWindowPane Pane
            {
                get { return pane; }
            }
        }

        #endregion

        void documentTabs_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Debug.Print("Mouse up at: {0,1}", e.Location);
            for (int i = 0; i < form.DocumentTabs.TabPages.Count; ++i)
            {
                var tabPage = form.DocumentTabs.TabPages[i];
                var tabRect = form.DocumentTabs.GetTabRect(i);
                if (tabRect.Contains(e.Location))
                {
                    var killZone = new Rectangle(tabRect.X + 3, tabRect.Y, 20, tabRect.Height);
                    Debug.Print("Hit tab #{0}; {1} {2}", i, e.Location, killZone.Contains(e.Location) ? "close!" : "", killZone);
                    if (killZone.Contains(e.Location))
                    {
                        WindowFrame frame;
                        if (framesByTab.TryGetValue(tabPage, out frame))
                            frame.Close();
                    }
                }
            }
        }

        void documentTabs_ControlRemoved(object sender, ControlEventArgs e)
        {
            WindowFrame frame;
            if (!framesByTab.TryGetValue((TabPage)e.Control, out frame))
                return;
            RemoveFrame(frame);
        }
    }
}
