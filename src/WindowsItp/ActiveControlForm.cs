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
    public partial class ActiveControlForm : Form
    {
        public ActiveControlForm()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
            var form1 = new Form
            {
                Text = "form1",
                MdiParent = this,
            };

            var form2 = new Form
            {
                Text = "form2",
                MdiParent = this,
                Controls = {
                    new TextBox {
                        Text = "Textie",
                        Name = "textbox1",
                    },
                }
            };

            listBox1.Items.AddRange(new object[] 
            {
                "Hello",
                "World",
            });
            form1.Show();
            form2.Show();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var active = this.ActiveControl;
            var activeForm = active.FindForm();
            var activeMdi = this.ActiveMdiChild;
        }
    }
}
