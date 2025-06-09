namespace Reko.WindowsItp
{
    partial class ItpForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memoryControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rTFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disassemblyControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byteMapViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dialogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assumeRegistesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.symbolSourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.procedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ollyScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rewriterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.suffixArrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decoderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.structureFieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlsToolStripMenuItem,
            this.dialogsToolStripMenuItem,
            this.loadingToolStripMenuItem,
            this.performanceToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(428, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.memoryControlToolStripMenuItem,
            this.rTFToolStripMenuItem,
            this.webControlToolStripMenuItem,
            this.treeViewToolStripMenuItem,
            this.disassemblyControlToolStripMenuItem,
            this.textViewToolStripMenuItem,
            this.codeViewToolStripMenuItem,
            this.byteMapViewToolStripMenuItem,
            this.visualizerToolStripMenuItem});
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.controlsToolStripMenuItem.Text = "&Controls";
            this.controlsToolStripMenuItem.Click += new System.EventHandler(this.controlsToolStripMenuItem_Click);
            // 
            // memoryControlToolStripMenuItem
            // 
            this.memoryControlToolStripMenuItem.Name = "memoryControlToolStripMenuItem";
            this.memoryControlToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.memoryControlToolStripMenuItem.Text = "&Memory Control";
            this.memoryControlToolStripMenuItem.Click += new System.EventHandler(this.memoryControlToolStripMenuItem_Click);
            // 
            // rTFToolStripMenuItem
            // 
            this.rTFToolStripMenuItem.Name = "rTFToolStripMenuItem";
            this.rTFToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.rTFToolStripMenuItem.Text = "&RTF ";
            this.rTFToolStripMenuItem.Click += new System.EventHandler(this.rtfToolStripMenuItem_Click);
            // 
            // webControlToolStripMenuItem
            // 
            this.webControlToolStripMenuItem.Name = "webControlToolStripMenuItem";
            this.webControlToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.webControlToolStripMenuItem.Text = "&Web Control";
            this.webControlToolStripMenuItem.Click += new System.EventHandler(this.webControlToolStripMenuItem_Click);
            // 
            // treeViewToolStripMenuItem
            // 
            this.treeViewToolStripMenuItem.Name = "treeViewToolStripMenuItem";
            this.treeViewToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.treeViewToolStripMenuItem.Text = "&Tree View";
            this.treeViewToolStripMenuItem.Click += new System.EventHandler(this.treeViewToolStripMenuItem_Click);
            // 
            // disassemblyControlToolStripMenuItem
            // 
            this.disassemblyControlToolStripMenuItem.Name = "disassemblyControlToolStripMenuItem";
            this.disassemblyControlToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.disassemblyControlToolStripMenuItem.Text = "&Disassembly Control";
            this.disassemblyControlToolStripMenuItem.Click += new System.EventHandler(this.disassemblyControlToolStripMenuItem_Click);
            // 
            // textViewToolStripMenuItem
            // 
            this.textViewToolStripMenuItem.Name = "textViewToolStripMenuItem";
            this.textViewToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.textViewToolStripMenuItem.Text = "Te&xt View";
            this.textViewToolStripMenuItem.Click += new System.EventHandler(this.textViewToolStripMenuItem_Click);
            // 
            // codeViewToolStripMenuItem
            // 
            this.codeViewToolStripMenuItem.Name = "codeViewToolStripMenuItem";
            this.codeViewToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.codeViewToolStripMenuItem.Text = "&Code View";
            this.codeViewToolStripMenuItem.Click += new System.EventHandler(this.codeViewToolStripMenuItem_Click);
            // 
            // byteMapViewToolStripMenuItem
            // 
            this.byteMapViewToolStripMenuItem.Name = "byteMapViewToolStripMenuItem";
            this.byteMapViewToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.byteMapViewToolStripMenuItem.Text = "&Byte Map View";
            this.byteMapViewToolStripMenuItem.Click += new System.EventHandler(this.byteMapViewToolStripMenuItem_Click);
            // 
            // visualizerToolStripMenuItem
            // 
            this.visualizerToolStripMenuItem.Name = "visualizerToolStripMenuItem";
            this.visualizerToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.visualizerToolStripMenuItem.Text = "&Visualizer";
            // 
            // dialogsToolStripMenuItem
            // 
            this.dialogsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchToolStripMenuItem,
            this.projectBrowserToolStripMenuItem,
            this.activationToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.assumeRegistesToolStripMenuItem,
            this.symbolSourcesToolStripMenuItem,
            this.procedureToolStripMenuItem,
            this.propertyOptionsToolStripMenuItem});
            this.dialogsToolStripMenuItem.Name = "dialogsToolStripMenuItem";
            this.dialogsToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.dialogsToolStripMenuItem.Text = "&Dialogs";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.searchToolStripMenuItem.Text = "&Search...";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // projectBrowserToolStripMenuItem
            // 
            this.projectBrowserToolStripMenuItem.Name = "projectBrowserToolStripMenuItem";
            this.projectBrowserToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.projectBrowserToolStripMenuItem.Text = "&Project Browser...";
            this.projectBrowserToolStripMenuItem.Click += new System.EventHandler(this.projectBrowserToolStripMenuItem_Click);
            // 
            // activationToolStripMenuItem
            // 
            this.activationToolStripMenuItem.Name = "activationToolStripMenuItem";
            this.activationToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.activationToolStripMenuItem.Text = "&Activation...";
            this.activationToolStripMenuItem.Click += new System.EventHandler(this.activationToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences...";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // assumeRegistesToolStripMenuItem
            // 
            this.assumeRegistesToolStripMenuItem.Name = "assumeRegistesToolStripMenuItem";
            this.assumeRegistesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.assumeRegistesToolStripMenuItem.Text = "&Assume registers";
            this.assumeRegistesToolStripMenuItem.Click += new System.EventHandler(this.assumeRegistesToolStripMenuItem_Click);
            // 
            // symbolSourcesToolStripMenuItem
            // 
            this.symbolSourcesToolStripMenuItem.Name = "symbolSourcesToolStripMenuItem";
            this.symbolSourcesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.symbolSourcesToolStripMenuItem.Text = "S&ymbol sources...";
            this.symbolSourcesToolStripMenuItem.Click += new System.EventHandler(this.symbolSourcesToolStripMenuItem_Click);
            // 
            // procedureToolStripMenuItem
            // 
            this.procedureToolStripMenuItem.Name = "procedureToolStripMenuItem";
            this.procedureToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.procedureToolStripMenuItem.Text = "P&rocedure...";
            this.procedureToolStripMenuItem.Click += new System.EventHandler(this.procedureToolStripMenuItem_Click);
            // 
            // propertyOptionsToolStripMenuItem
            // 
            this.propertyOptionsToolStripMenuItem.Name = "propertyOptionsToolStripMenuItem";
            this.propertyOptionsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.propertyOptionsToolStripMenuItem.Text = "Property &Options...";
            this.propertyOptionsToolStripMenuItem.Click += new System.EventHandler(this.propertyOptionsToolStripMenuItem_Click);
            // 
            // loadingToolStripMenuItem
            // 
            this.loadingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.emulatorToolStripMenuItem,
            this.ollyScriptToolStripMenuItem});
            this.loadingToolStripMenuItem.Name = "loadingToolStripMenuItem";
            this.loadingToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.loadingToolStripMenuItem.Text = "&Loading";
            // 
            // emulatorToolStripMenuItem
            // 
            this.emulatorToolStripMenuItem.Name = "emulatorToolStripMenuItem";
            this.emulatorToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.emulatorToolStripMenuItem.Text = "X86 &Emulator";
            this.emulatorToolStripMenuItem.Click += new System.EventHandler(this.emulatorToolStripMenuItem_Click);
            // 
            // ollyScriptToolStripMenuItem
            // 
            this.ollyScriptToolStripMenuItem.Name = "ollyScriptToolStripMenuItem";
            this.ollyScriptToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.ollyScriptToolStripMenuItem.Text = "&OllyScript";
            this.ollyScriptToolStripMenuItem.Click += new System.EventHandler(this.ollyScriptToolStripMenuItem_Click);
            // 
            // performanceToolStripMenuItem
            // 
            this.performanceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rewriterToolStripMenuItem,
            this.suffixArrayToolStripMenuItem,
            this.decoderToolStripMenuItem,
            this.structureFieldsToolStripMenuItem});
            this.performanceToolStripMenuItem.Name = "performanceToolStripMenuItem";
            this.performanceToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.performanceToolStripMenuItem.Text = "&Performance";
            // 
            // rewriterToolStripMenuItem
            // 
            this.rewriterToolStripMenuItem.Name = "rewriterToolStripMenuItem";
            this.rewriterToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.rewriterToolStripMenuItem.Text = "&Rewriter...";
            this.rewriterToolStripMenuItem.Click += new System.EventHandler(this.rewriterToolStripMenuItem_Click);
            // 
            // suffixArrayToolStripMenuItem
            // 
            this.suffixArrayToolStripMenuItem.Name = "suffixArrayToolStripMenuItem";
            this.suffixArrayToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.suffixArrayToolStripMenuItem.Text = "&Suffix Array...";
            this.suffixArrayToolStripMenuItem.Click += new System.EventHandler(this.suffixArrayToolStripMenuItem_Click);
            // 
            // decoderToolStripMenuItem
            // 
            this.decoderToolStripMenuItem.Name = "decoderToolStripMenuItem";
            this.decoderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.decoderToolStripMenuItem.Text = "&Decoder...";
            this.decoderToolStripMenuItem.Click += new System.EventHandler(this.decoderToolStripMenuItem_Click);
            // 
            // structureFieldsToolStripMenuItem
            // 
            this.structureFieldsToolStripMenuItem.Name = "structureFieldsToolStripMenuItem";
            this.structureFieldsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.structureFieldsToolStripMenuItem.Text = "&Structure Fields...";
            this.structureFieldsToolStripMenuItem.Click += new System.EventHandler(this.structureFieldsToolStripMenuItem_Click);
            // 
            // ItpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 273);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ItpForm";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem memoryControlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rTFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem webControlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dialogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem treeViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disassemblyControlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem emulatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ollyScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assumeRegistesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byteMapViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem procedureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem symbolSourcesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visualizerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertyOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rewriterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem suffixArrayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decoderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem structureFieldsToolStripMenuItem;
    }
}

