#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class ProcedureDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabProcedure = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSignature = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkTerminates = new System.Windows.Forms.CheckBox();
            this.chkDecompile = new System.Windows.Forms.CheckBox();
            this.chkMalloc = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabProcedure.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(247, 419);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(331, 419);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Name:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(6, 28);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(394, 20);
            this.txtName.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabProcedure);
            this.tabControl1.Location = new System.Drawing.Point(1, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(415, 410);
            this.tabControl1.TabIndex = 10;
            // 
            // tabProcedure
            // 
            this.tabProcedure.Controls.Add(this.label4);
            this.tabProcedure.Controls.Add(this.txtSignature);
            this.tabProcedure.Controls.Add(this.groupBox1);
            this.tabProcedure.Controls.Add(this.label3);
            this.tabProcedure.Controls.Add(this.txtComment);
            this.tabProcedure.Controls.Add(this.txtName);
            this.tabProcedure.Controls.Add(this.label1);
            this.tabProcedure.Location = new System.Drawing.Point(4, 22);
            this.tabProcedure.Name = "tabProcedure";
            this.tabProcedure.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcedure.Size = new System.Drawing.Size(407, 384);
            this.tabProcedure.TabIndex = 1;
            this.tabProcedure.Text = "Procedure";
            this.tabProcedure.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "&Signature:";
            // 
            // txtSignature
            // 
            this.txtSignature.Location = new System.Drawing.Point(6, 71);
            this.txtSignature.Multiline = true;
            this.txtSignature.Name = "txtSignature";
            this.txtSignature.Size = new System.Drawing.Size(391, 78);
            this.txtSignature.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkTerminates);
            this.groupBox1.Controls.Add(this.chkDecompile);
            this.groupBox1.Controls.Add(this.chkMalloc);
            this.groupBox1.Location = new System.Drawing.Point(6, 219);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 150);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "C&haracteristics";
            // 
            // chkTerminates
            // 
            this.chkTerminates.AutoSize = true;
            this.chkTerminates.Location = new System.Drawing.Point(7, 67);
            this.chkTerminates.Name = "chkTerminates";
            this.chkTerminates.Size = new System.Drawing.Size(211, 17);
            this.chkTerminates.TabIndex = 8;
            this.chkTerminates.Text = "Procedure terminates process or thread";
            this.chkTerminates.UseVisualStyleBackColor = true;
            // 
            // chkDecompile
            // 
            this.chkDecompile.AutoSize = true;
            this.chkDecompile.Location = new System.Drawing.Point(6, 43);
            this.chkDecompile.Name = "chkDecompile";
            this.chkDecompile.Size = new System.Drawing.Size(102, 17);
            this.chkDecompile.TabIndex = 7;
            this.chkDecompile.Text = "Decompile body of this procedure";
            this.chkDecompile.UseVisualStyleBackColor = true;
            // 
            // chkMalloc
            // 
            this.chkMalloc.AutoSize = true;
            this.chkMalloc.Location = new System.Drawing.Point(6, 19);
            this.chkMalloc.Name = "chkMalloc";
            this.chkMalloc.Size = new System.Drawing.Size(254, 17);
            this.chkMalloc.TabIndex = 6;
            this.chkMalloc.Text = "Memory allocating procedure (like malloc, calloc)";
            this.chkMalloc.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "&Comment:";
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(6, 168);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(395, 45);
            this.txtComment.TabIndex = 5;
            // 
            // ProcedureDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(415, 454);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcedureDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Procedure";
            this.tabControl1.ResumeLayout(false);
            this.tabProcedure.ResumeLayout(false);
            this.tabProcedure.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabProcedure;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.CheckBox chkMalloc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSignature;
        private System.Windows.Forms.CheckBox chkDecompile;
        private System.Windows.Forms.CheckBox chkTerminates;
    }
}
