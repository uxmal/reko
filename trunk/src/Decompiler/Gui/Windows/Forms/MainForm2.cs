using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public partial class MainForm2 :
        Form,
        IMainForm
    {
        private int childFormNumber = 0;

        public MainForm2()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        #region IMainForm Members

        public string TitleText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void AddToolbar(ToolStrip toolStrip)
        {
            throw new NotImplementedException();
        }

        public void BuildPhases()
        {
            throw new NotImplementedException();
        }

        public ListView BrowserList
        {
            get { throw new NotImplementedException(); }
        }

        public ImageList ImageList
        {
            get { return imageList; }
        }

        public ListView FindResultsList
        {
            get { throw new NotImplementedException(); }
        }

        public ListView DiagnosticsList
        {
            get { throw new NotImplementedException(); }
        }

        public OpenFileDialog OpenFileDialog
        {
            get { throw new NotImplementedException(); }
        }

        public SaveFileDialog SaveFileDialog
        {
            get { throw new NotImplementedException(); }
        }

        public void SetStatus(string txt)
        {
            throw new NotImplementedException();
        }

        public void SetStatusDetails(string txt)
        {
            throw new NotImplementedException();
        }

        public ToolStrip ToolBar
        {
            get { throw new NotImplementedException(); }
        }

        public IStartPage InitialPage
        {
            get { throw new NotImplementedException(); }
        }

        public ILoadedPage LoadedPage
        {
            get { throw new NotImplementedException(); }
        }

        public AnalyzedPage AnalyzedPage
        {
            get { throw new NotImplementedException(); }
        }

        public FinalPage FinalPage
        {
            get { throw new NotImplementedException(); }
        }

        public DialogResult ShowDialog(Form dialog)
        {
            throw new NotImplementedException();
        }

        public DialogResult ShowDialog(CommonDialog dialog)
        {
            throw new NotImplementedException();
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
