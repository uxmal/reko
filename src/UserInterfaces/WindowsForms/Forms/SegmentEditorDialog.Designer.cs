namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class SegmentEditorDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SegmentEditorDialog));
            Reko.Gui.TextViewing.EmptyEditorModel emptyEditorModel3 = new Reko.Gui.TextViewing.EmptyEditorModel();
            this.txtFileOffset = new System.Windows.Forms.TextBox();
            this.memView = new Reko.UserInterfaces.WindowsForms.Controls.MemoryControl();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFileData = new System.Windows.Forms.Label();
            this.ddlArch = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLoadAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dasmView = new Reko.UserInterfaces.WindowsForms.Controls.DisassemblyControl();
            this.lblDisassembly = new System.Windows.Forms.Label();
            this.txtSegmentName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkExecute = new System.Windows.Forms.CheckBox();
            this.chkWrite = new System.Windows.Forms.CheckBox();
            this.chkRead = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFileOffset
            // 
            this.txtFileOffset.Location = new System.Drawing.Point(161, 22);
            this.txtFileOffset.Name = "txtFileOffset";
            this.txtFileOffset.Size = new System.Drawing.Size(136, 20);
            this.txtFileOffset.TabIndex = 2;
            // 
            // memView
            // 
            this.memView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memView.Architecture = null;
            this.memView.BytesPerRow = ((uint)(16u));
            this.memView.Encoding = ((System.Text.Encoding)(resources.GetObject("memView.Encoding")));
            this.memView.ImageMap = null;
            this.memView.Procedures = null;
            this.memView.Location = new System.Drawing.Point(0, 14);
            this.memView.Name = "memView";
            this.memView.SegmentMap = null;
            this.memView.SelectedAddress = null;
            this.memView.Services = null;
            this.memView.Size = new System.Drawing.Size(433, 200);
            this.memView.TabIndex = 7;
            this.memView.TopAddress = null;
            this.memView.WordSize = ((uint)(1u));
            // 
            // txtLength
            // 
            this.txtLength.Location = new System.Drawing.Point(303, 22);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(139, 20);
            this.txtLength.TabIndex = 3;
            this.txtLength.Text = "8";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(158, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "File &Offset";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(300, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "&Length";
            // 
            // lblFileData
            // 
            this.lblFileData.AutoSize = true;
            this.lblFileData.Location = new System.Drawing.Point(3, 0);
            this.lblFileData.Name = "lblFileData";
            this.lblFileData.Size = new System.Drawing.Size(146, 13);
            this.lblFileData.TabIndex = 6;
            this.lblFileData.Text = "Preview data att offset 00420";
            // 
            // ddlArch
            // 
            this.ddlArch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlArch.FormattingEnabled = true;
            this.ddlArch.Location = new System.Drawing.Point(161, 66);
            this.ddlArch.Name = "ddlArch";
            this.ddlArch.Size = new System.Drawing.Size(136, 21);
            this.ddlArch.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(158, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "&Processor architecture";
            // 
            // txtLoadAddress
            // 
            this.txtLoadAddress.Location = new System.Drawing.Point(13, 66);
            this.txtLoadAddress.Name = "txtLoadAddress";
            this.txtLoadAddress.Size = new System.Drawing.Size(138, 20);
            this.txtLoadAddress.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Load &address:";
            // 
            // dasmView
            // 
            this.dasmView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dasmView.Location = new System.Drawing.Point(3, 14);
            this.dasmView.Model = emptyEditorModel3;
            this.dasmView.Name = "dasmView";
            this.dasmView.Program = null;
            this.dasmView.SelectedObject = null;
            this.dasmView.Services = null;
            this.dasmView.ShowPcRelative = false;
            this.dasmView.Size = new System.Drawing.Size(413, 200);
            this.dasmView.StartAddress = null;
            this.dasmView.StyleClass = null;
            this.dasmView.TabIndex = 8;
            this.dasmView.TopAddress = null;
            // 
            // lblDisassembly
            // 
            this.lblDisassembly.AutoSize = true;
            this.lblDisassembly.Location = new System.Drawing.Point(3, 0);
            this.lblDisassembly.Name = "lblDisassembly";
            this.lblDisassembly.Size = new System.Drawing.Size(145, 13);
            this.lblDisassembly.TabIndex = 12;
            this.lblDisassembly.Text = "Disassembly (X86 real mode):";
            // 
            // txtSegmentName
            // 
            this.txtSegmentName.Location = new System.Drawing.Point(12, 22);
            this.txtSegmentName.Name = "txtSegmentName";
            this.txtSegmentName.Size = new System.Drawing.Size(139, 20);
            this.txtSegmentName.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Segment &name";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 192);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.memView);
            this.splitContainer1.Panel1.Controls.Add(this.lblFileData);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dasmView);
            this.splitContainer1.Panel2.Controls.Add(this.lblDisassembly);
            this.splitContainer1.Size = new System.Drawing.Size(856, 214);
            this.splitContainer1.SplitterDistance = 436;
            this.splitContainer1.TabIndex = 15;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(712, 412);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(793, 412);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkExecute);
            this.groupBox1.Controls.Add(this.chkWrite);
            this.groupBox1.Controls.Add(this.chkRead);
            this.groupBox1.Location = new System.Drawing.Point(12, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 93);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Access mode";
            // 
            // chkExecute
            // 
            this.chkExecute.AutoSize = true;
            this.chkExecute.Location = new System.Drawing.Point(6, 66);
            this.chkExecute.Name = "chkExecute";
            this.chkExecute.Size = new System.Drawing.Size(65, 17);
            this.chkExecute.TabIndex = 2;
            this.chkExecute.Text = "E&xecute";
            this.chkExecute.UseVisualStyleBackColor = true;
            // 
            // chkWrite
            // 
            this.chkWrite.AutoSize = true;
            this.chkWrite.Location = new System.Drawing.Point(6, 43);
            this.chkWrite.Name = "chkWrite";
            this.chkWrite.Size = new System.Drawing.Size(51, 17);
            this.chkWrite.TabIndex = 1;
            this.chkWrite.Text = "&Write";
            this.chkWrite.UseVisualStyleBackColor = true;
            // 
            // chkRead
            // 
            this.chkRead.AutoSize = true;
            this.chkRead.Location = new System.Drawing.Point(6, 19);
            this.chkRead.Name = "chkRead";
            this.chkRead.Size = new System.Drawing.Size(52, 17);
            this.chkRead.TabIndex = 0;
            this.chkRead.Text = "&Read";
            this.chkRead.UseVisualStyleBackColor = true;
            // 
            // SegmentEditorDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(880, 447);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtSegmentName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtLoadAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ddlArch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.txtFileOffset);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SegmentEditorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Specify segment properties";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFileOffset;
        private Reko.UserInterfaces.WindowsForms.Controls.MemoryControl memView;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFileData;
        private System.Windows.Forms.ComboBox ddlArch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLoadAddress;
        private System.Windows.Forms.Label label5;
        private Reko.UserInterfaces.WindowsForms.Controls.DisassemblyControl dasmView;
        private System.Windows.Forms.Label lblDisassembly;
        private System.Windows.Forms.TextBox txtSegmentName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkExecute;
        private System.Windows.Forms.CheckBox chkWrite;
        private System.Windows.Forms.CheckBox chkRead;
    }
}