/* 
 * Copyright (C) 1999-2007 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
	public class LoadPage : PhasePage
	{
		private System.Windows.Forms.Label lblBinaryFileName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtAddress;
		private Decompiler.WindowsGui.Controls.MemoryControl memctl;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LoadPage()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblBinaryFileName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtAddress = new System.Windows.Forms.TextBox();
			this.memctl = new Decompiler.WindowsGui.Controls.MemoryControl();
			this.SuspendLayout();
			// 
			// lblBinaryFileName
			// 
			this.lblBinaryFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblBinaryFileName.Location = new System.Drawing.Point(8, 8);
			this.lblBinaryFileName.Name = "lblBinaryFileName";
			this.lblBinaryFileName.Size = new System.Drawing.Size(352, 16);
			this.lblBinaryFileName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "&Address:";
			// 
			// txtAddress
			// 
			this.txtAddress.Location = new System.Drawing.Point(56, 32);
			this.txtAddress.Name = "txtAddress";
			this.txtAddress.TabIndex = 2;
			this.txtAddress.Text = "";
			// 
			// memctl
			// 
			this.memctl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.memctl.BytesPerRow = 16;
			this.memctl.Font = new System.Drawing.Font("Lucida Console", 10F);
			this.memctl.ImageMap = null;
			this.memctl.Location = new System.Drawing.Point(8, 56);
			this.memctl.Name = "memctl";
			this.memctl.ProgramImage = null;
			this.memctl.SelectedAddress = null;
			this.memctl.Size = new System.Drawing.Size(352, 224);
			this.memctl.TabIndex = 3;
			this.memctl.TopAddress = null;
			this.memctl.WordSize = 1;
			// 
			// LoadPage
			// 
			this.Controls.Add(this.memctl);
			this.Controls.Add(this.txtAddress);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblBinaryFileName);
			this.Name = "LoadPage";
			this.Size = new System.Drawing.Size(368, 288);
			this.ResumeLayout(false);

		}
		#endregion

		[Browsable(false)]
		public ImageMap ImageMap
		{
			get { return memctl.ImageMap; }
			set { memctl.ImageMap = value; }
		}

		[Browsable(false)]
		public ProgramImage ProgramImage
		{
			get { return memctl.ProgramImage; }
			set {memctl.ProgramImage = value; }
		}
	}
}
