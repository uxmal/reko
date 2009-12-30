using Decompiler.Core;
using System;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
	/// <summary>
	/// Summary description for GotoDialogInteractor.
	/// </summary>
	public class GotoDialogInteractor
	{
		private GotoDialog dlg;
		private Address addr;

		public GotoDialogInteractor(GotoDialog dlg)
		{
			this.dlg = dlg;
			dlg.OKButton.Click += new System.EventHandler(this.btnOK_Click);
		}

		public Address Address
		{
			get { return addr; }
			set { addr = value; }
		}

		// Event handlers //////////////////

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				addr = Address.ToAddress(dlg.Address.Text, 16);
			} 
			catch 
			{
				MessageBox.Show(dlg, "The address is in an invalid format. Please use only hexadecimal digits.", dlg.Text);
			}
		}
	}
}
