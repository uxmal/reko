#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.Views;
using System.ComponentModel.Design;

namespace Reko.UserInterfaces.AvaloniaUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var sc = new ServiceContainer();
            var docFactory = new DockFactory(sc, new Project());
            var mainWindow = new MainWindow();

            MakeServices(sc, docFactory, mainWindow);
            var mvvm = new MainViewModel(sc, docFactory, mainWindow);
            mainWindow.DataContext = mvvm;
            mvvm.Interactor.Attach(mainWindow);

            switch (ApplicationLifetime)
            {
            case IClassicDesktopStyleApplicationLifetime desktop:
                {
                    desktop.MainWindow = mainWindow;
                    mainWindow.Closing += delegate
                    {
                        mvvm.CloseLayout();
                    };

                    desktop.MainWindow = mainWindow;

                    desktop.Exit += delegate
                    {
                        mvvm.CloseLayout();
                    };
                    break;
                }
            case ISingleViewApplicationLifetime singleView:
                {
                    var mainView = new MainView()
                    {
                        DataContext = mvvm
                    };

                    singleView.MainView = mainView;
                    break;
                }
            }
            base.OnFrameworkInitializationCompleted();
        }

        private static void MakeServices(IServiceContainer services, DockFactory dockFactory, MainWindow mainForm)
        {
            var factory = new AvaloniaTextSpanFactory();
            services.AddService<IDecompilerShellUiService>(new AvaloniaShellUiService(services, mainForm, dockFactory));
            services.AddService<IDialogFactory>(new AvaloniaDialogFactory(services));
            services.AddService<IWindowPaneFactory>(new AvaloniaWindowPaneFactory(services, factory));
            services.AddService<ISelectionService>(new SelectionService());
            services.AddService<ISelectedAddressService>(new SelectedAddressService());
            var fsSvc = new FileSystemService();
            services.AddService<IFileSystemService>(fsSvc);
            services.AddService<ISettingsService>(new FileSystemSettingsService(fsSvc));
            services.AddService<IPluginLoaderService>(new PluginLoaderService());

        }

        /*
        public static readonly Styles FluentDark = new Styles
        {
            new StyleInclude(new Uri("avares://Reko.UserInterfaces.AvaloniaUI/Styles"))
            {
                Source = new Uri("avares://Reko.UserInterfaces.AvaloniaUI/Themes/FluentDark.axaml")
            }
        };

        public static readonly Styles FluentLight = new Styles
        {
            new StyleInclude(new Uri("avares://Reko.UserInterfaces.AvaloniaUI/Styles"))
            {
                Source = new Uri("avares://Reko.UserInterfaces.AvaloniaUI/Themes/FluentLight.axaml")
            }
        };

        public static readonly Styles DefaultLight = new Styles
        {
            new StyleInclude(new Uri("avares://Reko.UserInterfaces.AvaloniaUI/Styles"))
            {
                Source = new Uri("avares://Reko.UserInterfaces.AvaloniaUI/Themes/DefaultLight.axaml")
            }
        };

        public static readonly Styles DefaultDark = new Styles
        {
            new StyleInclude(new Uri("avares://Reko.UserInterfaces.AvaloniaUI/Styles"))
            {
                Source = new Uri("avares://Reko.UserInterfaces.AvaloniaUI/Themes/DefaultDark.axaml")
            },
        };
        */
    }
}
