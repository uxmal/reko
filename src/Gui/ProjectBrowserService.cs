#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
    public class ProjectBrowserService : IProjectBrowserService, ITreeNodeDesignerHost, ICommandTarget
    {
        /// <summary>
        /// This event is raised when a file is dropped on the browser service.
        /// </summary>
        public event EventHandler<FileDropEventArgs> FileDropped;

        private ITabPage tabPage;
        protected ITreeView tree;
        private Dictionary<object, TreeNodeDesigner> mpitemToDesigner;
        private Project project;

        public ProjectBrowserService(IServiceProvider services, ITabPage tabPage, ITreeView treeView)
        {
            this.Services = services;
            this.tabPage = tabPage;
            this.tree = treeView;
            this.mpitemToDesigner = new Dictionary<object, TreeNodeDesigner>();
            this.tree.AfterSelect += tree_AfterSelect;
        }

        public IServiceProvider Services { get; private set; }
        public object SelectedObject
        {
            get { return GetSelectedObject(); }
            set { SetSelectedObject(value); }
        }

        public Program CurrentProgram { get { return FindCurrentProgram(); } }

        public bool ContainsFocus { get { return tree.Focused;  } }
        public void Clear()
        {
            Load(null);
        }

        public void Load(Project project)
        {
            var uiPrefsSvc = Services.RequireService<IUiPreferencesService>();
            uiPrefsSvc.UpdateControlStyle(UiStyles.Browser, tree);
            uiPrefsSvc.UiPreferencesChanged += delegate { uiPrefsSvc.UpdateControlStyle(UiStyles.Browser, tree); };
            Services.RequireService<IDecompilerShellUiService>().SetContextMenu(tree, MenuIds.CtxBrowser);
            tree.Nodes.Clear();
            this.mpitemToDesigner = new Dictionary<object, TreeNodeDesigner>();
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

        public void AddComponents(IEnumerable components)
        {
            var nodes = components
                .Cast<object>()
                .Select(o => CreateTreeNode(o, CreateDesigner(o), null));
            tree.Nodes.AddRange(nodes);
        }

        public void AddComponent(object parent, object component)
        {
            AddComponents(parent, new[] { component });
        }

        public void AddComponents(object parent, IEnumerable components)
        {
            TreeNodeDesigner parentDes = GetDesigner(parent);
            if (parentDes == null)
            {
                Debug.Print("No designer for parent object {0}", parent ?? "(null)");
                AddComponents(components);
                return;
            }
            var nodes = components
                .Cast<object>()
                .Select(o => CreateTreeNode(o, CreateDesigner(o), parentDes));
            parentDes.TreeNode.Nodes.AddRange(nodes);
        }

        private TreeNodeDesigner CreateDesigner(object o)
        {
            if (o == null)
                return null;
            TreeNodeDesigner des = o as TreeNodeDesigner;
            if (des != null)
            {
                if (des.Component != null)
                {
                    o = des.Component;
                }
            }
            if (des == null)
            {
                var attr = o.GetType().GetCustomAttributes(typeof(DesignerAttribute), true);
                if (attr.Length > 0)
                {
                    var desType = Type.GetType(
                        ((DesignerAttribute)attr[0]).DesignerTypeName,
                        false);
                    if (desType != null)
                        des = (TreeNodeDesigner)Activator.CreateInstance(desType);
                    else
                        des = new TreeNodeDesigner();
                }
                else
                {
                    des = new TreeNodeDesigner();
                }
            }
            mpitemToDesigner[o] = des;
            return des;
        }
        
        private ITreeNode CreateTreeNode(object o, TreeNodeDesigner des, TreeNodeDesigner parentDes)
        {
            var node = tree.CreateNode();
            node.Tag = des;
            node.Expand();
            des.Services = Services;
            des.Host = this;
            des.TreeNode = node;
            des.Component = o;
            des.Parent = parentDes;
            des.Initialize(o);

            return node;
        }

        private Program FindCurrentProgram()
        {
            var obj = SelectedObject;
            while (obj != null)
            {
                var program = obj as Program;
                if (program != null)
                    return program;
                var des = GetDesigner(obj);
                if (des.Parent == null)
                    return null;
                obj = des.Parent.Component;
            }
            return null;
        }

        public TreeNodeDesigner GetDesigner(object o)
        {
            if (o == null)
                return null;
            TreeNodeDesigner des;
            if (mpitemToDesigner.TryGetValue(o, out des))
                return des;
            else
                return o as TreeNodeDesigner;
        }

        public void RemoveComponent(object component)
        {
            throw new NotImplementedException();
        }

        private void tree_AfterSelect(object sender, EventArgs e)
        {
            var des = GetSelectedDesigner();
            if (des != null)
            {
                des.DoDefaultAction();
            }
        }

        private TreeNodeDesigner GetSelectedDesigner()
        {
            if (tree.SelectedNode == null)
                return null;
            return (TreeNodeDesigner) tree.SelectedNode.Tag;
        }

        private object GetSelectedObject()
        {
            var des = GetSelectedDesigner();
            if (des == null)
                return null;
            return des.Component;
        }

        private void SetSelectedObject(object component)
        {
            if (component == null)
                return;
            TreeNodeDesigner des;
            if (!mpitemToDesigner.TryGetValue(component, out des))
                return;
            tree.SelectedNode = des.TreeNode;
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
            var des = GetSelectedDesigner();
            if (des != null)
                return des.QueryStatus(cmdId, status, text);
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.CollapseAllNodes: status.Status = MenuStatus.Visible | MenuStatus.Enabled; return true;
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
                case CmdIds.CollapseAllNodes: tree.CollapseAll(); break;
                }
            }
            return false;
        }
    }
}
