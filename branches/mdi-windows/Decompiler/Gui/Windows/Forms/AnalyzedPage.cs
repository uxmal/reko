/* 
 * Copyright (C) 1999-2009 John Källén.
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
using System.Data;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
	public class AnalyzedPage : PhasePage
	{
		private System.Windows.Forms.CheckBox chkTypeRecovery;
        private RichTextBox txtProcedure;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AnalyzedPage()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.chkTypeRecovery = new System.Windows.Forms.CheckBox();
            this.txtProcedure = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // chkTypeRecovery
            // 
            this.chkTypeRecovery.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTypeRecovery.Location = new System.Drawing.Point(3, 333);
            this.chkTypeRecovery.Name = "chkTypeRecovery";
            this.chkTypeRecovery.Size = new System.Drawing.Size(584, 24);
            this.chkTypeRecovery.TabIndex = 0;
            this.chkTypeRecovery.Text = "Perform &Type Recovery";
            // 
            // txtProcedure
            // 
            this.txtProcedure.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProcedure.Location = new System.Drawing.Point(4, 4);
            this.txtProcedure.Name = "txtProcedure";
            this.txtProcedure.Size = new System.Drawing.Size(593, 323);
            this.txtProcedure.TabIndex = 1;
            this.txtProcedure.Text = "";
            // 
            // AnalyzedPage
            // 
            this.Controls.Add(this.txtProcedure);
            this.Controls.Add(this.chkTypeRecovery);
            this.Name = "AnalyzedPage";
            this.Size = new System.Drawing.Size(600, 360);
            this.ResumeLayout(false);

		}
		#endregion

		public CheckBox PerformTypeRecovery 
		{
			get { return chkTypeRecovery; }
		}

        public RichTextBox ProcedureText
        {
            get { return this.txtProcedure; }
        }
	}
}
