using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Environments.AmigaOS.Design
{
    public partial class AmigaOSProperties : UserControl
    {
        private AmigaOSPropertiesInteractor myInteractor;
        public AmigaOSProperties(AmigaOSPropertiesInteractor interactor)
        {
            InitializeComponent();
            myInteractor = interactor;
            List<int> klist = myInteractor.getAvailableKickstarts();
            foreach(int kick_ver in klist)
                this.cmbKickstartVersions.Items.Add(String.Format("Kickstart {0}",kick_ver));
            this.cmbKickstartVersions.SelectedIndex = 0;
        }
        private void onSelectedItemChanged(object sender, EventArgs e)
        {
            ComboBox src_cmb = (ComboBox)sender;
            int idx_in_list = src_cmb.SelectedIndex;
            if (idx_in_list >= 0)
            {
                List<int> klist = myInteractor.getAvailableKickstarts();
                List<String> libList = myInteractor.GetLibrariesForKickstart(klist.ElementAt(idx_in_list));
                this.lstLoadedLibs.Items.Clear();
                foreach(String lib in libList)
                    this.lstLoadedLibs.Items.Add(lib); //$TODO: mark available library definition files ?
            }
        }
    }
}
