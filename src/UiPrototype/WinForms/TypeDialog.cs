using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UiPrototype.WinForms
{
    public partial class TypeDialog : Form
    {
        private Tuple<string, string, string>[] tuples;
        private int step;

        public TypeDialog()
        {
            InitializeComponent();

            tuples = new Tuple<string, string, string>[] {
                Tuple.Create("", "", ""),
                Tuple.Create("a", "array", "g00410000[]"),
                Tuple.Create("ap", "array of pointer", "__ * g00410000[]"),
                Tuple.Create("apf", "array of pointer to bool", "bool *g00410000[]"),
                Tuple.Create("apfn", "array of pointer to function", "void (*g00410000[])()"),
            };
            step = 0;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (step >= tuples.Length)
                return;
            var t = tuples[step];
            textBox1.Text = t.Item1;
            textBox1.SelectionStart = textBox1.TextLength;
            var sb = new StringBuilder();
            sb.AppendLine(t.Item2);
            sb.AppendLine();
            sb.AppendLine(t.Item3);
            label1.Text = sb.ToString();
            ++step;
        }

        private void UserControl1_VisibleChanged(object sender, EventArgs e)
        {
            timer.Start();
            textBox1.Focus();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            var parent = this.ParentForm;
            if (parent is not null)
                parent.Controls.Remove(this);
            timer.Stop();
        }

        private void TypeDialog_Leave(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
