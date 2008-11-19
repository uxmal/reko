/* 
 * Copyright (C) 1999-2008 John Källén.
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

namespace Decompiler.WindowsGui.Forms
{
    public partial class MainForm : Form,
        IMainForm
    {
        private ProgressBarWrapper wrProgress;
        private ToolStrip toolBar;

        public MainForm()
        {
            InitializeComponent();
            wrProgress = new ProgressBarWrapper(progressStatus);
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
        
        //$REFACTOR: this should be a service.
        public void AddDiagnostic(Decompiler.Core.Diagnostic d, string format, params object[] args)
        {
            ListViewItem li = new ListViewItem();
            li.SubItems.Add(string.Format(format, args));
            this.listDiagnostics.Items.Add(li);
        }

        public ListView BrowserList
        {
            get { return listBrowser; }
        }

        public void BuildPhases()
        {
            throw new Exception("The method or operation is not implemented.");
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

        public IProgressBar ProgressBar
        {
            get { return wrProgress; }
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

        public void ShowMessageBox(string message, string caption)
        {
            MessageBox.Show(this, message, caption);
        }


        public ToolStrip ToolBar
        {
            get { return toolBar; }
        }

        public IStartPage StartPage
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

        private class ProgressBarWrapper : IProgressBar
        {
            private ToolStripProgressBar bar;
            public ProgressBarWrapper(ToolStripProgressBar bar)
            {
                this.bar = bar;
            }
            #region IProgressBar Members

            public int Value
            {
                get { return bar.Value; }
                set { bar.Value = Value; }
            }

            public int Minimum
            {
                get { return bar.Minimum; }
                set { bar.Minimum = Value; }
            }

            public int Maximum
            {
                get { return bar.Maximum; }
                set { bar.Maximum = Value; }
            }

            #endregion
        }

    }
}