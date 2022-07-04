#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
            this.chkIsAlloca = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabProcedure.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(288, 483);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 27);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(386, 483);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 27);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Name:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(7, 32);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(459, 23);
            this.txtName.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabProcedure);
            this.tabControl1.Location = new System.Drawing.Point(1, 3);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(484, 473);
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
            this.tabProcedure.Location = new System.Drawing.Point(4, 24);
            this.tabProcedure.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabProcedure.Name = "tabProcedure";
            this.tabProcedure.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabProcedure.Size = new System.Drawing.Size(476, 445);
            this.tabProcedure.TabIndex = 1;
            this.tabProcedure.Text = "Procedure";
            this.tabProcedure.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 63);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "&Signature:";
            // 
            // txtSignature
            // 
            this.txtSignature.Location = new System.Drawing.Point(7, 82);
            this.txtSignature.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSignature.Multiline = true;
            this.txtSignature.Name = "txtSignature";
            this.txtSignature.Size = new System.Drawing.Size(455, 89);
            this.txtSignature.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkIsAlloca);
            this.groupBox1.Controls.Add(this.chkTerminates);
            this.groupBox1.Controls.Add(this.chkDecompile);
            this.groupBox1.Controls.Add(this.chkMalloc);
            this.groupBox1.Location = new System.Drawing.Point(7, 253);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(457, 173);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "C&haracteristics";
            // 
            // chkTerminates
            // 
            this.chkTerminates.AutoSize = true;
            this.chkTerminates.Location = new System.Drawing.Point(8, 77);
            this.chkTerminates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkTerminates.Name = "chkTerminates";
            this.chkTerminates.Size = new System.Drawing.Size(233, 19);
            this.chkTerminates.TabIndex = 8;
            this.chkTerminates.Text = "Procedure &terminates process or thread";
            this.chkTerminates.UseVisualStyleBackColor = true;
            // 
            // chkDecompile
            // 
            this.chkDecompile.AutoSize = true;
            this.chkDecompile.Location = new System.Drawing.Point(7, 50);
            this.chkDecompile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkDecompile.Name = "chkDecompile";
            this.chkDecompile.Size = new System.Drawing.Size(206, 19);
            this.chkDecompile.TabIndex = 7;
            this.chkDecompile.Text = "&Decompile body of this procedure";
            this.chkDecompile.UseVisualStyleBackColor = true;
            // 
            // chkMalloc
            // 
            this.chkMalloc.AutoSize = true;
            this.chkMalloc.Location = new System.Drawing.Point(7, 22);
            this.chkMalloc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkMalloc.Name = "chkMalloc";
            this.chkMalloc.Size = new System.Drawing.Size(256, 19);
            this.chkMalloc.TabIndex = 6;
            this.chkMalloc.Text = "Allocates &heap memory (like malloc, calloc)";
            this.chkMalloc.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 175);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "&Comment:";
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(7, 194);
            this.txtComment.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(460, 51);
            this.txtComment.TabIndex = 5;
            // 
            // chkIsAlloca
            // 
            this.chkIsAlloca.AutoSize = true;
            this.chkIsAlloca.Location = new System.Drawing.Point(8, 102);
            this.chkIsAlloca.Name = "chkIsAlloca";
            this.chkIsAlloca.Size = new System.Drawing.Size(215, 19);
            this.chkIsAlloca.TabIndex = 9;
            this.chkIsAlloca.Text = "&Allocates stack memory (like alloca)";
            this.chkIsAlloca.UseVisualStyleBackColor = true;
            // 
            // ProcedureDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(484, 524);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
        private System.Windows.Forms.CheckBox chkIsAlloca;
    }
}
