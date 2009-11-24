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

using Decompiler.Gui;
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Windows Forms implementation of the IDecompulerUIService service.
    /// </summary>
    public class DecompilerUiService : IDecompilerUIService
    {
        private Form mainForm;
        private DecompilerMenus dm;
        private OpenFileDialog ofd;
        private SaveFileDialog sfd;

        public DecompilerUiService(Form mainForm, DecompilerMenus dm, OpenFileDialog ofd, SaveFileDialog sfd)
        {
            this.mainForm = mainForm;
            this.dm = dm;
            this.ofd = ofd;
            this.sfd = sfd;
        }

        #region IDecompilerUIService Members

        public virtual ContextMenu GetContextMenu(int menuId)
        {
            return dm.GetContextMenu(menuId);
        }

        public virtual DialogResult ShowModalDialog(Form dlg)
        {
            return (DialogResult)
                mainForm.Invoke(new Converter<Form, DialogResult>(delegate(Form dlgToShow)
                {
                    return dlgToShow.ShowDialog(mainForm);
                }), dlg);
        }

        public virtual string ShowOpenFileDialog(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                ofd.FileName = fileName;
            if (ofd.ShowDialog(mainForm) == DialogResult.OK)
            {
                return ofd.FileName;
            }
            else
                return null;
        }

        public virtual string ShowSaveFileDialog(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                sfd.FileName = fileName;
            if (sfd.ShowDialog(mainForm) == DialogResult.OK)
            {
                return sfd.FileName;
            }
            else
                return null;
        }

        public virtual void ShowError(Exception ex, string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(format, args);
            Exception e = ex;
            while (e != null)
            {
                sb.Append(" ");
                sb.Append(e.Message);
                e = e.InnerException;
            }
            MessageBox.Show(mainForm, sb.ToString(), "Decompiler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

		public virtual void ShowError(string format, params object [] args)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(format, args);
            MessageBox.Show(mainForm, sb.ToString(), "Decompiler");
		}
        #endregion
    }
}
