using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsItp
{
    public partial class MemoryControlDialog : Form
    {
        public MemoryControlDialog()
        {
            InitializeComponent();
        }

        private void chkShowData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowData.Checked)
            {
                var img = new ProgramImage(new Address(0x0100000), new byte[256]);
                memoryControl1.ProgramImage = img;
            }
            else
            {
                memoryControl1.ProgramImage = null;
            }
        }
    }
}
