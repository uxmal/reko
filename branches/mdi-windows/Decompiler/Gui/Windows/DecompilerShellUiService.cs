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
    public class DecompilerShellUiService : DecompilerUiService, IDecompilerShellUiService
    {
        private Form form;
        private DecompilerMenus dm;
        private Dictionary<string, WindowFrame> frames;
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
            this.frames = new Dictionary<string,WindowFrame>();
        }

        public virtual ContextMenu GetContextMenu(int menuId)
        {
            return dm.GetContextMenu(menuId);
        }

        public IWindowFrame FindWindow(string windowType)
        {
            WindowFrame frame;
            if (frames.TryGetValue(windowType, out frame))
                return frame;
            else 
                return null;
        }

        public IWindowFrame CreateWindow(string windowType, string windowTitle, IWindowPane pane)
        {
            Form mdiForm = new Form();
            mdiForm.Text = windowTitle;
            WindowFrame frame = new WindowFrame(this, windowType, mdiForm, pane);
            frames.Add(windowType, frame);
            pane.SetSite(sp);
            mdiForm.MdiParent = form;
            return frame;
        }

        public class WindowFrame : IWindowFrame
        {
            DecompilerShellUiService svc;
            string key;
            Form form;
            IWindowPane pane;

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
                svc.frames.Remove(key);
            }

            public void Show()
            {
                Control ctrl = pane.CreateControl();
                ctrl.Dock = DockStyle.Fill;
                form.Controls.Add(ctrl);
                form.Show();
            }
        }
    }
}
