namespace Reko.Gui.Windows.Controls
{
    partial class CodeView
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
            Reko.Gui.Windows.Controls.EmptyEditorModel emptyEditorModel1 = new Reko.Gui.Windows.Controls.EmptyEditorModel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnBack = new System.Windows.Forms.ToolStripButton();
            this.btnForward = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDeclaration = new System.Windows.Forms.TabPage();
            this.txtDataflow = new System.Windows.Forms.TextBox();
            this.txtDeclaration = new System.Windows.Forms.TextBox();
            this.tabCharacteristics = new System.Windows.Forms.TabPage();
            this.chkTerminates = new System.Windows.Forms.CheckBox();
            this.txtProcName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textView1 = new Reko.Gui.Windows.Controls.TextView();
            this.vldProcName = new Reko.Gui.Windows.Controls.RegexValidator();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabDeclaration.SuspendLayout();
            this.tabCharacteristics.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBack,
            this.btnForward});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(924, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnBack
            // 
            this.btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBack.Image = global::Reko.Gui.Resources.Back;
            this.btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(23, 22);
            this.btnBack.Text = "toolStripButton1";
            this.btnBack.ToolTipText = "Back";
            // 
            // btnForward
            // 
            this.btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnForward.Image = global::Reko.Gui.Resources.Forward;
            this.btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(23, 22);
            this.btnForward.Text = "toolStripButton2";
            this.btnForward.ToolTipText = "Forward";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.txtProcName);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(924, 382);
            this.splitContainer1.SplitterDistance = 583;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabDeclaration);
            this.tabControl1.Controls.Add(this.tabCharacteristics);
            this.tabControl1.Location = new System.Drawing.Point(4, 48);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(311, 331);
            this.tabControl1.TabIndex = 2;
            // 
            // tabDeclaration
            // 
            this.tabDeclaration.Controls.Add(this.txtDataflow);
            this.tabDeclaration.Controls.Add(this.txtDeclaration);
            this.tabDeclaration.Location = new System.Drawing.Point(4, 22);
            this.tabDeclaration.Name = "tabDeclaration";
            this.tabDeclaration.Padding = new System.Windows.Forms.Padding(3);
            this.tabDeclaration.Size = new System.Drawing.Size(303, 305);
            this.tabDeclaration.TabIndex = 0;
            this.tabDeclaration.Text = "Declaration";
            this.tabDeclaration.UseVisualStyleBackColor = true;
            // 
            // txtDataflow
            // 
            this.txtDataflow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataflow.Location = new System.Drawing.Point(3, 149);
            this.txtDataflow.Multiline = true;
            this.txtDataflow.Name = "txtDataflow";
            this.txtDataflow.ReadOnly = true;
            this.txtDataflow.Size = new System.Drawing.Size(294, 150);
            this.txtDataflow.TabIndex = 1;
            // 
            // txtDeclaration
            // 
            this.txtDeclaration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeclaration.Location = new System.Drawing.Point(4, 4);
            this.txtDeclaration.Multiline = true;
            this.txtDeclaration.Name = "txtDeclaration";
            this.txtDeclaration.Size = new System.Drawing.Size(293, 138);
            this.txtDeclaration.TabIndex = 0;
            // 
            // tabCharacteristics
            // 
            this.tabCharacteristics.Controls.Add(this.chkTerminates);
            this.tabCharacteristics.Location = new System.Drawing.Point(4, 22);
            this.tabCharacteristics.Name = "tabCharacteristics";
            this.tabCharacteristics.Padding = new System.Windows.Forms.Padding(3);
            this.tabCharacteristics.Size = new System.Drawing.Size(303, 305);
            this.tabCharacteristics.TabIndex = 1;
            this.tabCharacteristics.Text = "Characteristics";
            this.tabCharacteristics.UseVisualStyleBackColor = true;
            // 
            // chkTerminates
            // 
            this.chkTerminates.AutoSize = true;
            this.chkTerminates.Location = new System.Drawing.Point(4, 7);
            this.chkTerminates.Name = "chkTerminates";
            this.chkTerminates.Size = new System.Drawing.Size(140, 17);
            this.chkTerminates.TabIndex = 0;
            this.chkTerminates.Text = "Procedure never returns";
            this.chkTerminates.UseVisualStyleBackColor = true;
            // 
            // txtProcName
            // 
            this.txtProcName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProcName.Location = new System.Drawing.Point(4, 21);
            this.txtProcName.Name = "txtProcName";
            this.txtProcName.Size = new System.Drawing.Size(307, 20);
            this.txtProcName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Procedure name:";
            // 
            // textView1
            // 
            this.textView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textView1.Location = new System.Drawing.Point(0, 0);
            this.textView1.Model = emptyEditorModel1;
            this.textView1.Name = "textView1";
            this.textView1.Services = null;
            this.textView1.Size = new System.Drawing.Size(583, 382);
            this.textView1.TabIndex = 1;
            this.textView1.Text = "textView1";
            // 
            // vldProcName
            // 
            this.vldProcName.ControlToValidate = this.txtProcName;
            this.vldProcName.ErrorMessage = "Procedure name must be valid C identifier.";
            this.vldProcName.IsValid = true;
            this.vldProcName.ValidationExpression = "^[a-zA-Z_][a-zA-Z0-9_]*$";
            // 
            // CodeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip);
            this.Name = "CodeView";
            this.Size = new System.Drawing.Size(924, 407);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabDeclaration.ResumeLayout(false);
            this.tabDeclaration.PerformLayout();
            this.tabCharacteristics.ResumeLayout(false);
            this.tabCharacteristics.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripButton btnForward;
        private TextView textView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtProcName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDeclaration;
        private System.Windows.Forms.TextBox txtDataflow;
        private System.Windows.Forms.TextBox txtDeclaration;
        private System.Windows.Forms.TabPage tabCharacteristics;
        private System.Windows.Forms.CheckBox chkTerminates;
        private RegexValidator vldProcName;
    }
}
