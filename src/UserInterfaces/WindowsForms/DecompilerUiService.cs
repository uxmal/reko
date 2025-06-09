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
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DialogResult = Reko.Gui.Services.DialogResult;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Windows Forms implementation of the IDecompilerUIService service.
    /// </summary>
    public class DecompilerUiService : IDecompilerUIService
    {
        private readonly Form form;
        private readonly OpenFileDialog ofd;
        private readonly SaveFileDialog sfd;

        public DecompilerUiService(Form form, OpenFileDialog ofd, SaveFileDialog sfd)
        {
            this.form = form;
            this.ofd = ofd;
            this.sfd = sfd;
        }

        #region IDecompilerUIService Members

        public ValueTask<bool> Prompt(string prompt)
        {
            DialogResult dlgr = DialogResult.No;
            form.Invoke(new Action(
                () => { dlgr = (DialogResult)MessageBox.Show(prompt, "Reko Decompiler", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }));
            return ValueTask.FromResult(dlgr == DialogResult.Yes);
        }

        private ValueTask<DialogResult> ShowModalDialog(Form dlg)
        {
            return ValueTask.FromResult((Gui.Services.DialogResult)
                form.Invoke(new Func<DialogResult>(delegate()
                {
                    return (DialogResult)dlg.ShowDialog(form);
                })));
        }

        public virtual ValueTask<DialogResult> ShowModalDialog(IDialog dlg)
        {
            return ShowModalDialog((Form)dlg);
        }

        public virtual async ValueTask<T> ShowModalDialog<T>(IDialog<T> dlg)
        {
            await ShowModalDialog((Form) dlg);
            return dlg.Value;
        }

        public virtual ValueTask<string> ShowOpenFileDialog(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                ofd.FileName = fileName;
            if ((DialogResult)ofd.ShowDialog(form) == DialogResult.OK)
            {
                return ValueTask.FromResult(ofd.FileName);
            }
            else
                return ValueTask.FromResult<string>(null);
        }

        public virtual ValueTask<string> ShowSaveFileDialog(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                sfd.FileName = Path.GetFileName(fileName);
                sfd.InitialDirectory = Path.GetDirectoryName(fileName);
            }
            if ((DialogResult)sfd.ShowDialog(form) == DialogResult.OK)
            {
                return ValueTask.FromResult(sfd.FileName);
            }
            else
                return ValueTask.FromResult<string>(null);
        }

        public virtual ValueTask ShowError(Exception ex, string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(format, args);
            Exception e = ex;
            while (e is not null)
            {
                sb.Append(" ");
                sb.Append(e.Message);
                e = e.InnerException;
            }
            form.Invoke(new Action<string>(delegate(string s)
                { MessageBox.Show(form, s, "Reko decompiler", MessageBoxButtons.OK, MessageBoxIcon.Error); }),
                sb.ToString() );
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask ShowError(string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(format, args);
            form.Invoke(new Action<StringBuilder>(delegate(StringBuilder s)
                { MessageBox.Show(form, s.ToString(), "Reko decompiler", MessageBoxButtons.OK, MessageBoxIcon.Error); }),
                sb);
            return ValueTask.CompletedTask;
        }
        #endregion

        public ValueTask ShowMessage(string msg)
        {
            form.Invoke(new Action<string>(delegate(string s)
                { MessageBox.Show(form, s, "Reko decompiler", MessageBoxButtons.OK, MessageBoxIcon.Information); }),
                msg);
            return ValueTask.CompletedTask;
        }
    }
}