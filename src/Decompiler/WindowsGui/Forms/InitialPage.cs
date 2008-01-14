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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class InitialPage : PhasePage
	{
		private System.Windows.Forms.RadioButton rdbBinary;
		private System.Windows.Forms.RadioButton rdbProject;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox txtFileName;
		private System.ComponentModel.IContainer components = null;

		public InitialPage()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.rdbBinary = new System.Windows.Forms.RadioButton();
			this.rdbProject = new System.Windows.Forms.RadioButton();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.txtFileName = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// rdbBinary
			// 
			this.rdbBinary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rdbBinary.Location = new System.Drawing.Point(8, 8);
			this.rdbBinary.Name = "rdbBinary";
			this.rdbBinary.Size = new System.Drawing.Size(432, 16);
			this.rdbBinary.TabIndex = 0;
			this.rdbBinary.Text = "Load &binary file";
			// 
			// rdbProject
			// 
			this.rdbProject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rdbProject.Location = new System.Drawing.Point(8, 32);
			this.rdbProject.Name = "rdbProject";
			this.rdbProject.Size = new System.Drawing.Size(432, 16);
			this.rdbProject.TabIndex = 1;
			this.rdbProject.Text = "Load decompiler &project";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point(368, 56);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "&Browse...";
			// 
			// txtFileName
			// 
			this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtFileName.Location = new System.Drawing.Point(8, 56);
			this.txtFileName.Name = "txtFileName";
			this.txtFileName.Size = new System.Drawing.Size(352, 20);
			this.txtFileName.TabIndex = 3;
			this.txtFileName.Text = "";
			// 
			// InitialPage
			// 
			this.Controls.Add(this.txtFileName);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.rdbProject);
			this.Controls.Add(this.rdbBinary);
			this.Name = "InitialPage";
			this.Size = new System.Drawing.Size(448, 184);
			this.ResumeLayout(false);

		}
		#endregion
	}
}

