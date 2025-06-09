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
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtName = new System.Windows.Forms.TextBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabProcedure = new System.Windows.Forms.TabPage();
            txtSignature = new System.Windows.Forms.TextBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            chkIsAlloca = new System.Windows.Forms.CheckBox();
            chkTerminates = new System.Windows.Forms.CheckBox();
            chkDecompile = new System.Windows.Forms.CheckBox();
            chkMalloc = new System.Windows.Forms.CheckBox();
            txtComment = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            txtVarargsFormatParser = new System.Windows.Forms.TextBox();
            tabControl1.SuspendLayout();
            tabProcedure.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(288, 483);
            btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(88, 27);
            btnOK.TabIndex = 10;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(386, 483);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 27);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 14);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(42, 15);
            label1.TabIndex = 0;
            label1.Text = "&Name:";
            // 
            // txtName
            // 
            txtName.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtName.Location = new System.Drawing.Point(7, 32);
            txtName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(459, 23);
            txtName.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabProcedure);
            tabControl1.Location = new System.Drawing.Point(1, 3);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(484, 473);
            tabControl1.TabIndex = 10;
            // 
            // tabProcedure
            // 
            tabProcedure.Controls.Add(txtSignature);
            tabProcedure.Controls.Add(groupBox1);
            tabProcedure.Controls.Add(txtComment);
            tabProcedure.Controls.Add(txtName);
            tabProcedure.Controls.Add(label4);
            tabProcedure.Controls.Add(label3);
            tabProcedure.Controls.Add(label1);
            tabProcedure.Location = new System.Drawing.Point(4, 24);
            tabProcedure.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabProcedure.Name = "tabProcedure";
            tabProcedure.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabProcedure.Size = new System.Drawing.Size(476, 445);
            tabProcedure.TabIndex = 1;
            tabProcedure.Text = "Procedure";
            tabProcedure.UseVisualStyleBackColor = true;
            // 
            // txtSignature
            // 
            txtSignature.Location = new System.Drawing.Point(7, 82);
            txtSignature.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtSignature.Multiline = true;
            txtSignature.Name = "txtSignature";
            txtSignature.Size = new System.Drawing.Size(455, 89);
            txtSignature.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtVarargsFormatParser);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(chkIsAlloca);
            groupBox1.Controls.Add(chkTerminates);
            groupBox1.Controls.Add(chkDecompile);
            groupBox1.Controls.Add(chkMalloc);
            groupBox1.Location = new System.Drawing.Point(7, 253);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(457, 173);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "C&haracteristics";
            // 
            // chkIsAlloca
            // 
            chkIsAlloca.AutoSize = true;
            chkIsAlloca.Location = new System.Drawing.Point(8, 102);
            chkIsAlloca.Name = "chkIsAlloca";
            chkIsAlloca.Size = new System.Drawing.Size(215, 19);
            chkIsAlloca.TabIndex = 9;
            chkIsAlloca.Text = "&Allocates stack memory (like alloca)";
            chkIsAlloca.UseVisualStyleBackColor = true;
            // 
            // chkTerminates
            // 
            chkTerminates.AutoSize = true;
            chkTerminates.Location = new System.Drawing.Point(8, 77);
            chkTerminates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkTerminates.Name = "chkTerminates";
            chkTerminates.Size = new System.Drawing.Size(233, 19);
            chkTerminates.TabIndex = 8;
            chkTerminates.Text = "Procedure &terminates process or thread";
            chkTerminates.UseVisualStyleBackColor = true;
            // 
            // chkDecompile
            // 
            chkDecompile.AutoSize = true;
            chkDecompile.Location = new System.Drawing.Point(7, 50);
            chkDecompile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkDecompile.Name = "chkDecompile";
            chkDecompile.Size = new System.Drawing.Size(206, 19);
            chkDecompile.TabIndex = 7;
            chkDecompile.Text = "&Decompile body of this procedure";
            chkDecompile.UseVisualStyleBackColor = true;
            // 
            // chkMalloc
            // 
            chkMalloc.AutoSize = true;
            chkMalloc.Location = new System.Drawing.Point(7, 22);
            chkMalloc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkMalloc.Name = "chkMalloc";
            chkMalloc.Size = new System.Drawing.Size(256, 19);
            chkMalloc.TabIndex = 6;
            chkMalloc.Text = "Allocates &heap memory (like malloc, calloc)";
            chkMalloc.UseVisualStyleBackColor = true;
            // 
            // txtComment
            // 
            txtComment.Location = new System.Drawing.Point(7, 194);
            txtComment.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtComment.Multiline = true;
            txtComment.Name = "txtComment";
            txtComment.Size = new System.Drawing.Size(460, 51);
            txtComment.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(7, 63);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(60, 15);
            label4.TabIndex = 2;
            label4.Text = "&Signature:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(7, 175);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(64, 15);
            label3.TabIndex = 4;
            label3.Text = "&Comment:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(8, 125);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(217, 15);
            label2.TabIndex = 10;
            label2.Text = "Use the following &varargs format parser:";
            // 
            // txtVarargsFor
            // 
            txtVarargsFormatParser.Location = new System.Drawing.Point(8, 143);
            txtVarargsFormatParser.Name = "txtVarargsFor";
            txtVarargsFormatParser.Size = new System.Drawing.Size(442, 23);
            txtVarargsFormatParser.TabIndex = 11;
            // 
            // ProcedureDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(484, 524);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(tabControl1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProcedureDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit Procedure";
            tabControl1.ResumeLayout(false);
            tabProcedure.ResumeLayout(false);
            tabProcedure.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtVarargsFormatParser;
        private System.Windows.Forms.Label label2;
    }
}
