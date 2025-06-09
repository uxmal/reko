namespace Reko.Arch.MicrochipPIC.Design
{
    partial class PICArchitecturePicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PICArchitecturePicker));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbbModel = new System.Windows.Forms.ComboBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.grpFamily = new System.Windows.Forms.GroupBox();
            this.chkExtendedMode = new System.Windows.Forms.CheckBox();
            this.lblExplain = new System.Windows.Forms.Label();
            this.rdbPIC18 = new System.Windows.Forms.RadioButton();
            this.rdbPIC16 = new System.Windows.Forms.RadioButton();
            this.picImage = new System.Windows.Forms.Label();
            this.grpFamily.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(371, 146);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(290, 146);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // cbbModel
            // 
            this.cbbModel.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbbModel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbbModel.FormattingEnabled = true;
            this.cbbModel.Location = new System.Drawing.Point(25, 116);
            this.cbbModel.Name = "cbbModel";
            this.cbbModel.Size = new System.Drawing.Size(121, 21);
            this.cbbModel.TabIndex = 4;
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Location = new System.Drawing.Point(22, 93);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(39, 13);
            this.lblModel.TabIndex = 5;
            this.lblModel.Text = "Model:";
            // 
            // grpFamily
            // 
            this.grpFamily.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFamily.Controls.Add(this.chkExtendedMode);
            this.grpFamily.Controls.Add(this.lblExplain);
            this.grpFamily.Controls.Add(this.rdbPIC18);
            this.grpFamily.Controls.Add(this.rdbPIC16);
            this.grpFamily.Location = new System.Drawing.Point(155, 12);
            this.grpFamily.Name = "grpFamily";
            this.grpFamily.Size = new System.Drawing.Size(302, 125);
            this.grpFamily.TabIndex = 6;
            this.grpFamily.TabStop = false;
            this.grpFamily.Text = "PIC Family";
            // 
            // chkExtendedMode
            // 
            this.chkExtendedMode.AutoSize = true;
            this.chkExtendedMode.Enabled = false;
            this.chkExtendedMode.Location = new System.Drawing.Point(66, 43);
            this.chkExtendedMode.Name = "chkExtendedMode";
            this.chkExtendedMode.Size = new System.Drawing.Size(178, 17);
            this.chkExtendedMode.TabIndex = 3;
            this.chkExtendedMode.Text = "Allow Extended Execution mode";
            this.chkExtendedMode.UseVisualStyleBackColor = true;
            // 
            // lblExplain
            // 
            this.lblExplain.AutoSize = true;
            this.lblExplain.Enabled = false;
            this.lblExplain.Location = new System.Drawing.Point(19, 63);
            this.lblExplain.Name = "lblExplain";
            this.lblExplain.Size = new System.Drawing.Size(257, 26);
            this.lblExplain.TabIndex = 2;
            this.lblExplain.Text = "(Only for PIC18 supporting Extended Execution mode\r\n and with appropriate setting" +
    "s of configuration fuses)\r\n";
            // 
            // rdbPIC18
            // 
            this.rdbPIC18.AutoSize = true;
            this.rdbPIC18.Location = new System.Drawing.Point(6, 42);
            this.rdbPIC18.Name = "rdbPIC18";
            this.rdbPIC18.Size = new System.Drawing.Size(54, 17);
            this.rdbPIC18.TabIndex = 1;
            this.rdbPIC18.Text = "PIC18";
            this.rdbPIC18.UseVisualStyleBackColor = true;
            // 
            // rdbPIC16
            // 
            this.rdbPIC16.AutoSize = true;
            this.rdbPIC16.Checked = true;
            this.rdbPIC16.Location = new System.Drawing.Point(6, 19);
            this.rdbPIC16.Name = "rdbPIC16";
            this.rdbPIC16.Size = new System.Drawing.Size(54, 17);
            this.rdbPIC16.TabIndex = 0;
            this.rdbPIC16.TabStop = true;
            this.rdbPIC16.Text = "PIC16";
            this.rdbPIC16.UseVisualStyleBackColor = true;
            // 
            // picImage
            // 
            this.picImage.Image = global::Reko.Arch.MicrochipPIC.Design.Properties.Resources.PICImage;
            this.picImage.Location = new System.Drawing.Point(12, 9);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(137, 74);
            this.picImage.TabIndex = 3;
            // 
            // PICArchitecturePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 181);
            this.Controls.Add(this.grpFamily);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.cbbModel);
            this.Controls.Add(this.picImage);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PICArchitecturePicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PIC Architecture";
            this.TopMost = true;
            this.grpFamily.ResumeLayout(false);
            this.grpFamily.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label picImage;
        private System.Windows.Forms.ComboBox cbbModel;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.GroupBox grpFamily;
        private System.Windows.Forms.RadioButton rdbPIC18;
        private System.Windows.Forms.RadioButton rdbPIC16;
        private System.Windows.Forms.Label lblExplain;
        private System.Windows.Forms.CheckBox chkExtendedMode;
    }
}