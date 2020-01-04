#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
#endregion

using Reko.Core;
using System;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
	/// <summary>
	/// Summary description for GotoDialogInteractor.
	/// </summary>
	public class AddressPromptInteractor
	{
		private AddressPromptDialog dlg;

		public AddressPromptInteractor()
		{
		}

        public void Attach(AddressPromptDialog dlg)
        {
            this.dlg = dlg;
            dlg.OKButton.Click += new System.EventHandler(this.btnOK_Click);
        }

		public Address Address { get;set; }

		// Event handlers //////////////////

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			try
			{
                Address addr;
				Address.TryParse32(dlg.MemoryAddress.Text, out addr);
                Address = addr;
			} 
			catch 
			{
				MessageBox.Show(dlg, "The address is in an invalid format. Please use only hexadecimal digits.", dlg.Text);
			}
		}
    }
}
