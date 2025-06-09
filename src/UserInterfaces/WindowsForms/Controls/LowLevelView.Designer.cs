namespace Reko.UserInterfaces.WindowsForms.Controls
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
            if (disposing && (components is not null))
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LowLevelView));
            System.Text.ASCIIEncoding asciiEncodingSealed1 = new System.Text.ASCIIEncoding();
            System.Text.DecoderReplacementFallback decoderReplacementFallback1 = new System.Text.DecoderReplacementFallback();
            System.Text.EncoderReplacementFallback encoderReplacementFallback1 = new System.Text.EncoderReplacementFallback();
            Reko.Gui.TextViewing.EmptyEditorModel emptyEditorModel1 = new Reko.Gui.TextViewing.EmptyEditorModel();
            toolStrip = new System.Windows.Forms.ToolStrip();
            btnBack = new System.Windows.Forms.ToolStripButton();
            btnForward = new System.Windows.Forms.ToolStripButton();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            txtAddress = new System.Windows.Forms.ToolStripTextBox();
            btnGo = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            ddlArchitecture = new System.Windows.Forms.ToolStripComboBox();
            toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            ddlModel = new System.Windows.Forms.ToolStripComboBox();
            lowLeveImages = new System.Windows.Forms.ImageList(components);
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            memCtrl = new MemoryControl();
            lblMemoryView = new System.Windows.Forms.Label();
            dasmCtrl = new DisassemblyControl();
            lblDisassembly = new System.Windows.Forms.Label();
            ddlVisualizer = new System.Windows.Forms.ComboBox();
            visualizerControl = new VisualizerControl();
            label1 = new System.Windows.Forms.Label();
            imageMapControl1 = new ImageMapView();
            toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnBack, btnForward, toolStripLabel1, toolStripSeparator1, txtAddress, btnGo, toolStripSeparator2, toolStripLabel2, ddlArchitecture, toolStripLabel3, ddlModel });
            toolStrip.Location = new System.Drawing.Point(0, 27);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new System.Drawing.Size(1001, 25);
            toolStrip.TabIndex = 3;
            toolStrip.Text = "toolStrip1";
            // 
            // btnBack
            // 
            btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnBack.Image = Resources.Back;
            btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnBack.Name = "btnBack";
            btnBack.Size = new System.Drawing.Size(23, 22);
            btnBack.Text = "Back";
            // 
            // btnForward
            // 
            btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnForward.Image = Resources.Forward;
            btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnForward.Name = "btnForward";
            btnForward.Size = new System.Drawing.Size(23, 22);
            btnForward.Text = "Forward";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(49, 22);
            toolStripLabel1.Text = "Address";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // txtAddress
            // 
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new System.Drawing.Size(116, 25);
            // 
            // btnGo
            // 
            btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            btnGo.Image = (System.Drawing.Image) resources.GetObject("btnGo.Image");
            btnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnGo.Name = "btnGo";
            btnGo.Size = new System.Drawing.Size(26, 22);
            btnGo.Text = "Go";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new System.Drawing.Size(72, 22);
            toolStripLabel2.Text = "Architecture";
            // 
            // ddlArchitecture
            // 
            ddlArchitecture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ddlArchitecture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ddlArchitecture.Name = "ddlArchitecture";
            ddlArchitecture.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripLabel3
            // 
            toolStripLabel3.Name = "toolStripLabel3";
            toolStripLabel3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            toolStripLabel3.Size = new System.Drawing.Size(46, 22);
            toolStripLabel3.Text = "Model";
            // 
            // ddlModel
            // 
            ddlModel.Name = "ddlModel";
            ddlModel.Size = new System.Drawing.Size(121, 25);
            // 
            // lowLeveImages
            // 
            lowLeveImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            lowLeveImages.ImageSize = new System.Drawing.Size(16, 16);
            lowLeveImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 52);
            splitContainer2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(ddlVisualizer);
            splitContainer2.Panel2.Controls.Add(visualizerControl);
            splitContainer2.Panel2.Controls.Add(label1);
            splitContainer2.Size = new System.Drawing.Size(1001, 503);
            splitContainer2.SplitterDistance = 807;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 5;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(memCtrl);
            splitContainer1.Panel1.Controls.Add(lblMemoryView);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dasmCtrl);
            splitContainer1.Panel2.Controls.Add(lblDisassembly);
            splitContainer1.Size = new System.Drawing.Size(807, 503);
            splitContainer1.SplitterDistance = 296;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 1;
            // 
            // memCtrl
            // 
            memCtrl.Architecture = null;
            memCtrl.BytesPerRow =  16U;
            memCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            memCtrl.ImageMap = null;
            memCtrl.IsTextSideSelected = false;
            memCtrl.Location = new System.Drawing.Point(0, 21);
            memCtrl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            memCtrl.Name = "memCtrl";
            memCtrl.Procedures = null;
            memCtrl.SegmentMap = null;
            memCtrl.SelectedAddress = null;
            memCtrl.Services = null;
            memCtrl.Size = new System.Drawing.Size(807, 275);
            memCtrl.TabIndex = 0;
            memCtrl.Text = "memoryControl1";
            memCtrl.TopAddress = null;
            memCtrl.WordSize =  1U;
            // 
            // lblMemoryView
            // 
            lblMemoryView.BackColor = System.Drawing.SystemColors.InactiveCaption;
            lblMemoryView.Dock = System.Windows.Forms.DockStyle.Top;
            lblMemoryView.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblMemoryView.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            lblMemoryView.Location = new System.Drawing.Point(0, 0);
            lblMemoryView.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMemoryView.Name = "lblMemoryView";
            lblMemoryView.Size = new System.Drawing.Size(807, 21);
            lblMemoryView.TabIndex = 1;
            lblMemoryView.Text = "Memory View";
            lblMemoryView.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dasmCtrl
            // 
            dasmCtrl.Architecture = null;
            dasmCtrl.BackColor = System.Drawing.SystemColors.Window;
            dasmCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            dasmCtrl.Location = new System.Drawing.Point(0, 21);
            dasmCtrl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dasmCtrl.Name = "dasmCtrl";
            dasmCtrl.Program = null;
            dasmCtrl.SelectedObject = null;
            dasmCtrl.Services = null;
            dasmCtrl.ShowPcRelative = false;
            dasmCtrl.Size = new System.Drawing.Size(807, 181);
            dasmCtrl.StartAddress = null;
            dasmCtrl.StyleClass = null;
            dasmCtrl.TabIndex = 0;
            dasmCtrl.Text = "disassemblyControl1";
            dasmCtrl.TopAddress = null;
            // 
            // lblDisassembly
            // 
            lblDisassembly.BackColor = System.Drawing.SystemColors.InactiveCaption;
            lblDisassembly.Dock = System.Windows.Forms.DockStyle.Top;
            lblDisassembly.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblDisassembly.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            lblDisassembly.Location = new System.Drawing.Point(0, 0);
            lblDisassembly.Margin = new System.Windows.Forms.Padding(4, 0, 4, 3);
            lblDisassembly.Name = "lblDisassembly";
            lblDisassembly.Size = new System.Drawing.Size(807, 21);
            lblDisassembly.TabIndex = 1;
            lblDisassembly.Text = "Disassembly";
            lblDisassembly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ddlVisualizer
            // 
            ddlVisualizer.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            ddlVisualizer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ddlVisualizer.FormattingEnabled = true;
            ddlVisualizer.Location = new System.Drawing.Point(0, 475);
            ddlVisualizer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ddlVisualizer.Name = "ddlVisualizer";
            ddlVisualizer.Size = new System.Drawing.Size(140, 23);
            ddlVisualizer.TabIndex = 3;
            // 
            // visualizerControl
            // 
            visualizerControl.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            visualizerControl.LineLength = 16;
            visualizerControl.Location = new System.Drawing.Point(0, 21);
            visualizerControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            visualizerControl.Name = "visualizerControl";
            visualizerControl.Program = null;
            visualizerControl.Services = null;
            visualizerControl.Size = new System.Drawing.Size(188, 453);
            visualizerControl.TabIndex = 0;
            visualizerControl.Visualizer = null;
            // 
            // label1
            // 
            label1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            label1.Dock = System.Windows.Forms.DockStyle.Top;
            label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(189, 21);
            label1.TabIndex = 2;
            label1.Text = "Byte Map";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // imageMapControl1
            // 
            imageMapControl1.BackColor = System.Drawing.SystemColors.Control;
            imageMapControl1.Dock = System.Windows.Forms.DockStyle.Top;
            imageMapControl1.Granularity =  1L;
            imageMapControl1.ImageMap = null;
            imageMapControl1.Location = new System.Drawing.Point(0, 0);
            imageMapControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            imageMapControl1.Name = "imageMapControl1";
            imageMapControl1.Offset =  0L;
            imageMapControl1.SegmentMap = null;
            imageMapControl1.SelectedAddress = null;
            imageMapControl1.Size = new System.Drawing.Size(1001, 27);
            imageMapControl1.TabIndex = 4;
            imageMapControl1.Text = "imageMapControl1";
            // 
            // LowLevelView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer2);
            Controls.Add(toolStrip);
            Controls.Add(imageMapControl1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "LowLevelView";
            Size = new System.Drawing.Size(1001, 555);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripButton btnForward;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox txtAddress;
        private System.Windows.Forms.ImageList lowLeveImages;
        private System.Windows.Forms.ToolStripButton btnGo;
        private ImageMapView imageMapControl1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private MemoryControl memCtrl;
        private System.Windows.Forms.Label lblMemoryView;
        private DisassemblyControl dasmCtrl;
        private System.Windows.Forms.Label lblDisassembly;
        private VisualizerControl visualizerControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlVisualizer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox ddlArchitecture;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox ddlModel;
    }
}
