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
    public partial class MainForm2 : Form,
        IMainForm
    {
        private ProgressBarWrapper wrProgress;

        public MainForm2()
        {
            InitializeComponent();
            wrProgress = new ProgressBarWrapper(progressStatus);
        }

        public string TitleText
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
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

        public ListView FindResultsList
        {
            get { return listFindResults; }
        }

        public OpenFileDialog OpenFileDialog
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IProgressBar ProgressBar
        {
            get { return wrProgress; }
        }

        public SaveFileDialog SaveFileDialog
        {
            get { throw new Exception("The method or operation is not implemented."); }
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
            get { return toolbar; }
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