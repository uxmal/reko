using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsItp
{
    public partial class ItpForm : Form
    {
        public ItpForm()
        {
            InitializeComponent();
        }

        private void memoryControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new MemoryControlDialog())
            {
                dlg.ShowDialog(this);
            }
        }
    }
}
