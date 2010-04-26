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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public partial class MainForm :
        Form,
        IMainForm
    {
        private ToolStrip toolBar;

        public MainForm()
        {
            InitializeComponent();
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

        public ListView BrowserList
        {
            get { return listBrowser; }
        }

        public ImageList ImageList
        {
            get { return imageList; }
        }

        public ListView FindResultsList
        {
            get { return listFindResults; }
        }

        public ListView DiagnosticsList
        {
            get { return listDiagnostics; }
        }

        public OpenFileDialog OpenFileDialog
        {
            get { return ofd; }
        }

        public SaveFileDialog SaveFileDialog
        {
            get { return sfd; }
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

        public DialogResult ShowDialog(Form dialog)
        {
            throw new NotImplementedException();
        }

        public DialogResult ShowDialog(CommonDialog dialog)
        {
            return dialog.ShowDialog(this);
        }

        public DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(this, message, caption, buttons, icon);
        }

        public void SetCurrentPage(object page)
        {
            throw new NotImplementedException();
        }

        public StatusStrip StatusStrip
        {
            get { return statusStrip; }
        }

        #endregion
        
    }
}
