namespace Reko.Gui.Windows.Forms
{
    partial class OpenAsDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ddlEnvironments = new System.Windows.Forms.ComboBox();
            this.ddlArchitectures = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ddlRawFileTypes = new System.Windows.Forms.ComboBox();
            this.lblCPUModel = new System.Windows.Forms.Label();
            this.ddlCPUModels = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Open file:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBox1.Location = new System.Drawing.Point(8, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(352, 20);
            this.textBox1.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(366, 28);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(24, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Processor Architecture:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Operating &Environment:";
            // 
            // ddlEnvironments
            // 
            this.ddlEnvironments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlEnvironments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlEnvironments.FormattingEnabled = true;
            this.ddlEnvironments.Location = new System.Drawing.Point(8, 113);
            this.ddlEnvironments.Name = "ddlEnvironments";
            this.ddlEnvironments.Size = new System.Drawing.Size(382, 21);
            this.ddlEnvironments.TabIndex = 3;
            // 
            // ddlArchitectures
            // 
            this.ddlArchitectures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlArchitectures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlArchitectures.FormattingEnabled = true;
            this.ddlArchitectures.Location = new System.Drawing.Point(8, 161);
            this.ddlArchitectures.Name = "ddlArchitectures";
            this.ddlArchitectures.Size = new System.Drawing.Size(382, 21);
            this.ddlArchitectures.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(234, 296);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(315, 296);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Load binary at &address:";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(8, 206);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(166, 20);
            this.txtAddress.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Raw file &type:";
            // 
            // ddlRawFileTypes
            // 
            this.ddlRawFileTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlRawFileTypes.FormattingEnabled = true;
            this.ddlRawFileTypes.Location = new System.Drawing.Point(8, 73);
            this.ddlRawFileTypes.Name = "ddlRawFileTypes";
            this.ddlRawFileTypes.Size = new System.Drawing.Size(382, 21);
            this.ddlRawFileTypes.TabIndex = 12;
            // 
            // lblCPUModel
            // 
            this.lblCPUModel.AutoSize = true;
            this.lblCPUModel.Location = new System.Drawing.Point(190, 190);
            this.lblCPUModel.Name = "lblCPUModel";
            this.lblCPUModel.Size = new System.Drawing.Size(122, 13);
            this.lblCPUModel.TabIndex = 13;
            this.lblCPUModel.Text = "Select Processor &Model:";
            // 
            // ddlCPUModel
            // 
            this.ddlCPUModels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlCPUModels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCPUModels.FormattingEnabled = true;
            this.ddlCPUModels.Location = new System.Drawing.Point(193, 206);
            this.ddlCPUModels.Name = "ddlCPUModel";
            this.ddlCPUModels.Size = new System.Drawing.Size(197, 21);
            this.ddlCPUModels.TabIndex = 14;
            // 
            // OpenAsDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(402, 330);
            this.Controls.Add(this.ddlCPUModels);
            this.Controls.Add(this.lblCPUModel);
            this.Controls.Add(this.ddlRawFileTypes);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.ddlArchitectures);
            this.Controls.Add(this.ddlEnvironments);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 228);
            this.Name = "OpenAsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Binary File";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ddlEnvironments;
        private System.Windows.Forms.ComboBox ddlArchitectures;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ddlRawFileTypes;
        private System.Windows.Forms.Label lblCPUModel;
        private System.Windows.Forms.ComboBox ddlCPUModels;
    }
}