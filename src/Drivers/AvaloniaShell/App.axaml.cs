#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Reko.Core;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.Views;
using System.ComponentModel.Design;

namespace Reko.AvaloniaShell
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var sc = new ServiceContainer();
                var mainWindow = new MainWindow();
                MakeServices(sc, mainWindow);
                var mvvm = new MainViewModel(sc, mainWindow);
                mainWindow.DataContext = mvvm;
                desktop.MainWindow = mainWindow;
            }
            base.OnFrameworkInitializationCompleted();
        }

        private static void MakeServices(IServiceContainer services, MainWindow mainForm)
        {
            services.AddService<IDecompilerShellUiService>(new AvaloniaShellUiService(services, mainForm));
            services.AddService<IDialogFactory>(new AvaloniaDialogFactory(services));
            services.AddService<ISettingsService>(new AvaloniaSettingsService(services));
            services.AddService<IFileSystemService>(new FileSystemServiceImpl());
            services.AddService<IPluginLoaderService>(new PluginLoaderService());
        }
    }
}
