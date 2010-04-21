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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
	public class ScanPage : PhasePage
	{
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ContextMenu contextMenuList;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.ColumnHeader columnAddress;
		private System.Windows.Forms.ColumnHeader columnName;
		private System.Windows.Forms.ColumnHeader columnType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox ddlFilter;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ScanPage()
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
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnAddress = new System.Windows.Forms.ColumnHeader();
			this.columnType = new System.Windows.Forms.ColumnHeader();
			this.columnName = new System.Windows.Forms.ColumnHeader();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.contextMenuList = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.ddlFilter = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnAddress,
																						this.columnType,
																						this.columnName});
			this.listView1.Location = new System.Drawing.Point(0, 40);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(280, 312);
			this.listView1.TabIndex = 1;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnAddress
			// 
			this.columnAddress.Text = "Address";
			// 
			// columnType
			// 
			this.columnType.Text = "Type";
			this.columnType.Width = 81;
			// 
			// columnName
			// 
			this.columnName.Text = "Item";
			this.columnName.Width = 135;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(288, 40);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(304, 312);
			this.textBox1.TabIndex = 3;
			this.textBox1.Text = "textBox1";
			// 
			// contextMenuList
			// 
			this.contextMenuList.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.menuItem1,
																							this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "&View Item";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "&View Item as Code";
			// 
			// ddlFilter
			// 
			this.ddlFilter.Items.AddRange(new object[] {
														   "All Items",
														   "All Procedures",
														   "All Unidentified Blocks",
														   "All Vector Items"});
			this.ddlFilter.Location = new System.Drawing.Point(40, 8);
			this.ddlFilter.Name = "ddlFilter";
			this.ddlFilter.Size = new System.Drawing.Size(240, 21);
			this.ddlFilter.TabIndex = 0;
			this.ddlFilter.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "&Show:";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// ScanPage
			// 
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ddlFilter);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.listView1);
			this.Name = "ScanPage";
			this.Size = new System.Drawing.Size(600, 360);
			this.ResumeLayout(false);

		}
		#endregion

		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void label1_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
