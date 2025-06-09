namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class SearchDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ddlEncoding = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStartAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEndAddress = new System.Windows.Forms.TextBox();
            this.chkRegexp = new System.Windows.Forms.CheckBox();
            this.ddlScope = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ddlPatterns = new System.Windows.Forms.ComboBox();
            this.chkScanned = new System.Windows.Forms.CheckBox();
            this.chkUnscanned = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search &pattern:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "which &is:";
            // 
            // ddlEncoding
            // 
            this.ddlEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlEncoding.FormattingEnabled = true;
            this.ddlEncoding.Items.AddRange(new object[] {
            "Hexadecimal-encoded binary",
            "Octal-encoded binary",
            "ASCII-encoded text",
            "UTF8-encoded text"});
            this.ddlEncoding.Location = new System.Drawing.Point(12, 73);
            this.ddlEncoding.Name = "ddlEncoding";
            this.ddlEncoding.Size = new System.Drawing.Size(331, 21);
            this.ddlEncoding.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "address range";
            // 
            // txtStartAddress
            // 
            this.txtStartAddress.Location = new System.Drawing.Point(115, 150);
            this.txtStartAddress.Name = "txtStartAddress";
            this.txtStartAddress.Size = new System.Drawing.Size(100, 20);
            this.txtStartAddress.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(221, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "to";
            // 
            // txtEndAddress
            // 
            this.txtEndAddress.Location = new System.Drawing.Point(243, 150);
            this.txtEndAddress.Name = "txtEndAddress";
            this.txtEndAddress.Size = new System.Drawing.Size(100, 20);
            this.txtEndAddress.TabIndex = 6;
            // 
            // chkRegexp
            // 
            this.chkRegexp.AutoSize = true;
            this.chkRegexp.Location = new System.Drawing.Point(16, 100);
            this.chkRegexp.Name = "chkRegexp";
            this.chkRegexp.Size = new System.Drawing.Size(145, 17);
            this.chkRegexp.TabIndex = 3;
            this.chkRegexp.Text = "use as &regular expression";
            this.chkRegexp.UseVisualStyleBackColor = true;
            // 
            // ddlScope
            // 
            this.ddlScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlScope.FormattingEnabled = true;
            this.ddlScope.Items.AddRange(new object[] {
            "in entire project",
            "in current image",
            "in selected address range"});
            this.ddlScope.Location = new System.Drawing.Point(12, 123);
            this.ddlScope.Name = "ddlScope";
            this.ddlScope.Size = new System.Drawing.Size(332, 21);
            this.ddlScope.TabIndex = 4;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSearch.Location = new System.Drawing.Point(187, 254);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(268, 254);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ddlPatterns
            // 
            this.ddlPatterns.FormattingEnabled = true;
            this.ddlPatterns.Location = new System.Drawing.Point(12, 29);
            this.ddlPatterns.Name = "ddlPatterns";
            this.ddlPatterns.Size = new System.Drawing.Size(331, 21);
            this.ddlPatterns.TabIndex = 1;
            // 
            // chkScanned
            // 
            this.chkScanned.AutoSize = true;
            this.chkScanned.Checked = true;
            this.chkScanned.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkScanned.Location = new System.Drawing.Point(12, 179);
            this.chkScanned.Name = "chkScanned";
            this.chkScanned.Size = new System.Drawing.Size(143, 17);
            this.chkScanned.TabIndex = 9;
            this.chkScanned.Text = "Search &scanned memory";
            this.chkScanned.UseVisualStyleBackColor = true;
            // 
            // chkUnscanned
            // 
            this.chkUnscanned.AutoSize = true;
            this.chkUnscanned.Checked = true;
            this.chkUnscanned.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnscanned.Location = new System.Drawing.Point(12, 203);
            this.chkUnscanned.Name = "chkUnscanned";
            this.chkUnscanned.Size = new System.Drawing.Size(155, 17);
            this.chkUnscanned.TabIndex = 10;
            this.chkUnscanned.Text = "Search &unscanned memory";
            this.chkUnscanned.UseVisualStyleBackColor = true;
            // 
            // SearchDialog
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(356, 288);
            this.Controls.Add(this.chkUnscanned);
            this.Controls.Add(this.chkScanned);
            this.Controls.Add(this.ddlPatterns);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.ddlScope);
            this.Controls.Add(this.chkRegexp);
            this.Controls.Add(this.txtEndAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtStartAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ddlEncoding);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(372, 260);
            this.Name = "SearchDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search in image";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ddlEncoding;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStartAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEndAddress;
        private System.Windows.Forms.CheckBox chkRegexp;
        private System.Windows.Forms.ComboBox ddlScope;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox ddlPatterns;
        private System.Windows.Forms.CheckBox chkScanned;
        private System.Windows.Forms.CheckBox chkUnscanned;
    }
}