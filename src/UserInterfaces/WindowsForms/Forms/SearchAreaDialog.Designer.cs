namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class SearchAreaDialog
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
            listSegments = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            txtFreeForm = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            lblFreeFormError = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // listSegments
            // 
            listSegments.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            listSegments.CheckBoxes = true;
            listSegments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            listSegments.Location = new System.Drawing.Point(12, 29);
            listSegments.Name = "listSegments";
            listSegments.Size = new System.Drawing.Size(371, 155);
            listSegments.TabIndex = 0;
            listSegments.UseCompatibleStateImageBehavior = false;
            listSegments.View = System.Windows.Forms.View.Details;
            listSegments.ItemChecked += listSegments_ItemChecked;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Segment";
            columnHeader1.Width = 90;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Address";
            columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Attributes";
            columnHeader3.Width = 90;
            // 
            // btnOK
            // 
            btnOK.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(227, 373);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(75, 23);
            btnOK.TabIndex = 1;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(308, 373);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtFreeForm
            // 
            txtFreeForm.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtFreeForm.Location = new System.Drawing.Point(12, 205);
            txtFreeForm.Multiline = true;
            txtFreeForm.Name = "txtFreeForm";
            txtFreeForm.Size = new System.Drawing.Size(371, 88);
            txtFreeForm.TabIndex = 3;
            txtFreeForm.TextChanged += txtFreeForm_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 187);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(235, 15);
            label1.TabIndex = 4;
            label1.Text = "Enter memory ranges as free form text text:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(14, 7);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(62, 15);
            label2.TabIndex = 5;
            label2.Text = "&Segments:";
            // 
            // label3
            // 
            label3.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label3.Location = new System.Drawing.Point(13, 315);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(372, 38);
            label3.TabIndex = 6;
            label3.Text = "Memory ranges can be specified as half-open or closed intervals ( [0x0000-0x3000) or [0x0000-0x2FFF] respectively).";
            // 
            // lblFreeFormError
            // 
            lblFreeFormError.AutoSize = true;
            lblFreeFormError.ForeColor = System.Drawing.Color.Red;
            lblFreeFormError.Location = new System.Drawing.Point(13, 300);
            lblFreeFormError.Name = "lblFreeFormError";
            lblFreeFormError.Size = new System.Drawing.Size(0, 15);
            lblFreeFormError.TabIndex = 7;
            // 
            // SearchAreaDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(395, 408);
            Controls.Add(lblFreeFormError);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtFreeForm);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(listSegments);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SearchAreaDialog";
            ShowInTaskbar = false;
            Text = "Select memory areas to search ";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView listSegments;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtFreeForm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFreeFormError;
    }
}