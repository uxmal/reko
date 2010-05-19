using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsItp
{
    public partial class WebDialog : Form
    {
        public WebDialog()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            var doc = webBrowser1.Document;
            var mem = new MemoryStream();
            var writer = new StreamWriter(mem);
            writer.Write(textBox1.Text);
            writer.Flush();
            webBrowser1.DocumentStream = mem;
        }
    }
}
