using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.Arch.X86.Design
{
    public partial class GoofyForm : Form
    {
        public GoofyForm()
        {
            InitializeComponent();
            new GoofyInteractor().Attach(this);
        }

        public object Value { get; set; }

        public TextBox GoofyTextBox { get { return txtGoofy; } }

        private class GoofyInteractor
        {
            private GoofyForm form;

            internal void Attach(GoofyForm goofyForm)
            {
                this.form = goofyForm;
                form.Load += Form_Load;
                form.FormClosing += Form_FormClosing;
            }

            private void Form_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (form.DialogResult == DialogResult.OK)
                {
                    form.Value = form.GoofyTextBox.Text;
                }
            }

            private void Form_Load(object sender, EventArgs e)
            {
                form.GoofyTextBox.Text = (form.Value ?? "").ToString();
            }
        }

    }
}
