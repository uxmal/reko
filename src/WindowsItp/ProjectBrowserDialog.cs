using Reko.Core;
using Reko.Gui;
using Reko.Gui.Windows;
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
            var image = new LoadedImage(Address.Ptr32(0x12312300),new byte[0x1000]);
            var imageMap = image.CreateImageMap();
            var arch = new Reko.Arch.X86.X86ArchitectureFlat32();
            var program = new Core.Program(image, imageMap, arch, new DefaultPlatform(null, arch));
            var project = new Project { Programs = { program } };
            pbs.Load(project);
        }
    }
}
