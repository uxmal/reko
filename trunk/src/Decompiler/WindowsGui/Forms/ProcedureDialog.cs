using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
    public partial class ProcedureDialog : Form
    {
        public ProcedureDialog()
        {
            InitializeComponent();
        }

        public TextBox ProcedureName
        {
            get { return txtName; }
        }

        public ListView ArgumentList
        {
            get { return listArguments; }
        }

        public PropertyGrid ArgumentProperties
        {
            get { return propArgument; }
        }
    }
}