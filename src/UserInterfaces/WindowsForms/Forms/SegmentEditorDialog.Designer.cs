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
            this.txtFileOffset = new System.Windows.Forms.TextBox();
            this.memView = new System.Windows.Forms.TextBox();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFileData = new System.Windows.Forms.Label();
            this.ddlArch = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLoadAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dasmView = new System.Windows.Forms.TextBox();
            this.lblDisassembly = new System.Windows.Forms.Label();
            this.txtSegmentName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFileOffset
            // 
            this.txtFileOffset.Location = new System.Drawing.Point(12, 25);
            this.txtFileOffset.Name = "txtFileOffset";
            this.txtFileOffset.Size = new System.Drawing.Size(136, 20);
            this.txtFileOffset.TabIndex = 0;
            this.txtFileOffset.Text = "DDDDDDDDDDDDDDDD";
            // 
            // memView
            // 
            this.memView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memView.Location = new System.Drawing.Point(0, 16);
            this.memView.Multiline = true;
            this.memView.Name = "memView";
            this.memView.Size = new System.Drawing.Size(375, 231);
            this.memView.TabIndex = 2;
            this.memView.Text = "0001040 DF DF DF 03 42 31 90 10";
            // 
            // txtLength
            // 
            this.txtLength.Location = new System.Drawing.Point(164, 25);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(139, 20);
            this.txtLength.TabIndex = 3;
            this.txtLength.Text = "8";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Start file offset";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(161, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Length:";
            // 
            // lblFileData
            // 
            this.lblFileData.AutoSize = true;
            this.lblFileData.Location = new System.Drawing.Point(3, 0);
            this.lblFileData.Name = "lblFileData";
            this.lblFileData.Size = new System.Drawing.Size(124, 13);
            this.lblFileData.TabIndex = 6;
            this.lblFileData.Text = "File data att offset 00420";
            // 
            // ddlArch
            // 
            this.ddlArch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlArch.FormattingEnabled = true;
            this.ddlArch.Items.AddRange(new object[] {
            "ARM (32-bit)",
            "ARM Thumb",
            "X86 (Real mode)"});
            this.ddlArch.Location = new System.Drawing.Point(326, 25);
            this.ddlArch.Name = "ddlArch";
            this.ddlArch.Size = new System.Drawing.Size(136, 21);
            this.ddlArch.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(323, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Processor architecture";
            // 
            // txtLoadAddress
            // 
            this.txtLoadAddress.Location = new System.Drawing.Point(12, 64);
            this.txtLoadAddress.Name = "txtLoadAddress";
            this.txtLoadAddress.Size = new System.Drawing.Size(138, 20);
            this.txtLoadAddress.TabIndex = 9;
            this.txtLoadAddress.Text = "DDDDDDDDDDDDDDDD";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Load segment at address:";
            // 
            // dasmView
            // 
            this.dasmView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dasmView.Location = new System.Drawing.Point(3, 16);
            this.dasmView.Multiline = true;
            this.dasmView.Name = "dasmView";
            this.dasmView.Size = new System.Drawing.Size(355, 231);
            this.dasmView.TabIndex = 11;
            this.dasmView.Text = "0010040 mov ax,0x0010\r\n0010043 call 2424\r\n0010046 etc etc";
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
            this.txtSegmentName.Location = new System.Drawing.Point(164, 64);
            this.txtSegmentName.Name = "txtSegmentName";
            this.txtSegmentName.Size = new System.Drawing.Size(139, 20);
            this.txtSegmentName.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(161, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Segment name:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(10, 99);
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
            this.splitContainer1.Size = new System.Drawing.Size(740, 247);
            this.splitContainer1.SplitterDistance = 378;
            this.splitContainer1.TabIndex = 15;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(593, 374);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(674, 374);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // SegmentEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 409);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFileOffset;
        private System.Windows.Forms.TextBox memView;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFileData;
        private System.Windows.Forms.ComboBox ddlArch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLoadAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox dasmView;
        private System.Windows.Forms.Label lblDisassembly;
        private System.Windows.Forms.TextBox txtSegmentName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}