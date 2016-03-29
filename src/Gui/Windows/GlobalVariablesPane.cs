using Reko.Core;
using Reko.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows
{
    public class GlobalVariablesPane : IWindowPane
    {
        private IServiceProvider services;
        private Program program;
        private ImageSegment segment;
        private TextView control;

        public GlobalVariablesPane(Program program, ImageSegment segment)
        {
            this.program = program;
            this.segment = segment;
        }

        public IWindowFrame Frame { get; set; }


        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }

        public Control CreateControl()
        {
            var model = new NestedTextModel { };
            this.control = new TextView();
            this.control.Model = model;

            return control;
        }

        public void Close()
        {
            if (control != null)
            {
                control.Dispose();
                control = null;
            }
        }
    }
}
