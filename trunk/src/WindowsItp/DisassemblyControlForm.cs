using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsItp
{
    public partial class DisassemblyControlForm : Form
    {
        public DisassemblyControlForm()
        {
            InitializeComponent();
        }

        private void DisassemblyControlForm_Load(object sender, EventArgs e)
        {
            var random = new Random(0x4711);
            disassemblyControl1.Architecture =
                //new Decompiler.Arch.X86.X86ArchitectureFlat32();
                new Decompiler.Arch.PowerPC.PowerPcArchitecture(Core.Types.PrimitiveType.Word32);
            disassemblyControl1.Image = new LoadedImage(new Address(0x00100000),
                Enumerable.Range(0, 10000)
                .Select(i => (byte) random.Next(256)).ToArray());
            disassemblyControl1.StartAddress = disassemblyControl1.Image.BaseAddress;
        }
    }
}
