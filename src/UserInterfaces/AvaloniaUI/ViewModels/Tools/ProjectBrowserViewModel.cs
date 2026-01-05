#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Avalonia.Collections;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using Reko.Core;
using Reko.Core.Services;
using Reko.Gui.Controls;
using Reko.Gui.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools
{
    /// <summary>
    /// This is the view model for the tool window that hosts the 
    /// <see cref="ProjectBrowserService"/>.
    /// </summary>
    public class ProjectBrowserViewModel : Tool, ITabPage
    {
        private readonly IServiceProvider services;

        public ProjectBrowserViewModel(IServiceProvider services)
        {
            this.services = services;
            this.TreeView = new TreeViewModel();
        }

        public Project? Project { get; set; }

        public TreeViewModel TreeView { get; set; }

        public void BringToFront()
        {
            var x = base.Owner;
        }

        void ITabPage.Select()
        {
        }

        public void OnGotFocus()
        {
            var crSvc = services.RequireService<ICommandRouterService>();
            var pbSvc = services.GetService<IProjectBrowserService>();
            if (pbSvc is { })
            {
                crSvc.ActiveCommandTarget = pbSvc;
            }
        }

        public void SetMenuStatus(IList items)
        {
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            uiSvc.SetStatusForMenuItems(items);
        }
    }

    public class TreeViewModel : ReactiveObject, ITreeView, ICommandItemSource
    {
        private ObservableCollection<CommandItem> commandItems;

        public TreeViewModel()
        {
            this.Nodes = new TreeNodeCollection();
            this.commandItems = new ObservableCollection<CommandItem>();
        }

        public ObservableCollection<CommandItem> CommandItems { 
            get => commandItems;
            set => this.RaiseAndSetIfChanged(ref this.commandItems, value);
        }

        public bool Focused
        {
            get => focused;
            set =>this.RaiseAndSetIfChanged(ref focused, value);
        }
        private bool focused;

        public ITreeNode? SelectedNode
        {
            get { return selectedNode; }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedNode, value);
                this.AfterSelect?.Invoke(this, EventArgs.Empty);
            }
        }
        private ITreeNode? selectedNode;

        //$TODO: these are very WinForms specific
        public bool ShowNodeToolTips { get => false; set { } }
        public bool ShowRootLines { get => false; set { } }

        public ITreeNodeCollection Nodes { get; }

        public object ContextMenuStrip { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color ForeColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color BackColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

#pragma warning disable CS0067
        public event EventHandler? AfterSelect;
        public event EventHandler<TreeViewEventArgs>? AfterExpand;
        public event EventHandler<TreeViewEventArgs>? BeforeExpand;
        public event EventHandler? DragLeave;
        public event MouseEventHandler? MouseWheel;
        public event EventHandler? GotFocus;
        public event EventHandler? LostFocus;

        public void BringToFront()
        {
            throw new NotImplementedException();
        }

        public void CollapseAll()
        {
            throw new NotImplementedException();
        }

        public ITreeNode CreateNode()
        {
            return new TreeNode();
        }

        public ITreeNode CreateNode(string text)
        {
            return new TreeNode
            {
                Text = text
            };
        }

        public void Focus()
        {
            //$TODO: how to make the tree take the kbd focus.
        }

        public class TreeNodeCollection : AvaloniaList<ITreeNode>, ITreeNodeCollection
        {
            public TreeNodeCollection()
            {
            }

            public bool IsReadOnly => throw new NotImplementedException();

            public ITreeNode Add(string text)
            {
                var node = new TreeNode { Text = text };
                this.Add(node);
                return node;
            }
        }
    }

    public class TreeNode : ReactiveObject, ITreeNode
    {
        public TreeNode()
        {
            this.text = "";
            this.Nodes = new TreeViewModel.TreeNodeCollection();
        }

        public TreeNode(string text, params TreeNode[] nodes)
        {
            this.text = text;
            this.Nodes = new TreeViewModel.TreeNodeCollection();
            this.Nodes.AddRange(nodes);
        }

        public ITreeNodeCollection Nodes { get; }

        public string? ImageName { get; set; }

        public object? Tag { get; set; }

        public bool IsExpanded
        {
            get => isExpanded;
            set => this.RaiseAndSetIfChanged(ref isExpanded, value);
        }
        private bool isExpanded;

        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }
        private string text;

        public string? ToolTipText
        {
            get => toolTipText;
            set => this.RaiseAndSetIfChanged(ref toolTipText, value);
        }
        private string? toolTipText;

        public void Collapse()
        {
            this.IsExpanded = false;
        }

        public void Expand()
        {
            this.IsExpanded = true;
        }

        public void Invoke(Action action)
        {
            throw new NotImplementedException();
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }
    }

}
