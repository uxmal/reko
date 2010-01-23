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
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class DecompilerShellUiService : 
        DecompilerUiService, 
        IDecompilerShellUiService,
        ICommandTarget
    {
        private Form form;
        private DecompilerMenus dm;
        private Dictionary<string, WindowFrame> framesByName;
        private Dictionary<Form, WindowFrame> framesByForm;
        private IServiceProvider sp;

        public DecompilerShellUiService(
            Form form,
            DecompilerMenus dm,
            OpenFileDialog ofd,
            SaveFileDialog sfd,
            IServiceProvider sp)
            : base(form, ofd, sfd)
        {
            this.form = form;
            this.dm = dm;
            this.sp = sp;
            this.framesByName = new Dictionary<string,WindowFrame>();
            this.framesByForm = new Dictionary<Form, WindowFrame>();
        }

        public virtual ContextMenu GetContextMenu(int menuId)
        {
            return dm.GetContextMenu(menuId);
        }

        public IWindowFrame FindWindow(string windowType)
        {
            WindowFrame frame;
            if (framesByName.TryGetValue(windowType, out frame))
                return frame;
            else 
                return null;
        }

        public IWindowFrame CreateWindow(string windowType, string windowTitle, IWindowPane pane)
        {
            Form mdiForm = new Form();
            mdiForm.Text = windowTitle;
            WindowFrame frame = new WindowFrame(this, windowType, mdiForm, pane);
            framesByName.Add(windowType, frame);
            framesByForm.Add(mdiForm, frame);
            pane.SetSite(sp);
            mdiForm.MdiParent = form;
            return frame;
        }

        private void RemoveFrame(WindowFrame windowFrame, string key, Form form)
        {
            framesByName.Remove(key);
            framesByForm.Remove(form);
        }

        private ICommandTarget ActiveMdiCommandTarget()
        {
            var activeMdiForm = form.ActiveMdiChild;
            if (activeMdiForm == null)
                return null;
            WindowFrame frame;
            if (!framesByForm.TryGetValue(activeMdiForm, out frame))
                return null;
            return frame.Pane as ICommandTarget;
        }


        #region ICommandTarget Members

        /// <summary>
        /// Forward the query to the active pane if it also supports ICommandTarget.
        /// </summary>
        /// <param name="cmdSet"></param>
        /// <param name="cmdId"></param>
        /// <param name="status"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
        {
            ICommandTarget ct = ActiveMdiCommandTarget();
            if (ct == null)
                return false;
            return ct.QueryStatus(ref cmdSet, cmdId, status, text);
        }


        public bool Execute(ref Guid cmdSet, int cmdId)
        {
            ICommandTarget ct = ActiveMdiCommandTarget();
            if (ct == null)
                return false;
            return ct.Execute(ref cmdSet, cmdId);
        }

        public class WindowFrame : IWindowFrame
        {
            DecompilerShellUiService svc;
            string key;
            Form form;
            IWindowPane pane;
            Control ctrl;

            public WindowFrame(DecompilerShellUiService svc, string key, Form form, IWindowPane pane)
            {
                this.svc = svc;
                this.key = key;
                this.form = form;
                this.pane = pane;
                this.form.FormClosed += new FormClosedEventHandler(form_FormClosed);
            }

            public void Close()
            {
                form.Close();
            }

            void form_FormClosed(object sender, FormClosedEventArgs e)
            {
                pane.Close();
                svc.RemoveFrame(this, key, form);
            }

            public void Show()
            {
                if (ctrl == null)
                {
                    ctrl = pane.CreateControl();
                    ctrl.Dock = DockStyle.Fill;
                    form.Controls.Add(ctrl);
                }
                form.Show();
            }

            public IWindowPane Pane
            {
                get { return pane; }
            }
        }


        #endregion
    }
}
