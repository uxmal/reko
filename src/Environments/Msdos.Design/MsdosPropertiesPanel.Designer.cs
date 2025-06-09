namespace Reko.Environments.Msdos.Design
{
    partial class MsdosPropertiesPanel
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkEmulate8087 = new System.Windows.Forms.CheckBox();
            this.lblEmulate8087 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(74, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(387, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "MS-DOS properties";
            // 
            // label1
            // 
            this.label1.Image = global::Reko.Environments.Msdos.Design.Properties.Resources.msdos_64x64;
            this.label1.Location = new System.Drawing.Point(4, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 64);
            this.label1.TabIndex = 3;
            // 
            // chkEmulate8087
            // 
            this.chkEmulate8087.AutoSize = true;
            this.chkEmulate8087.Location = new System.Drawing.Point(79, 47);
            this.chkEmulate8087.Name = "chkEmulate8087";
            this.chkEmulate8087.Size = new System.Drawing.Size(115, 17);
            this.chkEmulate8087.TabIndex = 4;
            this.chkEmulate8087.Text = "Emulate 808&7 FPU";
            this.chkEmulate8087.UseVisualStyleBackColor = true;
            // 
            // lblEmulate8087
            // 
            this.lblEmulate8087.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEmulate8087.Location = new System.Drawing.Point(99, 67);
            this.lblEmulate8087.Name = "lblEmulate8087";
            this.lblEmulate8087.Size = new System.Drawing.Size(362, 35);
            this.lblEmulate8087.TabIndex = 5;
            this.lblEmulate8087.Text = "If checked, Reko will detect INT instructions that trigger 8087 emulation and rep" +
    "laces them with corresponding 8087 instructions.";
            // 
            // MsdosPropertiesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblEmulate8087);
            this.Controls.Add(this.chkEmulate8087);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "MsdosPropertiesPanel";
            this.Size = new System.Drawing.Size(461, 253);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkEmulate8087;
        private System.Windows.Forms.Label lblEmulate8087;
    }
}
