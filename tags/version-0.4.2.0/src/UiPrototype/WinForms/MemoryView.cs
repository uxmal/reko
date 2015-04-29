using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UiPrototype.WinForms
{
    public partial class MemoryView : Form
    {
        public MemoryView()
        {
            InitializeComponent();
        }

        private void setTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if !NO_CONTROL
            var uc = new MemoryTyperControl();
            this.Controls.Add(uc);
            uc.Location = this.PointToClient(Control.MousePosition);
            uc.BackColor = Color.Transparent;
            uc.BringToFront();
            uc.Show();
#else
            var dlg = new TypeDialog();
            dlg.Owner = this;
            dlg.Location = Control.MousePosition;
            dlg.BringToFront();
            dlg.Show();

#endif
        }

        private void MemoryView_Load(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
        }
    }
}
