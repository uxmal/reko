using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class SegmentListView : UserControl
    {
        public SegmentListView()
        {
            InitializeComponent();
        }

        public ListView Segments { get => this.listViewSegments; }
    }
}
