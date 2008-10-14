using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
    public partial class FindDialog : Form
    {
        public FindDialog()
        {
            InitializeComponent();
        }

        public Button FindButton
        {
            get { return btnFind; }
        }

        public TextBox FindText
        {
            get { return txtFindText; }
        }
    }
}