using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UiPrototype.WinForms
{
    public partial class MemoryTyperControl : UserControl
    {
        private (string, string, string)[] tuples;
        private int step;

        public MemoryTyperControl()
        {
            InitializeComponent();

            tuples = new ValueTuple<string, string, string>[] {
                ("", "", ""),
                ("a", "array", "g00410000[]"),
                ("ap", "array of pointer", "__ * g00410000[]"),
                ("apf", "array of pointer to bool", "bool *g00410000[]"),
                ("apfn", "array of pointer to function", "void (*g00410000[])()"),
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
    }
}
