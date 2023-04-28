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
    public partial class ProcedureListPanel : UserControl
    {
        public ProcedureListPanel()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public TextBox ProcedureFilter => txtProcedureFilter;

        [Browsable(false)]
        public ListView ProcedureList => listProcedures;

        [Browsable(false)]
        public ToolStripMenuItem AllProceduresMenuItem => this.allProceduresToolStripMenuItem;
        [Browsable(false)]
        public ToolStripMenuItem LeafProceduresMenuItem => this.leafProceduresToolStripMenuItem;
        [Browsable(false)]
        public ToolStripMenuItem RootProceduresMenuItem => this.rootProceduresToolStripMenuItem;
    }
}
