using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class DecoderPerformanceDialog : Form
    {
        public DecoderPerformanceDialog()
        {
            InitializeComponent();
        }

        private async void btnDoit_Click(object sender, EventArgs e)
        {
            lblTest.Text = "Running...";
            try
            {
                await Task.Run(runTest())
            } catch
            {

            }
        }

        private Task RunTest()
        {
            return Task
        }

    }
}
