namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class FindStringsDialog
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
            label1 = new System.Windows.Forms.Label();
            ddlCharSize = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            ddlStringKind = new System.Windows.Forms.ComboBox();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            label4 = new System.Windows.Forms.Label();
            ddlSearchArea = new System.Windows.Forms.ComboBox();
            btnSearchArea = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize) numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 15);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(83, 15);
            label1.TabIndex = 0;
            label1.Text = "&Character size:";
            // 
            // ddlCharSize
            // 
            ddlCharSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ddlCharSize.FormattingEnabled = true;
            ddlCharSize.Items.AddRange(new object[] { "8 bits (e.g. UTF-8)", "16 bits little endian (e.g. UTF-16LE)", "16 bits big endian (e.g. UTF-16BE)" });
            ddlCharSize.Location = new System.Drawing.Point(126, 12);
            ddlCharSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ddlCharSize.Name = "ddlCharSize";
            ddlCharSize.Size = new System.Drawing.Size(347, 23);
            ddlCharSize.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(15, 45);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(97, 15);
            label2.TabIndex = 2;
            label2.Text = "&Length specified:";
            // 
            // ddlStringKind
            // 
            ddlStringKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ddlStringKind.FormattingEnabled = true;
            ddlStringKind.Items.AddRange(new object[] { "C-style (NUL-delimited)", "8-bit size prefix ", "16-bit size prefix", "32-bit size prefix" });
            ddlStringKind.Location = new System.Drawing.Point(126, 45);
            ddlStringKind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ddlStringKind.Name = "ddlStringKind";
            ddlStringKind.Size = new System.Drawing.Size(347, 23);
            ddlStringKind.TabIndex = 2;
            // 
            // btnOk
            // 
            btnOk.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOk.Location = new System.Drawing.Point(292, 147);
            btnOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(88, 27);
            btnOk.TabIndex = 6;
            btnOk.Text = "&Search";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(386, 147);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 27);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(14, 78);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(100, 15);
            label3.TabIndex = 6;
            label3.Text = "&Minimum length:";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new System.Drawing.Point(126, 76);
            numericUpDown1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDown1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new System.Drawing.Size(140, 23);
            numericUpDown1.TabIndex = 3;
            numericUpDown1.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(15, 108);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(58, 15);
            label4.TabIndex = 7;
            label4.Text = "&Search in:";
            // 
            // ddlSearchArea
            // 
            ddlSearchArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ddlSearchArea.FormattingEnabled = true;
            ddlSearchArea.Location = new System.Drawing.Point(126, 105);
            ddlSearchArea.Name = "ddlSearchArea";
            ddlSearchArea.Size = new System.Drawing.Size(317, 23);
            ddlSearchArea.TabIndex = 4;
            // 
            // btnSearchArea
            // 
            btnSearchArea.Location = new System.Drawing.Point(449, 105);
            btnSearchArea.Name = "btnSearchArea";
            btnSearchArea.Size = new System.Drawing.Size(24, 23);
            btnSearchArea.TabIndex = 5;
            btnSearchArea.Text = "...";
            btnSearchArea.UseVisualStyleBackColor = true;
            // 
            // FindStringsDialog
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(488, 188);
            Controls.Add(btnSearchArea);
            Controls.Add(ddlSearchArea);
            Controls.Add(label4);
            Controls.Add(numericUpDown1);
            Controls.Add(label3);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(ddlStringKind);
            Controls.Add(label2);
            Controls.Add(ddlCharSize);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FindStringsDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Find possible strings";
            ((System.ComponentModel.ISupportInitialize) numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlCharSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ddlStringKind;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ddlSearchArea;
        private System.Windows.Forms.Button btnSearchArea;
    }
}