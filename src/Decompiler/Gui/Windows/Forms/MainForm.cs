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

using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public partial class MainForm : Form,
        IMainForm
    {
        private ToolStrip toolBar;

        public MainForm()
        {
            InitializeComponent();
        }

        public void AddToolbar(ToolStrip toolBar)
        {
            this.Controls.Add(toolBar);
            this.toolBar = toolBar;
        }

        public string TitleText
        {
            get { return Text; }
            set { Text = value; }
        }
        
        public ListView BrowserList
        {
            get { return listBrowser; }
        }

        public void BuildPhases()
        {
            throw new NotImplementedException();
        }

        public ListView DiagnosticsList
        {
            get { return listDiagnostics; }
        }

        public ListView FindResultsList
        {
            get { return listFindResults; }
        }

        public OpenFileDialog OpenFileDialog
        {
            get { return ofd; }
        }

        public SaveFileDialog SaveFileDialog
        {
            get { return sfd; }
        }

        public void SetCurrentPage(object oPage)
        {
            Control page = (Control) oPage;
            page.BringToFront();
            page.Visible = true;
        }

        public void SetStatus(string txt)
        {
            lblStatus.Text = txt;
        }

        public void SetStatusDetails(string txt)
        {
            lblStatusDetails.Text = txt;
        }

        public DialogResult ShowDialog(CommonDialog dlg)
        {
            return dlg.ShowDialog(this);
        }

        public DialogResult ShowDialog(Form dlg)
        {
            return dlg.ShowDialog(this);
        }

        public DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(this, message, caption, buttons, icon);
        }

        public ToolStrip ToolBar
        {
            get { return toolBar; }
        }

        public IStartPage InitialPage
        {
            get { return startPage; }
        }

        public ILoadedPage LoadedPage
        {
            get { return loadedPage1; }
        }

        public AnalyzedPage AnalyzedPage
        {
            get { return analyzedPage1; }
        }

        public FinalPage FinalPage
        {
            get { return finalPage1; }
        }

        public ImageList ImageList
        {
            get { return this.imageList; }
        }

        public StatusStrip StatusStrip
        {
            get { return this.statusStrip1; }
        }
    }
}