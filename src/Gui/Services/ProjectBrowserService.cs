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

using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Gui.Controls;
using System;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Gui.Services
{
    /// <summary>
    /// Interactor class used to display the decompiler project as a tree view
    /// for user browsing.
    /// </summary>
    public class ProjectBrowserService : TreeNodeDesignerHost, IProjectBrowserService, ICommandTarget
    {
        private static readonly TraceSwitch trace = new(nameof(ProjectBrowserService), "");

        private readonly ITabPage tabPage;
        protected readonly ITreeView tree;
        private Project? project;

        public ProjectBrowserService(
            IServiceProvider services,
            ITabPage tabPage,
            ITreeView treeView)
            : base(treeView, services)
        {
            this.tabPage = tabPage;
            tree = treeView;
        }

        public Program? CurrentProgram => FindCurrentProgram();

        public bool ContainsFocus => tree.Focused;

        public override void Clear()
        {
            base.Clear();
            Load(null);
        }

        public void Load(Project? project)
        {
            var uiPrefsSvc = Services.RequireService<IUiPreferencesService>();
            uiPrefsSvc.UpdateControlStyle(UiStyles.Browser, tree);
            uiPrefsSvc.UiPreferencesChanged += delegate { uiPrefsSvc.UpdateControlStyle(UiStyles.Browser, tree); };
            Services.RequireService<IDecompilerShellUiService>().SetContextMenu(tree, MenuIds.CtxBrowser);
            base.Clear();
            if (project is null)
            {
                tree.ShowRootLines = false;
                tree.ShowNodeToolTips = false;
                tree.Nodes.Clear();
                tree.Nodes.Add(tree.CreateNode("(No project loaded)"));
            }
            else
            {
                AddComponents(project.Programs);
                AddComponents(project.MetadataFiles);
                AddComponents(project.ScriptFiles);
                project.MetadataFiles.CollectionChanged +=
                    TypeLibraries_CollectionChanged;
                project.ScriptFiles.CollectionChanged +=
                    ScriptFiles_CollectionChanged;
                tree.ShowNodeToolTips = true;
                tree.ShowRootLines = true;
            }
            this.project = project;
        }

        public void Reload()
        {
            Load(project);
        }

        public void Show()
        {
            trace.Warn("WIP: Implement tabPage");
            tabPage?.Select();
            tree.Focus();
        }

        private Program? FindCurrentProgram()
        {
            var obj = SelectedObject;
            while (obj is not null)
            {
                if (obj is Program program)
                    return program;
                var des = GetDesigner(obj);
                if (des is null || des.Parent is null)
                    return null;
                obj = des.Parent.Component;
            }
            return null;
        }

        void TypeLibraries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
                AddComponents(e.NewItems);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        void ScriptFiles_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                {
                    AddComponents(e.NewItems);
                    SelectedObject = e.NewItems.OfType<object>().First();
                }
                break;
            default:
                throw new NotImplementedException();
            }
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            var des = GetSelectedDesigner();
            if (des is not null && des.QueryStatus(cmdId, status, text))
                return true;
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds) cmdId.ID)
                {
                case CmdIds.CollapseAllNodes: 
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                case CmdIds.EditSegments:
                    status.Status =
                        IsSegmentSelected()
                            ? MenuStatus.Visible | MenuStatus.Enabled
                            : MenuStatus.Visible;
                    return true;
                }
            }
            return false;
        }

        public async ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            var des = GetSelectedDesigner();
            if (des is not null)
            {
                if (await des.ExecuteAsync(cmdId))
                    return true;
            }
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds) cmdId.ID)
                {
                case CmdIds.CollapseAllNodes: tree.CollapseAll(); return true;
                case CmdIds.EditSegments: await EditSegments(); return true;
                }
            }
            return false;
        }

        private bool IsSegmentSelected()
        {
            var des = GetSelectedDesigner();
            if (des is null)
                return false;
            return des.Component is ImageSegment;
        }

        private async ValueTask EditSegments()
        {
            var des = GetSelectedDesigner();
            if (des is null)
                return;
            var program = FindCurrentProgram();
            if (program is null)
                return;
            if (des.Component is not ImageSegment segment)
                return;
            using (var dlg = Services.RequireService<IDialogFactory>().CreateSegmentEditorDialog())
            {
                dlg.LoadUserSegment(segment.MemoryArea, new UserSegment
                {
                    Name = segment.Name,
                    Address = segment.Address,
                    Length = segment.Size,
                    AccessMode = segment.Access,
                    Architecture = program.Architecture
                });
                if (await Services.RequireService<IDecompilerShellUiService>().ShowModalDialog(dlg) == DialogResult.OK)
                {

                }
            }
        }



    }
}
