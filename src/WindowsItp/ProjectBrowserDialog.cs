using Reko.Arch.X86;
using Reko.Core;
using Reko.Gui;
using Reko.Gui.Windows.Controls;
using System;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class ProjectBrowserDialog : Form
    {
        private ProjectBrowserService pbs;

        public ProjectBrowserDialog()
        {
            InitializeComponent();
            pbs = new ProjectBrowserService(null, new TreeViewWrapper(treeView));
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var image = new MemoryArea(Address.Ptr32(0x12312300),new byte[0x1000]);
            var imageMap = image.CreateImageMap();
            var arch = new X86ArchitectureFlat32();
            var program = new Core.Program(image, imageMap, arch, new DefaultPlatform(null, arch));
            var project = new Project { Programs = { program } };
            pbs.Load(project);
        }
    }
}
