#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Loading;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.ComponentModel.Design;

namespace Reko.Mono
{
    public class Driver
	{
		[STAThread]
		public static void Main(string [] args)
		{
            var services = new ServiceContainer();
            if (args.Length == 0)
			{
                var mainForm = new MainForm();
                services.AddService(typeof(IServiceFactory), new WindowsServiceFactory(services, mainForm));
                services.AddService(typeof(IDialogFactory), new WindowsFormsDialogFactory(services));
                services.AddService(typeof(IRegistryService), new WindowsFormsRegistryService());
                services.AddService(typeof(ISettingsService), new WindowsFormsSettingsService(services));

                mainForm.Attach(services);
                System.Windows.Forms.Application.Run(mainForm);

            }
			else
			{
                var listener = NullDecompilerEventListener.Instance;

                services.AddService(typeof (DecompilerEventListener), listener);
                services.AddService(typeof(IRegistryService), new WindowsFormsRegistryService());
                services.AddService(typeof(IConfigurationService), RekoConfigurationService.Load());
                var ldr = new Loader(services);
				var dec = new DecompilerDriver(ldr, services);
				dec.Decompile(args[0]);
			}
		}
	}
}
