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

using Decompiler;
using Decompiler.Loading;
using Decompiler.Core;
using Decompiler.Core.Services;
using Decompiler.Core.Configuration;
using Decompiler.Gui;
using Decompiler.Gui.Forms;
using Decompiler.Gui.Windows;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.Mono
{
	public class Driver
	{
		[STAThread]
		public static void Main(string [] args)
		{
            var services = new ServiceContainer();
            if (args.Length == 0)
			{
                services.AddService(typeof(IServiceFactory), new ServiceFactory(services));
                services.AddService(typeof(IDialogFactory), new WindowsFormsDialogFactory(services));
                services.AddService(typeof(ISettingsService), new MonoSettingsService(services));
                var interactor = new MainFormInteractor(services);
                interactor.Run();
            }
			else
			{
                var host = NullDecompilerHost.Instance;
                var listener = NullDecompilerEventListener.Instance;

                services.AddService(typeof (DecompilerEventListener), listener);
                services.AddService(typeof(IConfigurationService), new DecompilerConfiguration());
                var ldr = new Loader(services);
				var dec = new DecompilerDriver(ldr, host, services);
				dec.Decompile(args[0]);
			}
		}
	}
}
