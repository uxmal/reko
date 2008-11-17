using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
    public partial class StartPage : UserControl, IStartPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        #region IStartPage Members

        public TextBox AssemblerFile
        {
            get { return txtAssembler; }
        }

        public Button BrowseInputFile
        {
            get { return btnBrowseInputFile;  }
        }

        public TextBox HeaderFile
        {
            get { return txtTypes; }
        }

        public TextBox InputFile
        {
            get { return txtInputFile; }
        }

        public TextBox IntermediateFile
        {
            get { return txtIL; }
        }

        public bool IsDirty
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public event EventHandler IsDirtyChanged;

        public TextBox LoadAddress
        {
            get { return txtLoadAddress; }
        }

        public TextBox SourceFile
        {
            get { return txtSource; }
        }

        #endregion
    }
}
