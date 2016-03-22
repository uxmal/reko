using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Environments.Msdos.Design
{
    public partial class MsdosPropertiesPanel : UserControl
    {
        public MsdosPropertiesPanel()
        {
            InitializeComponent();
        }

        public CheckBox Emulate8087Checkbox { get { return chkEmulate8087; } }
    }
}
