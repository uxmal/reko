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

using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
	public partial class InitialPage : PhasePage, Decompiler.Gui.IStartPage
	{
		public event EventHandler IsDirtyChanged;

        //private DirtyManager dirtyManager;
        //private RegexValidator addressValidator;
        //private System.Windows.Forms.Button btnBrowseInputFile;
        //private System.Windows.Forms.Button btnBrowseSourceFile;
        //private System.Windows.Forms.TextBox txtSourceFile;
        //private System.Windows.Forms.Label label2;
        //private System.Windows.Forms.Button btnBrowseHeaderFile;
        //private System.Windows.Forms.TextBox txtHeaderFile;
        //private System.Windows.Forms.Label label1;
        //private System.Windows.Forms.GroupBox groupBox1;
        //private System.Windows.Forms.GroupBox groupBox2;
        //private System.Windows.Forms.CheckBox chkAssemblerFile;
        //private System.Windows.Forms.TextBox txtAssemblerFile;
        //private System.Windows.Forms.Label label3;
        //private System.Windows.Forms.CheckBox chkIntermediateFile;
        //private System.Windows.Forms.TextBox txtInputFile;
        //private System.Windows.Forms.Button btnBrowseAssemblerFile;
        //private System.Windows.Forms.TextBox txtIntermediateFile;
        //private System.Windows.Forms.Button btnBrowseIntermediateFile;
        //private System.Windows.Forms.Label label4;
        //private System.Windows.Forms.TextBox txtLoadAddress;
        //private System.Windows.Forms.Label label5;
        //private OpenFileDialog openFileDialog;

		public InitialPage()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			dirtyManager.IsDirtyChanged += new EventHandler(dirtyManager_IsDirtyChanged);// TODO: Add any initialization after the InitializeComponent call
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		#endregion

		public bool IsDirty
		{
			get { return dirtyManager.IsDirty; }
			set { dirtyManager.IsDirty = value; }
		}

		public Button BrowseInputFile
		{
			get { return btnBrowseInputFile; }
		}

		public TextBox InputFile
		{
			get { return txtInputFile; }
		}

		public TextBox LoadAddress
		{
			get { return txtLoadAddress; }
		}

		public TextBox SourceFile
		{
			get { return txtSourceFile; }
		}

		public TextBox HeaderFile
		{
			get { return txtHeaderFile; }
		}

		public TextBox IntermediateFile
		{
			get { return txtIntermediateFile; }
		}

		public TextBox AssemblerFile
		{
			get { return txtAssemblerFile; }
		}

		public OpenFileDialog OpenFileDialog
		{
			get { return openFileDialog; }
		}

		private void dirtyManager_IsDirtyChanged(object sender, EventArgs e)
		{
			if (IsDirtyChanged != null)
				IsDirtyChanged(this, e);
		}
	}
}

