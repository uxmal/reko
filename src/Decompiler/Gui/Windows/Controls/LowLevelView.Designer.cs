namespace Decompiler.Gui.Windows.Controls
{
    partial class LowLevelView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LowLevelView));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.memCtrl = new Decompiler.Gui.Windows.Controls.MemoryControl();
            this.lblMemoryView = new System.Windows.Forms.Label();
            this.dasmCtrl = new Decompiler.Gui.Windows.Controls.DisassemblyControl();
            this.lblDisassembly = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSegmentCaption = new System.Windows.Forms.Label();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnBack = new System.Windows.Forms.ToolStripButton();
            this.btnForward = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.txtAddress = new System.Windows.Forms.ToolStripTextBox();
            this.lowLeveImages = new System.Windows.Forms.ImageList(this.components);
            this.btnGo = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 86);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.memCtrl);
            this.splitContainer1.Panel1.Controls.Add(this.lblMemoryView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dasmCtrl);
            this.splitContainer1.Panel2.Controls.Add(this.lblDisassembly);
            this.splitContainer1.Size = new System.Drawing.Size(858, 395);
            this.splitContainer1.SplitterDistance = 234;
            this.splitContainer1.TabIndex = 0;
            // 
            // memCtrl
            // 
            this.memCtrl.BytesPerRow = ((uint)(16u));
            this.memCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memCtrl.ImageMap = null;
            this.memCtrl.Location = new System.Drawing.Point(0, 18);
            this.memCtrl.Name = "memCtrl";
            this.memCtrl.ProgramImage = null;
            this.memCtrl.SelectedAddress = null;
            this.memCtrl.Size = new System.Drawing.Size(858, 216);
            this.memCtrl.TabIndex = 0;
            this.memCtrl.Text = "memoryControl1";
            this.memCtrl.TopAddress = null;
            this.memCtrl.WordSize = ((uint)(1u));
            // 
            // lblMemoryView
            // 
            this.lblMemoryView.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblMemoryView.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMemoryView.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemoryView.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lblMemoryView.Location = new System.Drawing.Point(0, 0);
            this.lblMemoryView.Name = "lblMemoryView";
            this.lblMemoryView.Size = new System.Drawing.Size(858, 18);
            this.lblMemoryView.TabIndex = 1;
            this.lblMemoryView.Text = "Memory View";
            this.lblMemoryView.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // disassemblyControl1
            // 
            this.dasmCtrl.Architecture = null;
            this.dasmCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dasmCtrl.Image = null;
            this.dasmCtrl.Location = new System.Drawing.Point(0, 18);
            this.dasmCtrl.Name = "disassemblyControl1";
            this.dasmCtrl.SelectedAddress = null;
            this.dasmCtrl.Size = new System.Drawing.Size(858, 139);
            this.dasmCtrl.StartAddress = null;
            this.dasmCtrl.TabIndex = 0;
            this.dasmCtrl.Text = "disassemblyControl1";
            this.dasmCtrl.TopAddress = null;
            // 
            // lblDisassembly
            // 
            this.lblDisassembly.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblDisassembly.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDisassembly.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisassembly.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lblDisassembly.Location = new System.Drawing.Point(0, 0);
            this.lblDisassembly.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblDisassembly.Name = "lblDisassembly";
            this.lblDisassembly.Size = new System.Drawing.Size(858, 18);
            this.lblDisassembly.TabIndex = 1;
            this.lblDisassembly.Text = "Disassembly";
            this.lblDisassembly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblSegmentCaption);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(858, 61);
            this.panel1.TabIndex = 1;
            // 
            // lblSegmentCaption
            // 
            this.lblSegmentCaption.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblSegmentCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSegmentCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSegmentCaption.Location = new System.Drawing.Point(0, 0);
            this.lblSegmentCaption.Name = "lblSegmentCaption";
            this.lblSegmentCaption.Size = new System.Drawing.Size(858, 23);
            this.lblSegmentCaption.TabIndex = 2;
            this.lblSegmentCaption.Text = "Memory View";
            this.lblSegmentCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBack,
            this.btnForward,
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.txtAddress,
            this.btnGo});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(858, 25);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnBack
            // 
            this.btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBack.Image = global::Decompiler.Properties.Resources.Back;
            this.btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(23, 22);
            this.btnBack.Text = "Back";
            // 
            // btnForward
            // 
            this.btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnForward.Image = global::Decompiler.Properties.Resources.Forward;
            this.btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(23, 22);
            this.btnForward.Text = "Forward";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(49, 22);
            this.toolStripLabel1.Text = "Address";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // txtAddress
            // 
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(100, 25);
            // 
            // lowLeveImages
            // 
            this.lowLeveImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.lowLeveImages.ImageSize = new System.Drawing.Size(16, 16);
            this.lowLeveImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnGo
            // 
            this.btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGo.Image = ((System.Drawing.Image)(resources.GetObject("btnGo.Image")));
            this.btnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(26, 22);
            this.btnGo.Text = "Go";
            // 
            // LowLevelView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip);
            this.Name = "LowLevelView";
            this.Size = new System.Drawing.Size(858, 481);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private MemoryControl memCtrl;
        private DisassemblyControl dasmCtrl;
        private System.Windows.Forms.Label lblDisassembly;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblMemoryView;
        private System.Windows.Forms.Label lblSegmentCaption;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripButton btnForward;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox txtAddress;
        private System.Windows.Forms.ImageList lowLeveImages;
        private System.Windows.Forms.ToolStripButton btnGo;
    }
}
