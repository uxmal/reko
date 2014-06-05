using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.Gui.Windows;
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
            var image = new LoadedImage(new Address(0x12312300),new byte[0x1000]);
            var imageMap = new ImageMap(image);
            var arch = new Decompiler.Arch.X86.X86ArchitectureFlat32();
            var program = new Core.Program(image, imageMap, arch, new DefaultPlatform(null, arch));
            pbs.Load(new Project
            {
                InputFiles = {
                    new InputFile 
                    {
                        Filename = "c:\\test\\foo.exe",
                        BaseAddress = image.BaseAddress,
                    }
                }
            },
            program);
        }
    }
}
