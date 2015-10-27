using Reko.Core.Configuration;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class CodeViewerFrame : Form, IWindowFrame, ICommandTarget
    {
        public CodeViewerFrame()
        {
            InitializeComponent();

            var pane = new CodeViewerPane();
            pane.FrameWindow = this;
            Pane = pane;

            var sc = new ServiceContainer();
            var uiSvc = new FakeDecompilerShellUiService();
            var cfgSvc = new FakeConfigurationService();
            sc.AddService(typeof(IDecompilerShellUiService), uiSvc);
            sc.AddService(typeof(IConfigurationService), cfgSvc);
            var uipSvc = new UiPreferencesService(cfgSvc, new FakeSettingsService());
            uipSvc.Load();
            sc.AddService(typeof(IUiPreferencesService), uipSvc);

            pane.SetSite(sc);
            var ctrl = pane.CreateControl();
            ctrl.Dock = DockStyle.Fill;
            pane.DisplayDataType(new Reko.Core.Program(), CreateDataType());
            Controls.Add(ctrl);
        }

        public IWindowPane Pane
        {
            get; set;
        }

        public string Title
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public DataType CreateDataType()
        {
            var str = new StructureType("foo", 0);
            str.Fields.Add(0, PrimitiveType.Int32);
            str.Fields.Add(4, new Pointer(str, 4));
            return str;
        }

        public bool Execute(CommandID cmdId)
        {
            throw new NotImplementedException();
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            throw new NotImplementedException();
        }
    }
}
