using Reko.Core;
using Reko.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreProgram = Reko.Core.Program;

namespace Reko.WindowsItp
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
            var image =   new LoadedImage(Address.Ptr32(0x00100000),
                Enumerable.Range(0, 10000)
                .Select(i => (byte)random.Next(256)).ToArray());
            var imageMap = new ImageMap(image.BaseAddress, image.Bytes.Length);
            disassemblyControl1.Model = new DisassemblyTextModel(
                new CoreProgram
                {
                    //new Decompiler.Arch.X86.X86ArchitectureFlat32();
                    Architecture = new Reko.Arch.PowerPC.PowerPcArchitecture32(),
                    Image = image,
                    ImageMap = imageMap
                });
            disassemblyControl1.StartAddress = image.BaseAddress;
        }
    }
}
