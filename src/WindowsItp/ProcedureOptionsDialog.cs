using Reko.Core;
using Reko.Core.Configuration;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Reko.WindowsItp
{
    public partial class ProcedureOptionsDialog : Form
    {
        private Dictionary<string, string> values;
        public ProcedureOptionsDialog()
        {
            InitializeComponent();
            var options = new List<PropertyOption>
            {
                new PropertyOption { Name="noText"},
                new PropertyOption { Name="hasText", Text="Has Text", Description = "Has a description", },
                new PropertyOption { Name="Choices", Choices = new []
                    {
                        new ListOption_v1 { Text = "Intel 8086", Value = "8086" },
                        new ListOption_v1 { Value="80286"}
                    }
                },
                new PropertyOption { Name = "Custom", TypeName = typeof(CustomDialog).AssemblyQualifiedName },
            };

            values = new Dictionary<string, string>
            {
                { "noText", "initial garbage" }
            };
            var adapter = new PropertyOptionsGridAdapter(values, options);

            propertyGrid1.SelectedObject = adapter;
            propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            textBox1.Text = string.Format("{{ {0} }}",
                string.Join(
                    ",\r\n",
                    values.OrderBy(de => de.Key)
                        .Select(de => $"\"{de.Key}\": \"{de.Value}\"")));
        }

        class CustomDialog : Form
        {
            private TextBox textBox;
            private Button buttonOK;

            public CustomDialog()
            {
                this.ShowInTaskbar = false;
                this.MinimizeBox = false;
                this.MaximizeBox = false;


                this.textBox = new TextBox();
                this.textBox.Visible = true;
                this.textBox.Location = new Point(3, 3);
                this.textBox.Size = new Size(100, 21);
                this.textBox.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(textBox);

                this.buttonOK = new Button
                {
                    Text = "OK",
                    DialogResult = System.Windows.Forms.DialogResult.OK,
                    Location = new Point(3, 28),
                };
                this.Controls.Add(buttonOK);

                this.AcceptButton = buttonOK;
            }

            private void ButtonOK_Click(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }

            public string Value
            {
                get { return textBox.Text; }
                set { textBox.Text = value; }
            }
        }

    }
}
