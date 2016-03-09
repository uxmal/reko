using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Reko.Gui.Controls;
using Reko.Gui.Windows;

namespace Reko.Environments.AmigaOS.Design
{
    public partial class AmigaOSProperties : UserControl
    {
        public AmigaOSProperties()
        {
            InitializeComponent();
            KickstartVersionList = new ComboBoxWrapper(cmbKickstartVersions);
            LoadedLibraryList = lstLoadedLibs;      //$TODO: no wrapper for ListView available yet.
        }

        public IComboBox KickstartVersionList { get; private set; }
        public ListView LoadedLibraryList { get; private set; }
    }
}
