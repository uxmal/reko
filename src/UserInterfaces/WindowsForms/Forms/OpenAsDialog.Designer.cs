namespace Reko.UserInterfaces.WindowsForms.Forms
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
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ddlModels = new System.Windows.Forms.ComboBox();
            this.rdbSpecifyAddress = new System.Windows.Forms.RadioButton();
            this.rdbGuessAddress = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Open file:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBox1.Location = new System.Drawing.Point(9, 33);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(618, 23);
            this.textBox1.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(635, 31);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(28, 27);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 178);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "&Processor Architecture:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 132);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Operating &Environment:";
            // 
            // ddlEnvironments
            // 
            this.ddlEnvironments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlEnvironments.FormattingEnabled = true;
            this.ddlEnvironments.Location = new System.Drawing.Point(9, 150);
            this.ddlEnvironments.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ddlEnvironments.Name = "ddlEnvironments";
            this.ddlEnvironments.Size = new System.Drawing.Size(331, 23);
            this.ddlEnvironments.TabIndex = 6;
            // 
            // ddlArchitectures
            // 
            this.ddlArchitectures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlArchitectures.FormattingEnabled = true;
            this.ddlArchitectures.Location = new System.Drawing.Point(9, 196);
            this.ddlArchitectures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ddlArchitectures.Name = "ddlArchitectures";
            this.ddlArchitectures.Size = new System.Drawing.Size(331, 23);
            this.ddlArchitectures.TabIndex = 8;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(481, 422);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 27);
            this.btnOk.TabIndex = 15;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(575, 422);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 27);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 277);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "Load binary";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(115, 294);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(193, 23);
            this.txtAddress.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 84);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "Raw file &type:";
            // 
            // ddlRawFileTypes
            // 
            this.ddlRawFileTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlRawFileTypes.FormattingEnabled = true;
            this.ddlRawFileTypes.Location = new System.Drawing.Point(9, 104);
            this.ddlRawFileTypes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ddlRawFileTypes.Name = "ddlRawFileTypes";
            this.ddlRawFileTypes.Size = new System.Drawing.Size(330, 23);
            this.ddlRawFileTypes.TabIndex = 4;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(366, 103);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid.Size = new System.Drawing.Size(295, 302);
            this.propertyGrid.TabIndex = 14;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(363, 83);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Op&tions:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 228);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 15);
            this.label7.TabIndex = 9;
            this.label7.Text = "Processor &Model:";
            // 
            // ddlModels
            // 
            this.ddlModels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlModels.FormattingEnabled = true;
            this.ddlModels.Location = new System.Drawing.Point(9, 247);
            this.ddlModels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ddlModels.Name = "ddlModels";
            this.ddlModels.Size = new System.Drawing.Size(331, 23);
            this.ddlModels.TabIndex = 10;
            // 
            // rdbSpecifyAddress
            // 
            this.rdbSpecifyAddress.AutoSize = true;
            this.rdbSpecifyAddress.Checked = true;
            this.rdbSpecifyAddress.Location = new System.Drawing.Point(27, 295);
            this.rdbSpecifyAddress.Name = "rdbSpecifyAddress";
            this.rdbSpecifyAddress.Size = new System.Drawing.Size(81, 19);
            this.rdbSpecifyAddress.TabIndex = 17;
            this.rdbSpecifyAddress.TabStop = true;
            this.rdbSpecifyAddress.Text = "&at address:";
            this.rdbSpecifyAddress.UseVisualStyleBackColor = true;
            // 
            // rdbGuessAddress
            // 
            this.rdbGuessAddress.AutoSize = true;
            this.rdbGuessAddress.Location = new System.Drawing.Point(27, 323);
            this.rdbGuessAddress.Name = "rdbGuessAddress";
            this.rdbGuessAddress.Size = new System.Drawing.Size(185, 19);
            this.rdbGuessAddress.TabIndex = 18;
            this.rdbGuessAddress.Text = "at an address &guessed by Reko";
            this.rdbGuessAddress.UseVisualStyleBackColor = true;
            // 
            // OpenAsDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(677, 461);
            this.Controls.Add(this.rdbGuessAddress);
            this.Controls.Add(this.rdbSpecifyAddress);
            this.Controls.Add(this.ddlModels);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.propertyGrid);
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
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(371, 257);
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
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox ddlModels;
        private System.Windows.Forms.RadioButton rdbSpecifyAddress;
        private System.Windows.Forms.RadioButton rdbGuessAddress;
    }
}