#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;
using Reko.Gui.ViewModels.Documents;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Docks;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Views;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    /// <summary>
    /// This class is responsible for the layout of the dockable windows and documents
    /// in the shell.
    /// </summary>
    public class DockFactory : Factory
    {
        private readonly IServiceProvider services;
        private readonly object _context;
        private IRootDock? _rootDock;
        private IDocumentDock? _documentDock;

        public DockFactory(IServiceProvider services, object context)
        {
            this.services = services;
            _context = context;
        }

        public ProjectBrowserViewModel? ProjectBrowserTool { get; private set; }
        public ProcedureListViewModel? ProcedureList { get; private set; }
        public DiagnosticsViewModel? DiagnosticsList { get; private set; }
        public CallGraphNavigatorToolViewModel? CallGraphNavigator { get; private set; } 

        public override IDocumentDock CreateDocumentDock() => new CustomDocumentDock();

        public override IRootDock CreateLayout()
        {
            var syncCtx = SynchronizationContext.Current;
            if (syncCtx is null)
                throw new InvalidOperationException("Not running on UI thread.");

            var document1 = new MemoryViewModel {Id = "Document1", Title = "Memory View"};
            var document2 = new DocumentViewModel {Id = "Document2", Title = "Document2"};
            var document3 = new DocumentViewModel {Id = "Document3", Title = "Document3", CanClose = true};
            
            this.ProjectBrowserTool =  new ProjectBrowserViewModel { Id = "Tool1", Title = "Project browser"};
            this.ProcedureList = new ProcedureListViewModel {Id = "Tool2", Title = "Procedures"};

            this.DiagnosticsList = new DiagnosticsViewModel(syncCtx, services) {Id = "Tool3", Title = "Diagnostics"};
            var toolFindResults = new FindResultsViewModel {Id = "Tool4", Title = "Find Results"};
            this.CallGraphNavigator = new CallGraphNavigatorToolViewModel { Id = "Tool9", Title = "Call Graph Navigator"};
            var toolConsole = new ConsoleViewModel {Id = "Tool7", Title = "Console", CanClose = false, CanPin = false};
            var toolOutput = new OutputViewModel { Id = "Tool6", Title = "Output" };

            var toolVisualizer = new VisualizerViewModel { Id = "Tool6", Title = "Visualizer", CanClose = true, CanPin = false};
            
            var toolProperties = new PropertiesViewModel {Id = "Tool8", Title = "Tool8", CanClose = false, CanPin = true};

            var leftDock = new ProportionalDock
            {
                Proportion = 0.25,
                Orientation = Orientation.Vertical,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    new ToolDock
                    {
                        ActiveDockable = ProjectBrowserTool,
                        VisibleDockables = CreateList<IDockable>(ProjectBrowserTool, ProcedureList),
                        Alignment = Alignment.Left
                    }
                )
            };

            var bottomDock = new ProportionalDock
            {
                Proportion = 0.25,
                Orientation = Orientation.Horizontal,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    new ToolDock
                    {
                        ActiveDockable = DiagnosticsList,
                        VisibleDockables = CreateList<IDockable>(
                            DiagnosticsList, 
                            toolFindResults,
                            CallGraphNavigator,
                            toolConsole,
                            toolOutput),
                        Alignment = Alignment.Left
                    }
                )
            };

            var rightDock = new ProportionalDock
            {
                Proportion = 0.25,
                Orientation = Orientation.Vertical,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    new ToolDock
                    {
                        ActiveDockable = toolVisualizer,
                        VisibleDockables = CreateList<IDockable>(toolVisualizer),
                        Alignment = Alignment.Right,
                        GripMode = GripMode.Hidden
                    },
                    new ProportionalDockSplitter(),
                    new ToolDock
                    {
                        ActiveDockable = toolProperties,
                        VisibleDockables = CreateList<IDockable>(toolProperties),
                        Alignment = Alignment.Right,
                        GripMode = GripMode.AutoHide
                    }
                )
            };

            var documentDock = new CustomDocumentDock
            {
                IsCollapsable = false,
                ActiveDockable = document1,
                VisibleDockables = CreateList<IDockable>(document1, document2, document3),
                CanCreateDocument = true
            };

            var mainLayout = new ProportionalDock
            {
                Orientation = Orientation.Vertical,
                VisibleDockables = CreateList<IDockable>
                (
                    new ProportionalDock
                    {
                        Orientation = Orientation.Horizontal,
                        VisibleDockables = CreateList<IDockable>(
                            leftDock,
                            new ProportionalDockSplitter(),
                            documentDock,
                            new ProportionalDockSplitter(),
                            rightDock),
                    },
                    new ProportionalDockSplitter(),
                    bottomDock)
            };

            var homeView = new HomeViewModel
            {
                Id = "Home",
                Title = "Home",
                ActiveDockable = mainLayout,
                VisibleDockables = CreateList<IDockable>(mainLayout)
            };

            var rootDock = CreateRootDock();

            rootDock.IsCollapsable = false;
            rootDock.DefaultDockable = homeView;
            rootDock.VisibleDockables = CreateList<IDockable>(homeView);

            _documentDock = documentDock;
            _rootDock = rootDock;
            
            return rootDock;
        }

        public override void InitLayout(IDockable layout)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                //["Document1"] = () => new DemoDocument(),
                //["Document2"] = () => new DemoDocument(),
                //["Document3"] = () => new DemoDocument(),
                ["Tool1"] = () => this.ProjectBrowserTool!,
                //["Tool2"] = () => new Tool2(),
                //["Tool3"] = () => new Tool3(),
                //["Tool4"] = () => new Tool4(),
                //["Tool5"] = () => new Tool5(),
                //["Tool6"] = () => new Tool6(),
                //["Tool7"] = () => new Tool7(),
                //["Tool8"] = () => new Tool8(),
                ["Home"] = () => _context
            };

            DockableLocator = new Dictionary<string, Func<IDockable?>>()
            {
                ["Root"] = () => _rootDock,
                ["Documents"] = () => _documentDock
            };

            HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            base.InitLayout(layout);
        }
    }
}
