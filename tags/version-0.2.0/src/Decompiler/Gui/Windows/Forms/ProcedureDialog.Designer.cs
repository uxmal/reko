/* 
 * Copyright (C) 1999-2010 John Källén.
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

namespace Decompiler.Gui.Windows.Forms
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
            this.listArguments = new System.Windows.Forms.ListView();
            this.propArgument = new System.Windows.Forms.PropertyGrid();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabProcedure = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkMalloc = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.tabParameters = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRemoveArgument = new System.Windows.Forms.Button();
            this.btnAddArgument = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabProcedure.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Name:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(3, 28);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(394, 20);
            this.txtName.TabIndex = 3;
            // 
            // listArguments
            // 
            this.listArguments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listArguments.Location = new System.Drawing.Point(0, 0);
            this.listArguments.Name = "listArguments";
            this.listArguments.Size = new System.Drawing.Size(164, 230);
            this.listArguments.TabIndex = 4;
            this.listArguments.UseCompatibleStateImageBehavior = false;
            this.listArguments.View = System.Windows.Forms.View.List;
            // 
            // propArgument
            // 
            this.propArgument.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propArgument.Location = new System.Drawing.Point(0, 0);
            this.propArgument.Name = "propArgument";
            this.propArgument.Size = new System.Drawing.Size(233, 230);
            this.propArgument.TabIndex = 6;
            this.propArgument.ToolbarVisible = false;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(0, 288);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(401, 90);
            this.textBox2.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listArguments);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propArgument);
            this.splitContainer1.Size = new System.Drawing.Size(401, 230);
            this.splitContainer1.SplitterDistance = 164;
            this.splitContainer1.TabIndex = 9;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabProcedure);
            this.tabControl1.Controls.Add(this.tabParameters);
            this.tabControl1.Location = new System.Drawing.Point(1, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(415, 410);
            this.tabControl1.TabIndex = 10;
            // 
            // tabProcedure
            // 
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkMalloc);
            this.groupBox1.Location = new System.Drawing.Point(6, 145);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 224);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "C&haracteristics";
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
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "&Comment:";
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(3, 75);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(395, 64);
            this.txtComment.TabIndex = 4;
            // 
            // tabParameters
            // 
            this.tabParameters.Controls.Add(this.label2);
            this.tabParameters.Controls.Add(this.btnRemoveArgument);
            this.tabParameters.Controls.Add(this.btnAddArgument);
            this.tabParameters.Controls.Add(this.textBox2);
            this.tabParameters.Controls.Add(this.splitContainer1);
            this.tabParameters.Location = new System.Drawing.Point(4, 22);
            this.tabParameters.Name = "tabParameters";
            this.tabParameters.Padding = new System.Windows.Forms.Padding(3);
            this.tabParameters.Size = new System.Drawing.Size(407, 384);
            this.tabParameters.TabIndex = 0;
            this.tabParameters.Text = "Parameters";
            this.tabParameters.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 272);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Previe&w:";
            // 
            // btnRemoveArgument
            // 
            this.btnRemoveArgument.Location = new System.Drawing.Point(81, 236);
            this.btnRemoveArgument.Name = "btnRemoveArgument";
            this.btnRemoveArgument.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveArgument.TabIndex = 11;
            this.btnRemoveArgument.Text = "&Remove";
            this.btnRemoveArgument.UseVisualStyleBackColor = true;
            // 
            // btnAddArgument
            // 
            this.btnAddArgument.Location = new System.Drawing.Point(0, 236);
            this.btnAddArgument.Name = "btnAddArgument";
            this.btnAddArgument.Size = new System.Drawing.Size(75, 23);
            this.btnAddArgument.TabIndex = 10;
            this.btnAddArgument.Text = "&Add";
            this.btnAddArgument.UseVisualStyleBackColor = true;
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
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabProcedure.ResumeLayout(false);
            this.tabProcedure.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabParameters.ResumeLayout(false);
            this.tabParameters.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ListView listArguments;
        private System.Windows.Forms.PropertyGrid propArgument;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabProcedure;
        private System.Windows.Forms.TabPage tabParameters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRemoveArgument;
        private System.Windows.Forms.Button btnAddArgument;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.CheckBox chkMalloc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}