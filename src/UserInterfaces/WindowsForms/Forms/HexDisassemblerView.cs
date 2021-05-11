using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class HexDisassemblerView : UserControl
    {
        public HexDisassemblerView()
        {
            InitializeComponent();
            Architectures = new ComboBoxWrapper(this.ddlArchitecture);
            Address = new TextBoxWrapper(this.txtAddress);
            HexBytes = new TextBoxWrapper(this.txtHexBytes);
            Disassembly = new TextBoxWrapper(this.txtDisassembly);
        }

        public IComboBox Architectures { get; }
        public ITextBox Address { get; }
        public ITextBox HexBytes { get; }
        public ITextBox Disassembly { get; }
    }
}
