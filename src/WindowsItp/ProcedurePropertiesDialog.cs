using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class ProcedurePropertiesDialog : Form
    {
        public ProcedurePropertiesDialog()
        {
            InitializeComponent();
            this.Deactivate += ProcedurePropertiesDialog_Deactivate;
        }

        private void ProcedurePropertiesDialog_Deactivate(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
