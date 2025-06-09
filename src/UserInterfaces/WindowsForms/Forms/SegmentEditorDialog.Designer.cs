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
            System.Text.ASCIIEncoding asciiEncodingSealed1 = new System.Text.ASCIIEncoding();
            System.Text.DecoderReplacementFallback decoderReplacementFallback1 = new System.Text.DecoderReplacementFallback();
            System.Text.EncoderReplacementFallback encoderReplacementFallback1 = new System.Text.EncoderReplacementFallback();
            Gui.TextViewing.EmptyEditorModel emptyEditorModel1 = new Gui.TextViewing.EmptyEditorModel();
            txtFileOffset = new System.Windows.Forms.TextBox();
            memView = new Controls.MemoryControl();
            txtLength = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            lblFileData = new System.Windows.Forms.Label();
            ddlArch = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            txtLoadAddress = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            dasmView = new Controls.DisassemblyControl();
            lblDisassembly = new System.Windows.Forms.Label();
            txtSegmentName = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            chkExecute = new System.Windows.Forms.CheckBox();
            chkWrite = new System.Windows.Forms.CheckBox();
            chkRead = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtFileOffset
            // 
            txtFileOffset.Location = new System.Drawing.Point(188, 25);
            txtFileOffset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtFileOffset.Name = "txtFileOffset";
            txtFileOffset.Size = new System.Drawing.Size(158, 23);
            txtFileOffset.TabIndex = 2;
            // 
            // memView
            // 
            memView.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            memView.Architecture = null;
            memView.BytesPerRow =  16U;
            asciiEncodingSealed1.DecoderFallback = decoderReplacementFallback1;
            asciiEncodingSealed1.EncoderFallback = encoderReplacementFallback1;
            memView.Encoding = asciiEncodingSealed1;
            memView.ImageMap = null;
            memView.IsTextSideSelected = false;
            memView.Location = new System.Drawing.Point(0, 16);
            memView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            memView.Name = "memView";
            memView.Procedures = null;
            memView.SegmentMap = null;
            memView.SelectedAddress = null;
            memView.Services = null;
            memView.Size = new System.Drawing.Size(504, 231);
            memView.TabIndex = 7;
            memView.TopAddress = null;
            memView.WordSize =  1U;
            // 
            // txtLength
            // 
            txtLength.Location = new System.Drawing.Point(354, 25);
            txtLength.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtLength.Name = "txtLength";
            txtLength.Size = new System.Drawing.Size(162, 23);
            txtLength.TabIndex = 3;
            txtLength.Text = "8";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(184, 7);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(60, 15);
            label1.TabIndex = 4;
            label1.Text = "File &Offset";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(350, 7);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(44, 15);
            label2.TabIndex = 5;
            label2.Text = "&Length";
            // 
            // lblFileData
            // 
            lblFileData.AutoSize = true;
            lblFileData.Location = new System.Drawing.Point(4, 0);
            lblFileData.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFileData.Name = "lblFileData";
            lblFileData.Size = new System.Drawing.Size(157, 15);
            lblFileData.TabIndex = 6;
            lblFileData.Text = "Preview data att offset 00420";
            // 
            // ddlArch
            // 
            ddlArch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ddlArch.FormattingEnabled = true;
            ddlArch.Location = new System.Drawing.Point(188, 76);
            ddlArch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ddlArch.Name = "ddlArch";
            ddlArch.Size = new System.Drawing.Size(158, 23);
            ddlArch.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(184, 58);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(124, 15);
            label4.TabIndex = 8;
            label4.Text = "&Processor architecture";
            // 
            // txtLoadAddress
            // 
            txtLoadAddress.Location = new System.Drawing.Point(15, 76);
            txtLoadAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtLoadAddress.Name = "txtLoadAddress";
            txtLoadAddress.Size = new System.Drawing.Size(160, 23);
            txtLoadAddress.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(12, 58);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(79, 15);
            label5.TabIndex = 10;
            label5.Text = "Load &address:";
            // 
            // dasmView
            // 
            dasmView.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dasmView.Architecture = null;
            dasmView.Location = new System.Drawing.Point(4, 16);
            dasmView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dasmView.Model = emptyEditorModel1;
            dasmView.Name = "dasmView";
            dasmView.Program = null;
            dasmView.SelectedObject = null;
            dasmView.Services = null;
            dasmView.ShowPcRelative = false;
            dasmView.Size = new System.Drawing.Size(483, 231);
            dasmView.StartAddress = null;
            dasmView.StyleClass = null;
            dasmView.TabIndex = 8;
            dasmView.TopAddress = null;
            // 
            // lblDisassembly
            // 
            lblDisassembly.AutoSize = true;
            lblDisassembly.Location = new System.Drawing.Point(4, 0);
            lblDisassembly.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDisassembly.Name = "lblDisassembly";
            lblDisassembly.Size = new System.Drawing.Size(161, 15);
            lblDisassembly.TabIndex = 12;
            lblDisassembly.Text = "Disassembly (X86 real mode):";
            // 
            // txtSegmentName
            // 
            txtSegmentName.Location = new System.Drawing.Point(14, 25);
            txtSegmentName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtSegmentName.Name = "txtSegmentName";
            txtSegmentName.Size = new System.Drawing.Size(162, 23);
            txtSegmentName.TabIndex = 1;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(10, 7);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(87, 15);
            label7.TabIndex = 14;
            label7.Text = "Segment &name";
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer1.Location = new System.Drawing.Point(14, 222);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(memView);
            splitContainer1.Panel1.Controls.Add(lblFileData);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dasmView);
            splitContainer1.Panel2.Controls.Add(lblDisassembly);
            splitContainer1.Size = new System.Drawing.Size(999, 247);
            splitContainer1.SplitterDistance = 508;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 15;
            // 
            // btnOK
            // 
            btnOK.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOK.Location = new System.Drawing.Point(831, 475);
            btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(88, 27);
            btnOK.TabIndex = 9;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.Location = new System.Drawing.Point(925, 475);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 27);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(chkExecute);
            groupBox1.Controls.Add(chkWrite);
            groupBox1.Controls.Add(chkRead);
            groupBox1.Location = new System.Drawing.Point(14, 107);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(332, 107);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Access mode";
            // 
            // chkExecute
            // 
            chkExecute.AutoSize = true;
            chkExecute.Location = new System.Drawing.Point(7, 76);
            chkExecute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkExecute.Name = "chkExecute";
            chkExecute.Size = new System.Drawing.Size(67, 19);
            chkExecute.TabIndex = 2;
            chkExecute.Text = "E&xecute";
            chkExecute.UseVisualStyleBackColor = true;
            // 
            // chkWrite
            // 
            chkWrite.AutoSize = true;
            chkWrite.Location = new System.Drawing.Point(7, 50);
            chkWrite.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkWrite.Name = "chkWrite";
            chkWrite.Size = new System.Drawing.Size(54, 19);
            chkWrite.TabIndex = 1;
            chkWrite.Text = "&Write";
            chkWrite.UseVisualStyleBackColor = true;
            // 
            // chkRead
            // 
            chkRead.AutoSize = true;
            chkRead.Location = new System.Drawing.Point(7, 22);
            chkRead.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkRead.Name = "chkRead";
            chkRead.Size = new System.Drawing.Size(52, 19);
            chkRead.TabIndex = 0;
            chkRead.Text = "&Read";
            chkRead.UseVisualStyleBackColor = true;
            // 
            // SegmentEditorDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(1027, 516);
            Controls.Add(groupBox1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(splitContainer1);
            Controls.Add(label7);
            Controls.Add(txtSegmentName);
            Controls.Add(label5);
            Controls.Add(txtLoadAddress);
            Controls.Add(label4);
            Controls.Add(ddlArch);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtLength);
            Controls.Add(txtFileOffset);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SegmentEditorDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Specify segment properties";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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