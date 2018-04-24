using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    partial class TextView
    {
        private void InitializeComponent()
        {
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // vScroll
            // 
            this.vScroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScroll.Location = new System.Drawing.Point(0, 0);
            this.vScroll.Name = "vScroll";
            this.vScroll.Size = new System.Drawing.Size(17, 0);
            this.vScroll.TabIndex = 1;
            this.vScroll.Visible = true;
            // 
            // EditorView
            // 
            this.Controls.Add(this.vScroll);
            this.ResumeLayout(false);
        }

        private VScrollBar vScroll;
    }
}
