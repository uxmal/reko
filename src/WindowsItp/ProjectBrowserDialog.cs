using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Gui.Services;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class ProjectBrowserDialog : Form
    {
        private ProjectBrowserService pbs;

        public ProjectBrowserDialog()
        {
            InitializeComponent();
            pbs = new ProjectBrowserService(null, null, new TreeViewWrapper(treeView));
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x12312300),new byte[0x1000]);
            var segmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            var sc = new ServiceContainer();
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            var program = new Core.Program(new ByteProgramMemory(segmentMap), arch, new DefaultPlatform(sc, arch));
            var project = new Project { Programs = { program } };
            pbs.Load(project);
        }
    }
}
