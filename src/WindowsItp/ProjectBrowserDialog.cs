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
            pbs.Load(new Project
            {
                InputFiles = {
                    new InputFile 
                    {
                        Filename = "c:\\test\\foo.exe",
                        BaseAddress = new Address(0x12312300),
                    }
                }
            });
        }
    }
}
