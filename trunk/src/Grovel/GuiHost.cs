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

using Decompiler;
using System;
using System.Windows.Forms;	

namespace Grovel
{
	public class GuiHost : DecompilerHost
	{
		private ListView diags;

		public GuiHost()
		{
		}

		public ListView DiagnosticsControl
		{
			get { return diags; }
			set { diags = value; }
		}

		#region DecompilerHost Members //////////////////////////////////

		public System.IO.TextWriter DisassemblyWriter
		{
			get
			{
				// TODO:  Add GuiHost.DisassemblyWriter getter implementation
				return null;
			}
		}

		public System.IO.TextWriter TypesWriter
		{
			get
			{
				// TODO:  Add GuiHost.TypesWriter getter implementation
				return null;
			}
		}

		public void ShowProgress(string caption, int numerator, int denominator)
		{
			// TODO:  Add GuiHost.ShowProgress implementation
		}

		public void Finished()
		{
			// TODO:  Add GuiHost.Finished implementation
		}

		public void WriteDiagnostic(Diagnostic d, string format, params object[] args)
		{
			if (this.DiagnosticsControl != null)
			{
				ListViewItem li = new ListViewItem(string.Format(format, args));
				DiagnosticsControl.Items.Add(li);
			}
		}

		public System.IO.TextWriter DecompiledCodeWriter
		{
			get
			{
				// TODO:  Add GuiHost.DecompiledCodeWriter getter implementation
				return null;
			}
		}

		public System.IO.TextWriter IntermediateCodeWriter
		{
			get
			{
				// TODO:  Add GuiHost.IntermediateCodeWriter getter implementation
				return null;
			}
		}

		#endregion ////////////////////////////////////////////////////
	}
}
