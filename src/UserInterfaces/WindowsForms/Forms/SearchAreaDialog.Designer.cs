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
            listSegments = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            txtFreeForm = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
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
            // button1
            // 
            button1.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            button1.Location = new System.Drawing.Point(227, 373);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            button2.Location = new System.Drawing.Point(308, 373);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(75, 23);
            button2.TabIndex = 2;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            // 
            // txtFreeForm
            // 
            txtFreeForm.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtFreeForm.Location = new System.Drawing.Point(12, 209);
            txtFreeForm.Multiline = true;
            txtFreeForm.Name = "txtFreeForm";
            txtFreeForm.Size = new System.Drawing.Size(371, 103);
            txtFreeForm.TabIndex = 3;
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
            label3.Location = new System.Drawing.Point(12, 313);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(372, 57);
            label3.TabIndex = 6;
            label3.Text = "Memory ranges can be specified as half-open or closed intervals ( [0x0000-0x3000) or [0x0000-0x2FFF] respectively).";
            // 
            // SearchAreaDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(395, 408);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtFreeForm);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(listSegments);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtFreeForm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}