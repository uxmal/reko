#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.MicrochipPIC.Design
{
    partial class PICArchitecturePanel
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
            this.lblPICImage = new System.Windows.Forms.Label();
            this.rdbPIC16 = new System.Windows.Forms.RadioButton();
            this.rdbPIC18 = new System.Windows.Forms.RadioButton();
            this.chkExtendedMode = new System.Windows.Forms.CheckBox();
            this.cbbModel = new System.Windows.Forms.ComboBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblExplain = new System.Windows.Forms.Label();
            this.grpFamily = new System.Windows.Forms.GroupBox();
            this.grpFamily.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPICImage
            // 
            this.lblPICImage.Image = global::Reko.Arch.MicrochipPIC.Design.Properties.Resources.PIC;
            this.lblPICImage.Location = new System.Drawing.Point(12, 20);
            this.lblPICImage.Name = "lblPICImage";
            this.lblPICImage.Size = new System.Drawing.Size(144, 82);
            this.lblPICImage.TabIndex = 0;
            // 
            // rdbPIC16
            // 
            this.rdbPIC16.AutoSize = true;
            this.rdbPIC16.Location = new System.Drawing.Point(15, 19);
            this.rdbPIC16.Name = "rdbPIC16";
            this.rdbPIC16.Size = new System.Drawing.Size(54, 17);
            this.rdbPIC16.TabIndex = 1;
            this.rdbPIC16.TabStop = true;
            this.rdbPIC16.Text = "PIC16";
            this.rdbPIC16.UseVisualStyleBackColor = true;
            // 
            // rdbPIC18
            // 
            this.rdbPIC18.AutoSize = true;
            this.rdbPIC18.Location = new System.Drawing.Point(15, 42);
            this.rdbPIC18.Name = "rdbPIC18";
            this.rdbPIC18.Size = new System.Drawing.Size(54, 17);
            this.rdbPIC18.TabIndex = 2;
            this.rdbPIC18.TabStop = true;
            this.rdbPIC18.Text = "PIC18";
            this.rdbPIC18.UseVisualStyleBackColor = true;
            // 
            // chkExtendedMode
            // 
            this.chkExtendedMode.AutoSize = true;
            this.chkExtendedMode.Location = new System.Drawing.Point(33, 65);
            this.chkExtendedMode.Name = "chkExtendedMode";
            this.chkExtendedMode.Size = new System.Drawing.Size(129, 17);
            this.chkExtendedMode.TabIndex = 3;
            this.chkExtendedMode.Text = "Allow Extended Mode";
            this.chkExtendedMode.UseVisualStyleBackColor = true;
            // 
            // cbbModel
            // 
            this.cbbModel.FormattingEnabled = true;
            this.cbbModel.Location = new System.Drawing.Point(25, 119);
            this.cbbModel.Name = "cbbModel";
            this.cbbModel.Size = new System.Drawing.Size(121, 21);
            this.cbbModel.TabIndex = 4;
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Location = new System.Drawing.Point(22, 103);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(39, 13);
            this.lblModel.TabIndex = 5;
            this.lblModel.Text = "Model:";
            // 
            // lblExplain
            // 
            this.lblExplain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExplain.AutoSize = true;
            this.lblExplain.Location = new System.Drawing.Point(12, 85);
            this.lblExplain.Name = "lblExplain";
            this.lblExplain.Size = new System.Drawing.Size(257, 26);
            this.lblExplain.TabIndex = 6;
            this.lblExplain.Text = "(Only for PIC18 supporting Extended Execution mode\r\n and with appropriate setting" +
    "s of configuration fuses)\r\n";
            // 
            // grpFamily
            // 
            this.grpFamily.Controls.Add(this.rdbPIC16);
            this.grpFamily.Controls.Add(this.lblExplain);
            this.grpFamily.Controls.Add(this.rdbPIC18);
            this.grpFamily.Controls.Add(this.chkExtendedMode);
            this.grpFamily.Location = new System.Drawing.Point(162, 29);
            this.grpFamily.Name = "grpFamily";
            this.grpFamily.Size = new System.Drawing.Size(295, 127);
            this.grpFamily.TabIndex = 7;
            this.grpFamily.TabStop = false;
            this.grpFamily.Text = "PIC Family";
            // 
            // PICArchitecturePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpFamily);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.cbbModel);
            this.Controls.Add(this.lblPICImage);
            this.Name = "PICArchitecturePanel";
            this.Size = new System.Drawing.Size(479, 171);
            this.grpFamily.ResumeLayout(false);
            this.grpFamily.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPICImage;
        private System.Windows.Forms.RadioButton rdbPIC16;
        private System.Windows.Forms.RadioButton rdbPIC18;
        private System.Windows.Forms.CheckBox chkExtendedMode;
        private System.Windows.Forms.ComboBox cbbModel;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.Label lblExplain;
        private System.Windows.Forms.GroupBox grpFamily;
    }
}
