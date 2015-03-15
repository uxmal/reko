namespace Decompiler.Gui.Windows.Controls
{
    partial class ImageSegmentView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Decompiler.Gui.Windows.Controls.EmptyEditorModel emptyEditorModel1 = new Decompiler.Gui.Windows.Controls.EmptyEditorModel();
            this.label1 = new System.Windows.Forms.Label();
            this.textView1 = new Decompiler.Gui.Windows.Controls.TextView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(374, 36);
            this.label1.TabIndex = 1;
            this.label1.Text = "Image segment";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textView1
            // 
            this.textView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textView1.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textView1.Location = new System.Drawing.Point(0, 39);
            this.textView1.Model = emptyEditorModel1;
            this.textView1.Name = "textView1";
            this.textView1.Services = null;
            this.textView1.Size = new System.Drawing.Size(374, 296);
            this.textView1.TabIndex = 0;
            this.textView1.Text = "textView1";
            // 
            // ImageSegmentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textView1);
            this.Name = "ImageSegmentView";
            this.Size = new System.Drawing.Size(374, 335);
            this.ResumeLayout(false);

        }

        #endregion

        private TextView textView1;
        private System.Windows.Forms.Label label1;
    }
}
