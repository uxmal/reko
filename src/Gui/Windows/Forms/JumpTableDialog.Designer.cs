namespace Reko.Gui.Windows.Forms
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
            this.components = new System.ComponentModel.Container();
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
            this.label3 = new System.Windows.Forms.Label();
            this.errorStartAddress = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numEntries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorStartAddress)).BeginInit();
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
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstruction.Location = new System.Drawing.Point(13, 31);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(47, 11);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Start address:";
            // 
            // txtStartAddress
            // 
            this.errorStartAddress.SetError(this.txtStartAddress, "Invalid address");
            this.txtStartAddress.Location = new System.Drawing.Point(104, 56);
            this.txtStartAddress.MaxLength = 16;
            this.txtStartAddress.Name = "txtStartAddress";
            this.txtStartAddress.Size = new System.Drawing.Size(126, 20);
            this.txtStartAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "&Entries";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(84, 205);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(165, 205);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkIndirectTable
            // 
            this.chkIndirectTable.AutoSize = true;
            this.chkIndirectTable.Location = new System.Drawing.Point(15, 113);
            this.chkIndirectTable.Name = "chkIndirectTable";
            this.chkIndirectTable.Size = new System.Drawing.Size(87, 17);
            this.chkIndirectTable.TabIndex = 8;
            this.chkIndirectTable.Text = "&Indirect table";
            this.chkIndirectTable.UseVisualStyleBackColor = true;
            // 
            // numEntries
            // 
            this.numEntries.Location = new System.Drawing.Point(104, 82);
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
            this.numEntries.TabIndex = 9;
            this.numEntries.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtIndirectTable
            // 
            this.txtIndirectTable.Location = new System.Drawing.Point(29, 155);
            this.txtIndirectTable.MaxLength = 16;
            this.txtIndirectTable.Name = "txtIndirectTable";
            this.txtIndirectTable.Size = new System.Drawing.Size(126, 20);
            this.txtIndirectTable.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Start of in&direct table:";
            // 
            // errorStartAddress
            // 
            this.errorStartAddress.ContainerControl = this;
            // 
            // JumpTableDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 240);
            this.Controls.Add(this.label3);
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
            this.Text = "JumpTableDialog";
            ((System.ComponentModel.ISupportInitialize)(this.numEntries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorStartAddress)).EndInit();
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ErrorProvider errorStartAddress;
    }
}