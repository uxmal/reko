using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsItp
{
    public partial class RtfDialog : Form
    {
        public RtfDialog()
        {
            InitializeComponent();
            txtRich.LinkClicked += new LinkClickedEventHandler(txtRich_LinkClicked);
        }

        void txtRich_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string s = e.LinkText;
            s.ToCharArray();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                txtRich.Rtf = txtRtf.Text;
            }
            catch (ArgumentException)
            {
                txtRtf.BackColor = Color.Red;
                timer1.Enabled = true;
                timer1.Interval = 200;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            txtRtf.BackColor = SystemColors.Window;
        }
    }
}
