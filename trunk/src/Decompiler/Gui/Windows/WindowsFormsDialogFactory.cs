#region License
/* 
* Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui.Forms;
using Decompiler.Gui.Windows.Forms;
using System;

namespace Decompiler.Gui.Windows
{
	public class WindowsFormsDialogFactory : IDialogFactory
	{
        private IServiceProvider services;

		public WindowsFormsDialogFactory(IServiceProvider services)
		{
            this.services = services;
		}

		public IAddressPromptDialog CreateAddressPromptDialog()
		{
			return new AddressPromptDialog();
		}

        public IAssembleFileDialog CreateAssembleFileDialog()
        {
            return new AssembleFileDialog
            {
                Services = services
            };
        }

        public IMainForm CreateMainForm()
        {
            return new MainForm();
        }

        public IOpenAsDialog CreateOpenAsDialog()
        {
            return new OpenAsDialog
            {
                Services = services,
            };
        }

        public IProgramPropertiesDialog CreateProgramPropertiesDialog(Program program)
        {
            return new ProgramPropertiesDialog
            {
                Services = services,
                Program = program,
            };
        }

        public ISearchDialog CreateSearchDialog()
        {
            return new SearchDialog()
            {
				Services = services,
            };
        }

        public IUserPreferencesDialog CreateUserPreferencesDialog()
        {
            return new UserPreferencesDialog
            {
                Services = services,
            };
        }
    }
}

