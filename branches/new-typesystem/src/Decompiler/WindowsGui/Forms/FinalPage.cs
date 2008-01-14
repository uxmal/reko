/* 
 * Copyright (C) 1999-2008 John Källén.
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

using System;

namespace Decompiler.WindowsGui.Forms
{
	public class FinalPage : PhasePage
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnBrowseHeaderFile;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button btnBrowseSourceFile;
		private System.Windows.Forms.Label label2;
	
		public FinalPage()
		{
			InitializeComponent();
		}

		#region Windows Forms designer
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnBrowseHeaderFile = new System.Windows.Forms.Button();
			this.btnBrowseSourceFile = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(176, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Data types header file:";
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(8, 24);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(264, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "textBox1";
			// 
			// btnBrowseHeaderFile
			// 
			this.btnBrowseHeaderFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseHeaderFile.Location = new System.Drawing.Point(280, 24);
			this.btnBrowseHeaderFile.Name = "btnBrowseHeaderFile";
			this.btnBrowseHeaderFile.Size = new System.Drawing.Size(24, 23);
			this.btnBrowseHeaderFile.TabIndex = 2;
			this.btnBrowseHeaderFile.Text = "...";
			// 
			// btnBrowseSourceFile
			// 
			this.btnBrowseSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseSourceFile.Location = new System.Drawing.Point(280, 64);
			this.btnBrowseSourceFile.Name = "btnBrowseSourceFile";
			this.btnBrowseSourceFile.Size = new System.Drawing.Size(24, 23);
			this.btnBrowseSourceFile.TabIndex = 5;
			this.btnBrowseSourceFile.Text = "...";
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.Location = new System.Drawing.Point(8, 64);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(264, 20);
			this.textBox2.TabIndex = 4;
			this.textBox2.Text = "textBox2";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(176, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Source code file:";
			// 
			// FinalPage
			// 
			this.Controls.Add(this.btnBrowseSourceFile);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnBrowseHeaderFile);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.Name = "FinalPage";
			this.Size = new System.Drawing.Size(312, 104);
			this.ResumeLayout(false);

		}
		#endregion
	

	}
}
