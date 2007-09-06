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

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Decompiler.WindowsGui.Controls
{
	/// <summary>
	/// Displays memory.
	/// </summary>
	public class MemoryControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.RichTextBox txtMemory;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MemoryControl()
		{
			// This call is required by the Windows.Forms Form Designer.
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
			this.txtMemory = new System.Windows.Forms.RichTextBox();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// txtMemory
			// 
			this.txtMemory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtMemory.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtMemory.Location = new System.Drawing.Point(0, 0);
			this.txtMemory.Name = "txtMemory";
			this.txtMemory.ReadOnly = true;
			this.txtMemory.Size = new System.Drawing.Size(160, 144);
			this.txtMemory.TabIndex = 0;
			this.txtMemory.Text = "richTextBox1";
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar1.Location = new System.Drawing.Point(143, 0);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 144);
			this.vScrollBar1.TabIndex = 1;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// MemoryControl
			// 
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.txtMemory);
			this.Name = "MemoryControl";
			this.Size = new System.Drawing.Size(160, 144);
			this.ResumeLayout(false);

		}
		#endregion

		private void vScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
		}
	}
}
