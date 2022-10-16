using Reko.Core;
using Reko.Core.Loading;
using Reko.Gui;
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
    public partial class BaseAddressFinderView : UserControl, IWindowPane
    {
        public BaseAddressFinderView()
        {
            InitializeComponent();
        }

        public Base Program { get; set; }
        public IWindowFrame Frame { get; set; }

        public void Close()
        {
        }

        public void SetSite(IServiceProvider services)
        {
        }

        object IWindowPane.CreateControl()
        {
            return this;
        }
    }
}
