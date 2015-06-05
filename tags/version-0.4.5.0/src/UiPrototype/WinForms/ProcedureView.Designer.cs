namespace Decompiler.UiPrototype.WinForms
{
    partial class ProcedureView
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
            this.editorView1 = new Decompiler.Gui.Windows.Controls.TextView();
            this.SuspendLayout();
            // 
            // editorView1
            // 
            this.editorView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editorView1.BackColor = System.Drawing.SystemColors.Window;
            this.editorView1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editorView1.Location = new System.Drawing.Point(13, 13);
            this.editorView1.Name = "editorView1";
            this.editorView1.Size = new System.Drawing.Size(334, 328);
            this.editorView1.TabIndex = 0;
            this.editorView1.Text = "editorView1";
            // 
            // ProcedureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 353);
            this.Controls.Add(this.editorView1);
            this.Name = "ProcedureView";
            this.Text = "ProcedureView";
            this.ResumeLayout(false);

        }

        #endregion

        private Decompiler.Gui.Windows.Controls.TextView editorView1;
    }
}