namespace Reko.UserInterfaces.WindowsForms.Controls
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
            Reko.Gui.TextViewing.EmptyEditorModel emptyEditorModel1 = new Reko.Gui.TextViewing.EmptyEditorModel();
            this.textView1 = new Reko.UserInterfaces.WindowsForms.Controls.TextView();
            this.SuspendLayout();
            // 
            // textView1
            // 
            this.textView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textView1.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textView1.Location = new System.Drawing.Point(0, 0);
            this.textView1.Model = emptyEditorModel1;
            this.textView1.Name = "textView1";
            this.textView1.Services = null;
            this.textView1.Size = new System.Drawing.Size(374, 335);
            this.textView1.TabIndex = 0;
            this.textView1.Text = "textView1";
            // 
            // ImageSegmentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textView1);
            this.Name = "ImageSegmentView";
            this.Size = new System.Drawing.Size(374, 335);
            this.ResumeLayout(false);

        }

        #endregion

        private TextView textView1;
    }
}
