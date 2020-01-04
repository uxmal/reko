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
using Reko.Gui.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// Interactor class used to display the decompiler project as a tree view for user browsing.
    /// </summary>
    public class ProjectBrowserService : TreeNodeDesignerHost, IProjectBrowserService, ICommandTarget
    {
        /// <summary>
        /// This event is raised when a file is dropped on the browser service.
        /// </summary>
        public event EventHandler<FileDropEventArgs> FileDropped;

        private ITabPage tabPage;
        protected ITreeView tree;
        private Project project;

        public ProjectBrowserService(IServiceProvider services, ITabPage tabPage, ITreeView treeView)
            : base(treeView, services)
        {
            this.tabPage = tabPage;
            this.tree = treeView;
        }

        public Program CurrentProgram { get { return FindCurrentProgram(); } }

        public bool ContainsFocus { get { return tree.Focused;  } }

        public override void Clear()
        {
            base.Clear();
            Load(null);
        }

        public void Load(Project project)
        {
            var uiPrefsSvc = Services.RequireService<IUiPreferencesService>();
            uiPrefsSvc.UpdateControlStyle(UiStyles.Browser, tree);
            uiPrefsSvc.UiPreferencesChanged += delegate { uiPrefsSvc.UpdateControlStyle(UiStyles.Browser, tree); };
            Services.RequireService<IDecompilerShellUiService>().SetContextMenu(tree, MenuIds.CtxBrowser);
            base.Clear();
            if (project == null)
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
                project.MetadataFiles.CollectionChanged += TypeLibraries_CollectionChanged;
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
            tabPage.Select();
            tree.Focus();
        }


        private Program FindCurrentProgram()
        {
            var obj = SelectedObject;
            while (obj != null)
            {
                if (obj is Program program)
                    return program;
                var des = GetDesigner(obj);
                if (des.Parent == null)
                    return null;
                obj = des.Parent.Component;
            }
            return null;
        }

        protected virtual void OnFileDropped(FileDropEventArgs e)
        {
            FileDropped?.Invoke(this, e);
        }

        void TypeLibraries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            var des = base.GetSelectedDesigner();
            if (des != null)
                return des.QueryStatus(cmdId, status, text);
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.CollapseAllNodes: status.Status = MenuStatus.Visible | MenuStatus.Enabled; return true;
                case CmdIds.CreateUserSegment: status.Status = 
                        IsSegmentSelected()
                            ? MenuStatus.Visible | MenuStatus.Enabled
                            : MenuStatus.Visible;
                    return true;
                }
            }
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            var des = GetSelectedDesigner();
            if (des != null)
            {
                if (des.Execute(cmdId))
                    return true;
            }
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.CollapseAllNodes: tree.CollapseAll(); return true;
                case CmdIds.CreateUserSegment: CreateUserSegment(); return true;
                }
            }
            return false;
        }

        private bool IsSegmentSelected()
        {
            var des = GetSelectedDesigner();
            if (des == null)
                return false;
            return des.Component is ImageSegment;
        }

        private void CreateUserSegment()
        {
            var des = GetSelectedDesigner();
            if (des == null)
                return;
            var program = FindCurrentProgram();
            if (!(des.Component is ImageSegment segment))
                return;
            using (var dlg = Services.RequireService<IDialogFactory>().CreateSegmentEditorDialog())
            {
                dlg.LoadUserSegment(segment.MemoryArea.Bytes, new UserSegment
                {
                    Name = segment.Name,
                    Address = segment.Address,
                    Length = segment.Size,
                    AccessMode = segment.Access,
                    Architecture = program.Architecture
                });
                if (Services.RequireService<IDecompilerShellUiService>().ShowModalDialog(dlg) == DialogResult.OK)
                {

                }
            }
        }
    }
}
