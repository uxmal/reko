namespace Reko.WindowsItp
{
    partial class DisassemblyControlForm
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
            this.disassemblyControl1 = new Reko.UserInterfaces.WindowsForms.Controls.DisassemblyControl();
            this.SuspendLayout();
            // 
            // disassemblyControl1
            // 
            this.disassemblyControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.disassemblyControl1.BackColor = System.Drawing.SystemColors.Window;
            this.disassemblyControl1.Location = new System.Drawing.Point(12, 12);
            this.disassemblyControl1.Name = "disassemblyControl1";
            this.disassemblyControl1.Size = new System.Drawing.Size(248, 286);
            this.disassemblyControl1.TabIndex = 0;
            this.disassemblyControl1.Text = "disassemblyControl1";
            // 
            // DisassemblyControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 310);
            this.Controls.Add(this.disassemblyControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DisassemblyControlForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "DisassemblyControlForm";
            this.Load += new System.EventHandler(this.DisassemblyControlForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Reko.UserInterfaces.WindowsForms.Controls.DisassemblyControl disassemblyControl1;
    }
}