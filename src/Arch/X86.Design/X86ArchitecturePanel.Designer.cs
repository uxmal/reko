#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion
 
namespace Reko.Arch.X86.Design
{
    partial class X86ArchitecturePanel
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
            this.label2.Location = new System.Drawing.Point(142, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(319, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "x86 properties";
            // 
            // label1
            // 
            this.label1.Image = global::Reko.Arch.X86.Design.Properties.Resources.x86;
            this.label1.Location = new System.Drawing.Point(4, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 130);
            this.label1.TabIndex = 3;
            // 
            // chkEmulate8087
            // 
            this.chkEmulate8087.AutoSize = true;
            this.chkEmulate8087.Location = new System.Drawing.Point(147, 47);
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
            this.lblEmulate8087.Location = new System.Drawing.Point(165, 67);
            this.lblEmulate8087.Name = "lblEmulate8087";
            this.lblEmulate8087.Size = new System.Drawing.Size(296, 35);
            this.lblEmulate8087.TabIndex = 5;
            this.lblEmulate8087.Text = "If checked, Reko will detect INT instructions that trigger 8087 emulation and rep" +
    "laces them with corresponding 8087 instructions.";
            // 
            // X86ArchitecturePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblEmulate8087);
            this.Controls.Add(this.chkEmulate8087);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "X86ArchitecturePanel";
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
