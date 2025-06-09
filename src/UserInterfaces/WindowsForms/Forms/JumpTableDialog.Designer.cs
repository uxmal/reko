namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class JumpTableDialog
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
            this.lblCaption = new System.Windows.Forms.Label();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtStartAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkIndirectTable = new System.Windows.Forms.CheckBox();
            this.numEntries = new System.Windows.Forms.NumericUpDown();
            this.txtIndirectTable = new System.Windows.Forms.TextBox();
            this.lblIndirectTable = new System.Windows.Forms.Label();
            this.panelSegmentedAddresses = new System.Windows.Forms.Panel();
            this.rdbRelativeOffsets = new System.Windows.Forms.RadioButton();
            this.ddlSegments = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rdbOffsets = new System.Windows.Forms.RadioButton();
            this.rdbFarAddresses = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.ddlRegisters = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listEntries = new System.Windows.Forms.ListBox();
            this.txtDasm = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtJumpAddress = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numEntries)).BeginInit();
            this.panelSegmentedAddresses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(12, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(54, 13);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "<replace>";
            // 
            // lblInstruction
            // 
            this.lblInstruction.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstruction.Location = new System.Drawing.Point(13, 58);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(217, 11);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Start address:";
            // 
            // txtStartAddress
            // 
            this.txtStartAddress.Location = new System.Drawing.Point(104, 77);
            this.txtStartAddress.MaxLength = 16;
            this.txtStartAddress.Name = "txtStartAddress";
            this.txtStartAddress.Size = new System.Drawing.Size(126, 20);
            this.txtStartAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "&Entries";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(456, 333);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(537, 333);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkIndirectTable
            // 
            this.chkIndirectTable.AutoSize = true;
            this.chkIndirectTable.Location = new System.Drawing.Point(15, 158);
            this.chkIndirectTable.Name = "chkIndirectTable";
            this.chkIndirectTable.Size = new System.Drawing.Size(87, 17);
            this.chkIndirectTable.TabIndex = 8;
            this.chkIndirectTable.Text = "&Indirect table";
            this.chkIndirectTable.UseVisualStyleBackColor = true;
            // 
            // numEntries
            // 
            this.numEntries.Location = new System.Drawing.Point(104, 103);
            this.numEntries.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numEntries.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numEntries.Name = "numEntries";
            this.numEntries.Size = new System.Drawing.Size(126, 20);
            this.numEntries.TabIndex = 5;
            this.numEntries.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtIndirectTable
            // 
            this.txtIndirectTable.Location = new System.Drawing.Point(29, 196);
            this.txtIndirectTable.MaxLength = 16;
            this.txtIndirectTable.Name = "txtIndirectTable";
            this.txtIndirectTable.Size = new System.Drawing.Size(126, 20);
            this.txtIndirectTable.TabIndex = 10;
            // 
            // lblIndirectTable
            // 
            this.lblIndirectTable.AutoSize = true;
            this.lblIndirectTable.Location = new System.Drawing.Point(26, 180);
            this.lblIndirectTable.Name = "lblIndirectTable";
            this.lblIndirectTable.Size = new System.Drawing.Size(107, 13);
            this.lblIndirectTable.TabIndex = 9;
            this.lblIndirectTable.Text = "Start of in&direct table:";
            // 
            // panelSegmentedAddresses
            // 
            this.panelSegmentedAddresses.Controls.Add(this.rdbRelativeOffsets);
            this.panelSegmentedAddresses.Controls.Add(this.ddlSegments);
            this.panelSegmentedAddresses.Controls.Add(this.label4);
            this.panelSegmentedAddresses.Controls.Add(this.rdbOffsets);
            this.panelSegmentedAddresses.Controls.Add(this.rdbFarAddresses);
            this.panelSegmentedAddresses.Location = new System.Drawing.Point(0, 222);
            this.panelSegmentedAddresses.Name = "panelSegmentedAddresses";
            this.panelSegmentedAddresses.Size = new System.Drawing.Size(258, 109);
            this.panelSegmentedAddresses.TabIndex = 16;
            // 
            // rdbRelativeOffsets
            // 
            this.rdbRelativeOffsets.AutoSize = true;
            this.rdbRelativeOffsets.Location = new System.Drawing.Point(29, 53);
            this.rdbRelativeOffsets.Name = "rdbRelativeOffsets";
            this.rdbRelativeOffsets.Size = new System.Drawing.Size(98, 17);
            this.rdbRelativeOffsets.TabIndex = 12;
            this.rdbRelativeOffsets.Text = "Relative offsets";
            this.rdbRelativeOffsets.UseVisualStyleBackColor = true;
            // 
            // ddlSegments
            // 
            this.ddlSegments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlSegments.FormattingEnabled = true;
            this.ddlSegments.Location = new System.Drawing.Point(156, 75);
            this.ddlSegments.Name = "ddlSegments";
            this.ddlSegments.Size = new System.Drawing.Size(92, 21);
            this.ddlSegments.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Table entries are:";
            // 
            // rdbOffsets
            // 
            this.rdbOffsets.AutoSize = true;
            this.rdbOffsets.Location = new System.Drawing.Point(29, 76);
            this.rdbOffsets.Name = "rdbOffsets";
            this.rdbOffsets.Size = new System.Drawing.Size(121, 17);
            this.rdbOffsets.TabIndex = 13;
            this.rdbOffsets.Text = "Offsets into segment";
            this.rdbOffsets.UseVisualStyleBackColor = true;
            // 
            // rdbFarAddresses
            // 
            this.rdbFarAddresses.AutoSize = true;
            this.rdbFarAddresses.Checked = true;
            this.rdbFarAddresses.Location = new System.Drawing.Point(29, 30);
            this.rdbFarAddresses.Name = "rdbFarAddresses";
            this.rdbFarAddresses.Size = new System.Drawing.Size(92, 17);
            this.rdbFarAddresses.TabIndex = 11;
            this.rdbFarAddresses.TabStop = true;
            this.rdbFarAddresses.Text = "Full addresses";
            this.rdbFarAddresses.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Inde&x register:";
            // 
            // ddlRegisters
            // 
            this.ddlRegisters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlRegisters.FormattingEnabled = true;
            this.ddlRegisters.Location = new System.Drawing.Point(104, 129);
            this.ddlRegisters.Name = "ddlRegisters";
            this.ddlRegisters.Size = new System.Drawing.Size(92, 21);
            this.ddlRegisters.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(296, 31);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listEntries);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtDasm);
            this.splitContainer1.Size = new System.Drawing.Size(316, 287);
            this.splitContainer1.SplitterDistance = 126;
            this.splitContainer1.TabIndex = 18;
            // 
            // listEntries
            // 
            this.listEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listEntries.FormattingEnabled = true;
            this.listEntries.IntegralHeight = false;
            this.listEntries.Location = new System.Drawing.Point(0, 0);
            this.listEntries.Name = "listEntries";
            this.listEntries.Size = new System.Drawing.Size(126, 287);
            this.listEntries.TabIndex = 0;
            // 
            // txtDasm
            // 
            this.txtDasm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDasm.Enabled = false;
            this.txtDasm.Location = new System.Drawing.Point(0, 0);
            this.txtDasm.Multiline = true;
            this.txtDasm.Name = "txtDasm";
            this.txtDasm.Size = new System.Drawing.Size(186, 287);
            this.txtDasm.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "&Jump address";
            // 
            // txtJumpAddress
            // 
            this.txtJumpAddress.Location = new System.Drawing.Point(104, 30);
            this.txtJumpAddress.Name = "txtJumpAddress";
            this.txtJumpAddress.Size = new System.Drawing.Size(126, 20);
            this.txtJumpAddress.TabIndex = 2;
            // 
            // JumpTableDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(624, 368);
            this.Controls.Add(this.txtJumpAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.ddlRegisters);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panelSegmentedAddresses);
            this.Controls.Add(this.lblIndirectTable);
            this.Controls.Add(this.txtIndirectTable);
            this.Controls.Add(this.numEntries);
            this.Controls.Add(this.chkIndirectTable);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtStartAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.lblCaption);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JumpTableDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Jump Table";
            ((System.ComponentModel.ISupportInitialize)(this.numEntries)).EndInit();
            this.panelSegmentedAddresses.ResumeLayout(false);
            this.panelSegmentedAddresses.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtStartAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkIndirectTable;
        private System.Windows.Forms.NumericUpDown numEntries;
        private System.Windows.Forms.TextBox txtIndirectTable;
        private System.Windows.Forms.Label lblIndirectTable;
        private System.Windows.Forms.Panel panelSegmentedAddresses;
        private System.Windows.Forms.ComboBox ddlSegments;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rdbOffsets;
        private System.Windows.Forms.RadioButton rdbFarAddresses;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ddlRegisters;
        private System.Windows.Forms.RadioButton rdbRelativeOffsets;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtDasm;
        private System.Windows.Forms.ListBox listEntries;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtJumpAddress;
    }
}