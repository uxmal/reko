namespace Reko.UserInterfaces.WindowsForms.Controls
{
    partial class TypeMarker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnClose = new System.Windows.Forms.Button();
            txtUserText = new System.Windows.Forms.TextBox();
            lblCaption = new System.Windows.Forms.Label();
            lblRenderedType = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.Location = new System.Drawing.Point(357, 80);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(75, 23);
            btnClose.TabIndex = 0;
            btnClose.Text = "&Close";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // txtUserText
            // 
            txtUserText.Location = new System.Drawing.Point(6, 27);
            txtUserText.Name = "txtUserText";
            txtUserText.Size = new System.Drawing.Size(426, 23);
            txtUserText.TabIndex = 1;
            // 
            // lblCaption
            // 
            lblCaption.AutoSize = true;
            lblCaption.Location = new System.Drawing.Point(3, 9);
            lblCaption.Name = "lblCaption";
            lblCaption.Size = new System.Drawing.Size(328, 15);
            lblCaption.TabIndex = 2;
            // 
            // lblRenderedType
            // 
            lblRenderedType.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblRenderedType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblRenderedType.Location = new System.Drawing.Point(6, 53);
            lblRenderedType.Name = "lblRenderedType";
            lblRenderedType.Size = new System.Drawing.Size(426, 24);
            lblRenderedType.TabIndex = 5;
            lblRenderedType.Text = "label1";
            // 
            // TypeMarker2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(lblRenderedType);
            Controls.Add(txtUserText);
            Controls.Add(btnClose);
            Controls.Add(lblCaption);
            Name = "TypeMarker2";
            Size = new System.Drawing.Size(435, 106);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtUserText;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label lblRenderedType;
    }
}
