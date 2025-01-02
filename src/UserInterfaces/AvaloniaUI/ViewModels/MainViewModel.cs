#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Dock.Model.Controls;
using Dock.Model.Core;
using ReactiveUI;
using Reko.Core.Diagnostics;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(MainViewModel), "Trace events in the MainViewModel");

        private readonly DockFactory dockFactory;

        public MainViewModel(IServiceContainer services, DockFactory dockFactory, IMainForm mainForm)
        {
            trace.Inform("MainViewModel constructor");

            this.dockFactory = dockFactory;
            DebugFactoryEvents(this.dockFactory);

            var svcFactory = new AvaloniaServiceFactory(services, this);
            services.AddService<IServiceFactory>(svcFactory);
            this.Layout = dockFactory.CreateLayout();
            if (Layout is not null)
            {
                dockFactory.InitLayout(Layout);
                if (Layout is { } root)
                {
                    root.Navigate.Execute("Home");
                }
            }

            this.ProjectBrowser = dockFactory.ProjectBrowserTool!;
            this.ProcedureList = dockFactory.ProcedureListTool!;
            this.DiagnosticsList = dockFactory.DiagnosticsListTool!;
            this.SearchResults = dockFactory.SearchResultsTool!;
            this.CallGraphNavigator = dockFactory.CallGraphNavigatorTool!;

            var cmdDefs = new CommandDefinitions();
            this.Interactor = new MainFormInteractor(services);
            Interactor.PropertyChanged += Interactor_PropertyChanged;
            this.CommandItems = new DecompilerCommandItems(cmdDefs, this.Interactor);
        }

        private void Interactor_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainFormInteractor.TitleText))
                this.WindowTitle = Interactor.TitleText;
        }

        /// <summary>
        /// The current dockable window layout.
        /// </summary>
        public IRootDock? Layout
        {
            get => layout;
            set => this.RaiseAndSetIfChanged(ref layout, value, nameof(Layout));
        }

        private IRootDock? layout;

        public MainFormInteractor Interactor { get; private set; }

        public DecompilerCommandItems CommandItems { get; private set; }

        public ObservableCollection<CommandItem> MainMenu => this.CommandItems.GetMenu(MenuIds.MainMenu);

        public ProjectBrowserViewModel ProjectBrowser { get; set; }

        public DiagnosticsViewModel DiagnosticsList { get; set; }

        public SearchResultsToolViewModel SearchResults { get; set; }

        public ProcedureListToolViewModel ProcedureList { get; set; }
        public CallGraphNavigatorToolViewModel CallGraphNavigator { get; set; }

        public AvaloniaStatusBarService? Status { get; set; }

        public string? WindowTitle 
        {
            get { return windowTitle ?? Interactor.TitleText; }
            set {
                this.RaiseAndSetIfChanged(ref windowTitle, value);
                Interactor.TitleText = value!;
            }
        }
        private string? windowTitle;

        [Conditional("DEBUG")]
        private void DebugFactoryEvents(IFactory factory)
        {
            factory.ActiveDockableChanged += (_, args) =>
            {
                trace.Verbose($"[ActiveDockableChanged] Title='{args.Dockable?.Title}'");
            };

            factory.FocusedDockableChanged += (_, args) =>
            {
                trace.Verbose($"[FocusedDockableChanged] Title='{args.Dockable?.Title}'");
            };

            factory.DockableAdded += (_, args) =>
            {
                trace.Verbose($"[DockableAdded] Title='{args.Dockable?.Title}'");
            };

            factory.DockableRemoved += (_, args) =>
            {
                trace.Verbose($"[DockableRemoved] Title='{args.Dockable?.Title}'");
            };

            factory.DockableClosed += (_, args) =>
            {
                trace.Verbose($"[DockableClosed] Title='{args.Dockable?.Title}'");
            };

            factory.DockableMoved += (_, args) =>
            {
                trace.Verbose($"[DockableMoved] Title='{args.Dockable?.Title}'");
            };

            factory.DockableSwapped += (_, args) =>
            {
                trace.Verbose($"[DockableSwapped] Title='{args.Dockable?.Title}'");
            };

            factory.DockablePinned += (_, args) =>
            {
                trace.Verbose($"[DockablePinned] Title='{args.Dockable?.Title}'");
            };

            factory.DockableUnpinned += (_, args) =>
            {
                trace.Verbose($"[DockableUnpinned] Title='{args.Dockable?.Title}'");
            };

            factory.WindowOpened += (_, args) =>
            {
                trace.Verbose($"[WindowOpened] Title='{args.Window?.Title}'");
            };

            factory.WindowClosed += (_, args) =>
            {
                trace.Verbose($"[WindowClosed] Title='{args.Window?.Title}'");
            };

            factory.WindowClosing += (_, args) =>
            {
                // NOTE: Set to True to cancel window closing.
#if false
                args.Cancel = true;
#endif
                trace.Verbose($"[WindowClosing] Title='{args.Window?.Title}', Cancel={args.Cancel}");
            };

            factory.WindowAdded += (_, args) =>
            {
                trace.Verbose($"[WindowAdded] Title='{args.Window?.Title}'");
            };

            factory.WindowRemoved += (_, args) =>
            {
                trace.Verbose($"[WindowRemoved] Title='{args.Window?.Title}'");
            };

            factory.WindowMoveDragBegin += (_, args) =>
            {
                // NOTE: Set to True to cancel window dragging.
#if false
                args.Cancel = true;
#endif
                trace.Verbose($"[WindowMoveDragBegin] Title='{args.Window?.Title}', Cancel={args.Cancel}, X='{args.Window?.X}', Y='{args.Window?.Y}'");
            };

            factory.WindowMoveDrag += (_, args) =>
            {
                trace.Verbose($"[WindowMoveDrag] Title='{args.Window?.Title}', X='{args.Window?.X}', Y='{args.Window?.Y}");
            };

            factory.WindowMoveDragEnd += (_, args) =>
            {
                trace.Verbose($"[WindowMoveDragEnd] Title='{args.Window?.Title}', X='{args.Window?.X}', Y='{args.Window?.Y}");
            };
        }


        public void CloseLayout()
        {
            if (Layout is IDock dock)
            {
                if (dock.Close.CanExecute(null))
                {
                    dock.Close.Execute(null);
                }
            }
        }

        public void ResetLayout()
        {
            if (Layout is not null)
            {
                if (Layout.Close.CanExecute(null))
                {
                    Layout.Close.Execute(null);
                }
            }

            var layout = dockFactory?.CreateLayout();
            if (layout is not null)
            {
                Layout = layout;
                dockFactory?.InitLayout(layout);
            }
        }


        public void SetMenuStatus(IList<CommandItem> items)
        {
            CommandItems.SetStatusForMenuItems(items);
        }
    }
}
