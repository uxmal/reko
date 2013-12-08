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
    public partial class PatternSearchDialog : Form
    {
        public PatternSearchDialog()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(new object[] {
                new Wrapper("Hex-encoded binary data"),
                new Wrapper(Encoding.ASCII),
                new Wrapper(Encoding.GetEncoding("ISO_8859-1")),
                new Wrapper(Encoding.UTF8),
                new Wrapper(Encoding.GetEncoding("UTF-16LE")),
                new Wrapper(Encoding.GetEncoding("UTF-16BE"))
            });
            comboBox1.SelectedIndex = 0;
        }

        public StringSearcher CreateStringSearcher()
        {
            return new StringSearcher(textBox1.Text);
        }

        private class Wrapper
        {
            private string name;
            private Encoding e;

            public Wrapper(string name)
            {
                this.name = name;
            }

            public Wrapper(Encoding e)
            {
                this.e = e;
                this.name = e.WebName.ToUpper() + "-encoded string";
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}
