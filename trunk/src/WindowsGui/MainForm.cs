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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Grovel
{
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Button btnFinish;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.TextBox txtLog;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			// Required for Windows Form Designer support
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
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
			this.btnNext = new System.Windows.Forms.Button();
			this.btnFinish = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnNext
			// 
			this.btnNext.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnNext.Location = new System.Drawing.Point(336, 248);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(72, 24);
			this.btnNext.TabIndex = 0;
			this.btnNext.Text = "&Next >";
			// 
			// btnFinish
			// 
			this.btnFinish.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnFinish.Location = new System.Drawing.Point(416, 248);
			this.btnFinish.Name = "btnFinish";
			this.btnFinish.Size = new System.Drawing.Size(72, 24);
			this.btnFinish.TabIndex = 1;
			this.btnFinish.Text = "&Finish >>";
			// 
			// btnSave
			// 
			this.btnSave.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnSave.Location = new System.Drawing.Point(512, 248);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(72, 24);
			this.btnSave.TabIndex = 2;
			this.btnSave.Text = "&Save";
			// 
			// txtLog
			// 
			this.txtLog.Location = new System.Drawing.Point(0, 280);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.Size = new System.Drawing.Size(592, 64);
			this.txtLog.TabIndex = 3;
			this.txtLog.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 341);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.txtLog,
																		  this.btnSave,
																		  this.btnFinish,
																		  this.btnNext});
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
